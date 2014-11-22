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
        private int ID;
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

        public string name
        {
            get { return "Stoplight #" + ID.ToString(); }
            set
            {
                ID = Convert.ToInt32(value);
            }
        }

        public stopLight(int i) 
        {
            carsWaiting = false;
            name = (i + 1).ToString();
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
