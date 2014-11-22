using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace StopLights
{
    class stopLight : INotifyPropertyChanged
    {
        private Brush lightActive;
        private Brush arrowActive;
        private bool moreCars;
        public event PropertyChangedEventHandler PropertyChanged;

        public Brush lightColor 
        {
            get { return lightActive; }
            set
            { 
                lightActive = value;
                OnPropertyChanged("lightColor");
            }
        }

        public Brush arrowColor
        {
            get { return arrowActive; }
            set
            {
                arrowActive = value;
                OnPropertyChanged("arrowColor");
            }
        }

        public bool carsWaiting
        {
            get { return moreCars; }
            set
            {
                moreCars = value;
                OnPropertyChanged("carsWaiting");
            }
        }

        public stopLight() 
        {
            carsWaiting = false;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}
