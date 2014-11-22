using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace StopLights
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //Value used to parse the int from the button names using substring in the button_Click handler.
        private const int AFTERBUTTON = 6;
       
        private const int ONESECOND = 1000;
        private const int TWOSECONDS = 2000;
        private const int FIVESECONDS = 5000;
        private const int THIRTYSECONDS = 30000;

        private static int currentLight;
        private static stopLight[] lights;
        private static Queue<int> lightOrder;
        private static DispatcherTimer dispatcherTimer;
        private static bool transitioning;
        private static int stopWatch;

        public event PropertyChangedEventHandler PropertyChanged;

        private static string lightListText = "Cars are waiting at:\n";

        public string lightListHandler
        {
            get { return lightListText; }
            set
            {
                lightListText = value;
                OnPropertyChanged("lightListHandler");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private static void initializeDPTimer()
        {
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
        }

        public static async Task cycleLights()
        {
            changeLights(2);
            stopWatch = 0;
            dispatcherTimer.Start();
        }

        //Initializes the stopLight objects, enables the buttons, and sets the default light configuration.
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            lights = new stopLight[4];

            for (int i = 0; i < 4; i++)
                lights[i] = new stopLight(i);

            lightOrder = new Queue<int>();
            initializeStopLights();
            initializeDPTimer();
            cycleLights();
        }

        //Connects the stoplight objects to the corresponding canvas groupings.
        private void initializeStopLights()
        {
            LightOne.DataContext = lights[0];
            LightTwo.DataContext = lights[1];
            LightThree.DataContext = lights[2];
            LightFour.DataContext = lights[3];
            lightList.DataContext = lightListText;

            initalizeLightColors();
        }

        //Uses a switch statement to set the lights to red or green based on boolean values.
        private static async Task changeLights(int lightID) 
        {
            currentLight = lightID;
            lights[currentLight].carsWaiting = false;

            await transitionToRed();
            await Task.Delay(ONESECOND);

            switch (lightID)
            {
                case 0:
                    lights[0].lightColor = Brushes.Green;
                    break;
                case 1:
                    lights[1].lightColor = Brushes.Green;
                    lights[3].lightColor = Brushes.Green;
                    lights[3].arrowColor = Brushes.Green;
                    break;
                case 2:
                    lights[2].lightColor = Brushes.Green;
                    lights[2].arrowColor = Brushes.Green;
                    lights[3].arrowColor = Brushes.Green;
                    break;
                default:
                    break;
            }
        }

        //Cycles through all of the lights and sets their values to red. Used to clear the lights before each new configuration.
        private static async Task transitionToRed()
        {
            foreach (stopLight light in lights)
            {
                if (light.lightColor == Brushes.Green)
                    light.lightColor = Brushes.Yellow;
                if (light.arrowColor == Brushes.Green)
                    light.arrowColor = Brushes.Yellow;
            }

            await Task.Delay(TWOSECONDS);

            foreach (stopLight light in lights)
            {
                if (light.lightColor == Brushes.Yellow)
                    light.lightColor = Brushes.Red;
                if (light.arrowColor == Brushes.Yellow)
                    light.arrowColor = Brushes.Red;
            }

            transitioning = false;
        }

        private static void initalizeLightColors()
        {
            foreach (stopLight light in lights)
            {
                light.lightColor = Brushes.Red;
                light.arrowColor = Brushes.Red;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            int lightID = Convert.ToInt32(((Button)sender).Name.Substring(AFTERBUTTON))-1;

            if (lightID == 3)
                lightID = 1;

            if (lightID != currentLight && !lightOrder.Contains(lightID)) 
            { 
                if(lightOrder.Count == 0)
                    lights[currentLight].carsWaiting = false;

                lightOrder.Enqueue(lightID);
                
            }

            lights[lightID].carsWaiting = true;
        }

        private static async void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (lightOrder.Count == 0 && currentLight == 2)
                return;

            displayQueue();

            if (!transitioning)
                stopWatch++;

            if (lightOrder.Count == 0 && currentLight != 2 && stopWatch == 5)
                lightOrder.Enqueue(2);

            if ((stopWatch == 5 || stopWatch == 30) && !lights[currentLight].carsWaiting)
            {
                transitioning = true;
                stopWatch = 0; 
                await changeLights(lightOrder.Dequeue());  
            }
            else if (stopWatch == 5 && lights[currentLight].carsWaiting)
            {
                await Task.Delay(24000);
                lights[currentLight].carsWaiting = false;
            }
            
        }

        private static void displayQueue()
        {
            lightListText = "Cars are waiting at:\n";

            foreach(int i in lightOrder)
            {
                lightListText += lights[i].name;
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
    }
}
