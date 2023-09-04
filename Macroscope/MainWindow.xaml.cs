using ServerConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using ServerCamList;
using Player;

namespace Macroscope
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Config _serverConfig;
        public MainWindow()
        {
            InitializeComponent();
            _serverConfig = new Config();
            cbServerData.SetServerUrl = $"http://demo.macroscop.com:8080/configex?login={_serverConfig._ConfigDate.Login}";  
        }

        

        private void cbServerData_SelectedDataChanged(object sender, EventArgs e)
        {
            Player.FPS = _serverConfig._ConfigDate.FPS;
            Player.Url = $"http://demo.macroscop.com:8080/mobile?login={_serverConfig._ConfigDate.Login}&channelid=" +
                $"{cbServerData.CurentData?.Id}&resolutionX={_serverConfig._ConfigDate.ResolutionX}&resolutionY={_serverConfig._ConfigDate.ResolutionY}&fps={_serverConfig._ConfigDate.FPS}";
            
        }

 

        private void Window_Closed(object sender, EventArgs e)
        {
            Player.StopThreads();
            cbServerData.StopThreads();
            Application.Current.Shutdown();
        }


        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            _serverConfig.ShowDialog();
            cbServerData.SetServerUrl = $"http://demo.macroscop.com:8080/configex?login={_serverConfig._ConfigDate.Login}";
        }
    }
}
