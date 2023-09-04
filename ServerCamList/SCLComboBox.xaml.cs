using ServerConnection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ServerCamList
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SCLComboBox : UserControl
    {
        private ServerApi _api;
        private object _lockObj;
        
        private CancellationTokenSource _token ;
        private Queue<Thread> _threads;

        public event EventHandler? SelectedDataChanged;
        public ApiData? CurentData { get; private set; }
        public List<ApiData> apiDatas { get; private set; }

        public string SetServerUrl
        {
            get
            {
                return _api.ApiUrl;
            }
            set
            {
                //пересоздаем поток при изменении данных
                _api = new ServerApi(value);
                _token.Cancel();
                _token = new CancellationTokenSource();
                while (_threads.Count > 0)
                    lock (_lockObj)
                        _threads.Dequeue().Join();
                CancellationToken ct = _token.Token;
                lock (_lockObj)
                {
                    _threads.Enqueue(new Thread(()=>UpdateComboboxData(ct)));
                    _threads.Peek().Start();
                }

            }
        }

       

        public SCLComboBox()
        {
            InitializeComponent();
            _token = new CancellationTokenSource();
            apiDatas = new List<ApiData>();
            _threads = new Queue<Thread>();
            _api = new ServerApi();
            _lockObj = new object();
        }

        /// <summary>
        /// поток получение информации о камерах
        /// </summary>
        /// <param name="cancellationToken">токен для завершения потока</param>
        private void UpdateComboboxData(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _api.UpdateXML();
                List<ApiData> data = _api.GetData(_api.GetXmlDoc());
                lock (_lockObj)
                {
                    var query = apiDatas.Where(x => data.Any(nd => x.Id != nd.Id)).ToList();
                    if (query.Any())
                    {
                        bool SelectedItemChanged = false;
                        Dispatcher.BeginInvoke(
                              DispatcherPriority.Background,
                              new Action(() =>
                              {

                                  foreach (var item in query)
                                  {
                                      if (cbApiData.SelectedItem.Equals(item))
                                      {
                                          SelectedItemChanged = true;
                                      }
                                      cbApiData.Items.Remove(item);
                                  }

                              }));
                        if (SelectedItemChanged)
                            SelectedDataChanged?.Invoke(this, EventArgs.Empty);

                    }
                    query = apiDatas.Where(x => data.Any(nd => x.Id == nd.Id)).ToList()
                        .Concat(data.Where(x => !apiDatas.Any(nd => x.Id == nd.Id)).ToList()).ToList();

                    if (query.Any())
                    {
                        Dispatcher.BeginInvoke(
                              DispatcherPriority.Background,
                              new Action(() =>
                              {

                                  foreach (var item in query)
                                  {
                                      if (!cbApiData.Items.Contains(item))
                                      {
                                          cbApiData.Items.Add(item);
                                      }

                                  }

                              }));
                    }


                }
                Thread.Sleep(10000);
            }
        }
      
        private void cbApiData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //вызов внешних ивентов на срабатывание внутреннего (SelectionChanged)
            CurentData = (ApiData) cbApiData.SelectedItem;
            SelectedDataChanged?.Invoke(this, EventArgs.Empty);
        }

        public void StopThreads()
        {
            _token.Cancel();
            while (_threads.Count > 0)
                lock (_lockObj)
                    switch (_threads.Peek().ThreadState)
                    {
                        case ThreadState.Stopped:
                            _threads.Dequeue();
                            break;
                        case ThreadState.Running:
                            _threads.Dequeue().Join();
                            break;
                        case ThreadState.Unstarted:
                            _threads.Dequeue();
                            break;
                        case ThreadState.StopRequested: 
                            _threads.Dequeue();
                            break;
                        case ThreadState.WaitSleepJoin:
                            _threads.Dequeue().Join();
                            break;
                    }
        }
    }
}