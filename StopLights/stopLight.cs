using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StopLights
{
    class stopLight
    {
        public bool isLightGreen { get; set; }
        public bool isArrowGreen { get; set; }
        public int amountCarsWaiting { get; set; }


        public stopLight() 
        {
            amountCarsWaiting = 0;
        }

    }
}
