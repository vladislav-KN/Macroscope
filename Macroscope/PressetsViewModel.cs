using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Macroscope
{
    public class PressetsViewModel : INotifyPropertyChanged
    {
        private ConfigData selectedPreset;
        public ObservableCollection<ConfigData> Presets { get; set; }
        public ConfigData SelectedPreset
        {
            get { return selectedPreset; }
            set
            {
                selectedPreset = value;
                OnPropertyChanged("SelectedPreset");
            }
        }
        public PressetsViewModel()
        {
            Presets = new ObservableCollection<ConfigData>
            {
                new ConfigData {  Login =  "root",  ResolutionX=640,ResolutionY=480,FPS=25 },
                new ConfigData {  Login =  "root",  ResolutionX=640,ResolutionY=480,FPS=25 },
                new ConfigData {  Login =  "root",  ResolutionX=640,ResolutionY=480,FPS=25 }

            };
            SelectedPreset = Presets[0];
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
