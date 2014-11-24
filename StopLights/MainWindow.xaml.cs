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
    /// Group: 1-3, Ty Anderson, Cody Booher, Kevin Gardner, and Tyler White
    /// Last Edited On: 11/24/14
    /// Description: Models the behavior of a stoplight at the intersection of 300 N and 900 E in Provo UT.
    /// </summary>

    public partial class MainWindow : Window
    {
        //Value used to parse the int from the button names using substring in the button_Click handler.
        private const int AFTERBUTTON = 6;
       
        private const int ONESECOND = 1000;
        private const int TWOSECONDS = 2000;
        private const int TWENTYFOURSECONDS = 24000;

        private int currentLight; //Used to store the value of the stoplight that is currently enabled.
        private stopLight[] lights; //Holds all of the stoplight objects that have been created.
        private List<int> lightOrder; //Stores a list of stoplight IDs in the order in which the sensors detected a car. Switched to a list for the remove functionality.
        private DispatcherTimer dispatcherTimer; //Timer used fire a tick event on the thread.
        private bool transitioning; //Flag used to show that lights are switching. Used to prevent the stopwatch from incrementing as the lights change.
        private int stopWatch; //Holds the time (in seconds) since the lights changed or since a car was detected at a light.

        public MainWindow()
        {
            InitializeComponent();
        }

        //Initializes the stopLight objects, enables the buttons, sets the default light configuration.
        private async void startButton_Click(object sender, RoutedEventArgs e)
        {
            lightOrder = new List<int>();

            enableButtons();
            initializeStopLights();
            initializeDPTimer();

            await changeLights(2);
            stopWatch = 0;
            dispatcherTimer.Start();
        }

        private void enableButtons()
        {
            button1.IsEnabled = true;
            button2.IsEnabled = true;
            button3.IsEnabled = true;
            button4.IsEnabled = true;
        }

        //Connects the stoplight objects to the corresponding canvas groupings.
        private void initializeStopLights()
        {
            lights = new stopLight[4];

            for (int i = 0; i < 4; i++)
                lights[i] = new stopLight(i);

            LightOne.DataContext = lights[0];
            LightTwo.DataContext = lights[1];
            LightThree.DataContext = lights[2];
            LightFour.DataContext = lights[3];

            initalizeLightColors();
        }


        //Sets all of the lights to red when the start button is pushed.
        private void initalizeLightColors()
        {
            foreach (stopLight light in lights)
            {
                light.lightColor = Brushes.Red;
                light.arrowColor = Brushes.Red;
            }
        }

        //Creates an instance of a dispatcherTimer object. The dispatcherTimer has a tick event which fires at the timespan indicated.
        private void initializeDPTimer()
        {
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
        }

        //Uses a switch statement to set the lights to red or green based on boolean values.
        private async Task changeLights(int lightID) 
        {
            //Sets the current to the light we're changing to. Sets the flag for waiting cars at that light to false.
            currentLight = lightID;
            lights[currentLight].carsWaiting = false;

            //Designed to remove #2 or #4 from the queue if the other is set as the current light.
            if (lightID == 1 && lightOrder.Contains(3))
                lightOrder.Remove(3);
            if (lightID == 3 && lightOrder.Contains(1))
                lightOrder.Remove(1);

            //Changes the appropriate green lights to red.
            await transitionToRed(lightID);
            //Delays the change to green to make it seem less sudden.
            await Task.Delay(ONESECOND);

            switch (lightID)
            {
                case 0:
                    lights[0].lightColor = Brushes.Green;
                    break;
                case 1:
                case 3:
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

            //Updates the label with a list of the currently occupied lights.
            displayQueue();
            //Unsets the transitioning flag, indicating that the stopwatch can begin incrementing again.
            transitioning = false;
        }

        //Cycles through all of the lights and sets their values to red. Used to clear the lights before each new configuration.
        private async Task transitionToRed(int lightID)
        {
            foreach (stopLight light in lights)
            {
                if (light.lightColor == Brushes.Green)
                    light.lightColor = Brushes.Yellow;

                //Ensures that arrows only change colors for the cases where they need to (e.g., the transition from #2/#4 to 1 or from #3 to 1 or 2).
                if ((light.name == "Stoplight #4" && currentLight == 0) || (light.name == "Stoplight #3" && currentLight != 2))
                {
                    if (light.arrowColor == Brushes.Green)
                        light.arrowColor = Brushes.Yellow;
                }
            }

            //Delay to make yellow lights more visible.
            await Task.Delay(TWOSECONDS);

            foreach (stopLight light in lights)
            {
                if (light.lightColor == Brushes.Yellow)
                    light.lightColor = Brushes.Red;
                if (light.arrowColor == Brushes.Yellow)
                    light.arrowColor = Brushes.Red;
            }
        }

        //All buttons publish to this button click event. It grabs the buttonID and handles the task of adding the button to waiting light list (lightOrder).  
        private void button_Click(object sender, RoutedEventArgs e)
        {

            //Uses the substring function to parse the name variable of the active button. Gets the ID of the light and then subtracts 1 to put it into a 0-based format.
            int lightID = Convert.ToInt32(((Button)sender).Name.Substring(AFTERBUTTON))-1;

            //Confirms that the light being pushed is not the current light. Used to avoid adding the current light to the queue of waiting lights.
            if (lightID != currentLight && !lightOrder.Contains(lightID)) 
            {
                //If the edge cases don't apply, add the light to the list. If there currently aren't any cars waiting, set the carswaiting flag on the current light to false and reset the stopwatch.
                if (!checkForCommonConfiguration(lightID))
                {
                    lightOrder.Add(lightID);

                    if (lightOrder.Count == 0)
                    {
                        //If no light has cars waiting and a car arrives at a light that isn't the current light, resets the stopwatch to 0.
                        lights[currentLight].carsWaiting = false;
                        stopWatch = 0;
                    }
                }

                //Updates the lightList label with a list of the currently occupied lights.
                displayQueue();
            }

            //Checks for the edge case of having a car go through a green light that isn't part of the current stoplight object 
            //(e.g., a car going through stoplight 4 when stoplight 2 is designated as the current light.
            if ((currentLight == 1 && lightID == 3) || (currentLight == 3 && lightID == 1) || (currentLight == 2 && lightID == 3))
            {
                lights[currentLight].carsWaiting = true;
            }
            else
            {
                lights[lightID].carsWaiting = true;
            }
        }

        private bool checkForCommonConfiguration(int lightID)
        {
            if (lightID == 3 || lightID == 1)
            {
                //Returns true if the light being added is #4 and the current light is #2 or #3. 
                //Will also return true if the light being added is #2 and the current light is #4.
                //Intended for situations where lights are green at stoplights other than the current light.
                if ((currentLight == 3 && lightID == 1) || (currentLight == 1 && lightID == 3) || (currentLight == 2 && lightID == 3))
                {
                    return true;
                }
            }

            return false;
        }

        private async void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //Checks to see if default state conditions are met.
            if (lightOrder.Count == 0 && currentLight == 2)
                return;

            //Increments the stopwatch if they light's aren't changing.
            if (!transitioning)
            {
                stopWatch++;
                stopWatchDisplay.Content = "Stopwatch: " + stopWatch.ToString();
            }

            //Sets a 24 second timer at 5 seconds if cars have arrived at a green light.
            if (stopWatch == 5 && lights[currentLight].carsWaiting)
            {
                await Task.Delay(TWENTYFOURSECONDS); //Pauses for 24 seconds. Only 24 because we're at 5 seconds and an additional second will be added by the stopwatch when this handler fires again.
                lights[currentLight].carsWaiting = false;
            } 
            //Transitions the light at 5 or 30 seconds if the carswaiting value is false;
            else if ((stopWatch == 5 || stopWatch == 30) && !lights[currentLight].carsWaiting)
            {
                transitioning = true; //Sets flag to pause the stopwatch
                stopWatch = 0; //Resets the stopwatch

                if (lightOrder.Count == 0 && currentLight != 2) //Transitions to the default stoplight (#3) if there are no remaining stoplights in the queue.
                    await changeLights(2);
                else
                {
                    int nextLight = lightOrder[0];
                    lightOrder.RemoveAt(0);
                    await changeLights(nextLight); //Gets the light at the front of the list and transitions to it.
                    
                }
            }
            
        }

        //Updates the lightList label with a list of the currently occupied lights.
        private void displayQueue()
        {
            //Reinitializes the list.
            lightList.Content = "Cars are waiting at:\n";

            foreach(int i in lightOrder)
            {
                //Adds all cars that are currently located in the waiting queue.
                lightList.Content += lights[i].name + "\n";
            }
        }
    }
}
