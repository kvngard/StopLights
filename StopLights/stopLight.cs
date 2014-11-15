using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StopLights
{
    class stopLight : INotifyPropertyChanged
    {
        private bool lightActive;
        private bool arrowActive;
        private int carCount;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool isLightGreen 
        {
            get { return lightActive; }
            set
            { 
                lightActive = value;
                OnPropertyChanged("isLightGreen");
            }
        }

        public bool isArrowGreen
        {
            get { return arrowActive; }
            set
            {
                arrowActive = value;
                OnPropertyChanged("isArrowGreen");
            }
        }
        public int amountCarsWaiting
        {
            get { return carCount; }
            set
            {
                carCount = value;
                OnPropertyChanged("amountCarsWaiting");
            }
        }
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public stopLight() 
        {
            amountCarsWaiting = 0;
        }

        public void emptyWaitingQueue()
        {
            if (isArrowGreen || isLightGreen)
            {
                while (amountCarsWaiting != 0)
                    amountCarsWaiting--;
            }
        }
    }
}
