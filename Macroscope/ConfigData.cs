using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Macroscope
{
    public class ConfigData : INotifyPropertyChanged
    {
        private string login;
        private int resolutionX;
        private int resolutionY;
        private int fps;


        public string Login 
        {
            get 
            {
                return login;
            }
            set
            {
                login = value;
                OnPropertyChanged("Login");
            }
        }
      
        public int ResolutionX
        {
            get
            {
                return resolutionX;
            }
            set
            {
                if (value > 0 && value <= 1920)
                {
                    resolutionX = value;
                    OnPropertyChanged("ResolutionX");
                }
            }
        }
        public int ResolutionY
        {
            get
            {
                return resolutionY;
            }
            set
            {
                if (value > 0 && value <= 1080)
                {
                    resolutionY = value;
                    OnPropertyChanged("ResolutionY");
                }
            }
        }
        public int FPS
        {
            get
            {
                return fps;
            }
            set
            {
                if (value > 0 && value <= 60)
                {
                    fps = value;
                    OnPropertyChanged("FPS");
                }
                
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}