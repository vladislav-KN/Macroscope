using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
using static System.Net.Mime.MediaTypeNames;

namespace Player
{
    /// <summary>
    /// Interaction logic for PlayerWindow.xaml
    /// </summary>
    public partial class PlayerWindow : UserControl, IStream
    {
        private HttpClient _client;
        private MjpegStream _stream;
        private MjpegOption _option;
        private Queue<Thread> _threads;
        private CancellationTokenSource _token;
        private object _lockObj;

        public string Url
        {
            get 
            {
                if (_option.Url is not null)
                    return _option.Url;
                else
                    return "";
            }
            set 
            {
                _token.Cancel();
                while (_threads.Count > 0)
                    _threads.Dequeue().Join();
                _token = new CancellationTokenSource();
                _option.Url = value;
                _stream.Options = _option;
                CancellationToken ct = _token.Token;
                _threads.Enqueue(new Thread(()=> _ = _stream.StreamAsync(ct)));
                _threads.Peek().Start();
            }
        }
        public int FPS { get; set; }
        public PlayerWindow()
        {
            InitializeComponent();
            _token = new CancellationTokenSource();
            _threads = new Queue<Thread>();
            _option = new MjpegOption();
            _client = new HttpClient(); 
            _stream = new MjpegStream(_client,this, _option);
            _lockObj = new object(); 
        }

        public void WriteImage(DateTime date, byte[] imageBytes)
        {
            Dispatcher.Invoke(()=> PlayerImage.Source = ConvertBytesToImage(imageBytes));
            Thread.Sleep(1000/FPS);
        }

        BitmapImage ConvertBytesToImage(byte[] bytes)
        {
            using (var ms = new System.IO.MemoryStream(bytes))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
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