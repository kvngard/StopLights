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

namespace StopLights
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Value used to parse the int from the button names using substring in the button_Click handler.
        private const int AFTERBUTTON = 6;
        private const int FIVESECONDS = 5000;
        private const int THIRTYSECONDS = 30000;
        private stopLight[] lights;

        public MainWindow()
        {
            InitializeComponent();
        }

        public async Task cycleLights()
        {
            while (true)
            {
                changeLights(3);
                await Task.Delay(FIVESECONDS);
                changeLights(2);
                await Task.Delay(FIVESECONDS);
                changeLights(1);
                await Task.Delay(FIVESECONDS);
            }
            
        }

        //Initializes the stopLight objects, enables the buttons, and sets the default light configuration.
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            lights = new stopLight[4];

            for (int i = 0; i < 4; i++)
                lights[i] = new stopLight();

            
            initializeStopLights();
            cycleLights();
        }

        //Connects the stoplight objects to the corresponding canvas groupings.
        private void initializeStopLights()
        {
            LightOne.DataContext = lights[0];
            LightTwo.DataContext = lights[1];
            LightThree.DataContext = lights[2];
            LightFour.DataContext = lights[3];
        }

        //Uses a switch statement to set the lights to red or green based on boolean values.
        private void changeLights(int carLocation) 
        { 
            switch(carLocation)
            {
                case 1:
                    turnLightsRed();
                    lights[0].isLightGreen = true;
                    break;
                case 2:
                case 4:
                    turnLightsRed();
                    lights[1].isLightGreen = true;
                    lights[3].isLightGreen = true;
                    lights[3].isArrowGreen = true;
                    break;
                case 3:
                    turnLightsRed();
                    lights[2].isLightGreen = true;
                    lights[2].isArrowGreen = true;
                    lights[3].isArrowGreen = true;
                    break;
                default:
                    break;
            }
        }

        //Cycles through all of the lights and sets their values to red. Used to clear the lights before each new configuration.
        private void turnLightsRed()
        {
            foreach (stopLight light in lights)
            {
                light.isArrowGreen = false;
                light.isLightGreen = false;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            int ID = Convert.ToInt32(((Button)sender).Name.Substring(AFTERBUTTON)) - 1;
            lights[ID].amountCarsWaiting++;
        }
        
    }
}
