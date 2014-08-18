using Prototype.Model;
using Prototype.Model.Global_Classes;
using Prototype.Model.Global_Container_Classes;
using Prototype.Model.Incident_Sub_System;
using Prototype.Model.Incident_Sub_System.Custom_Controls;
using Prototype.Model.Resource_Sub_System;
using Prototype.Model.Resource_Sub_System.Container_Classes;
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
using System.Windows.Shapes;

namespace Prototype.View.Popup_Windows
{
    /// <summary>
    /// Interaction logic for MobilisingWindow.xaml
    /// </summary>
    public partial class ApplianceSelectionWindow : Window
    {
        #region Fields

        List<ApplianceCheckBox> checkBoxes; //the list of check boxes that are currently on the screen
        Incident incident; //the incident to mobilise resources to

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="incident">The incident to mobilise resources to</param>
        public ApplianceSelectionWindow(Incident incident)
        {
            InitializeComponent();

            //assign fields and instantiate objects
            this.incident = incident;
            checkBoxes = new List<ApplianceCheckBox>();

            //display relevant incident information
            lblAddress.Content = "Address: " + incident.Address.ToString();
            lblType.Content = "Incident Type: " + incident.Type.Name;
            lblResponse.Content = "Response: " + incident.Type.ResponseDescription;

            //display the appliance selection window
            buildResponse(incident);
        }

        #region Methods

        /// <summary>
        /// Builds a window that displays a groupbox for each appliance type required for the incident.
        /// Each group box has a series of check boxes, each representing a matching, available appliance together
        /// with their travel time.  The user chooses the appliances that will attend the incident in this window 
        /// and then they are mobilised when they click the mobilise button.
        /// </summary>
        /// <param name="incident">The incident to mobilise resources to</param>
        private void buildResponse(Incident incident)
        {
            //display a groupbox for each appliance type
            foreach (KeyValuePair<ApplianceType, int> type in incident.Type.Response)
            {
                GroupBox gb = new GroupBox();
                gb.Margin = new Thickness(10, 10, 10, 0);
                gb.Header = type.Key.Name;

                //give the group box a scoll bar
                ScrollViewer level1 = new ScrollViewer();
                level1.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                level1.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                level1.Height = 400;

                //the panel to add the appliance controls to
                StackPanel level2 = new StackPanel();

                //retrieve a list of all available appliances with a matching resource type
                List<KeyValuePair<string, int>> travelTimes = new List<KeyValuePair<string, int>>();
                foreach (KeyValuePair<string, Point> appliance in Tools.ResourceControllerDB.GetAppliances(type.Key.Name, true))
                {
                    //get the travel time from Google
                    int duration = Tools.AddressSearch.CalculateRoute(appliance.Value.Y, appliance.Value.X, incident.Address.Latitude, incident.Address.Longitude);
                    //int duration = Tools.AddressSearch.CalculateRouteDummy();

                    //add travel time to list
                    travelTimes.Add(new KeyValuePair<string, int>(appliance.Key, duration));
                }
            

                //sort the travel times, so that the station with the shortest travel time is first in the list
                travelTimes.Sort((x, y) => x.Value.CompareTo(y.Value));

                //Display a checkbox in the appliance type group box with the resource callsign and travel time 
                int mark = 0; //used to automatically check the required number of closest resources
                foreach (KeyValuePair<string, int> appliance in travelTimes)
                {
                    //create a new checkbox
                    ApplianceCheckBox checkBox = new ApplianceCheckBox(appliance.Key, appliance.Value);
                    checkBox.Content = appliance.Key + ": " + Tools.GetTimeString(appliance.Value);
                    checkBox.Margin = new Thickness(10);
                    
                    //check the new check box if the appropriate number of checkboxes have not yet been checked
                    if (mark < type.Value)
                    {
                        checkBox.IsChecked = true;
                        mark++;
                    }

                    //add the new checkbox to the window and store a reference to it
                    level2.Children.Add(checkBox);
                    checkBoxes.Add(checkBox);
                }

                //add all layers to the next layer, so that they are displayed propertly
                level1.Content = level2;
                gb.Content = level1;
                stackPanelResponse.Children.Add(gb);
            }
        }

        /// <summary>
        /// Builds the window that displays the confirmation of mobilisation method for all appliances to mobilise
        /// Each appliance has its own group box, each containing checkboxes that each represent a single mobilisation 
        /// method for that appliance
        /// </summary>
        /// <param name="appliancesToMobilise">A list of appliances to mobilise</param>
        private void buildMobiliser(string[] appliancesToMobilise)
        {
            //clear the window
            checkBoxes.Clear();
            stackPanelResponse.Children.Clear();

            //make design changes to the panels
            stackPanelResponse.Orientation = Orientation.Vertical;
            ScrollViewer level1 = new ScrollViewer();
            level1.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            level1.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            level1.Height = 400;
            StackPanel level2 = new StackPanel();

            //display the details for each appliance to mobilise
            foreach (string appliance in appliancesToMobilise)
            {
                //retrieve the current mobilising methods for the appliance
                string[] methods = Tools.ResourceDB.GetMobilisingMethods(appliance);

                //create a groupbox to store the mobilising methods in
                GroupBox applianceGroupBox = new GroupBox();
                applianceGroupBox.Header = appliance;
                applianceGroupBox.Margin = new Thickness(10);

                StackPanel applianceStackPanel = new StackPanel();
                applianceStackPanel.Orientation = Orientation.Horizontal;

                //add a new checkbox to the groupbox for each mobilising method
                foreach (string method in methods)
                {
                    ApplianceCheckBox cb = new ApplianceCheckBox(appliance, method);
                    cb.Content = method;
                    applianceStackPanel.Children.Add(cb);
                    cb.Margin = new Thickness(10);
                    checkBoxes.Add(cb);
                }

                //add all layers to each other so that they display correctly
                applianceGroupBox.Content = applianceStackPanel;
                level2.Children.Add(applianceGroupBox);
            }

            //add all layers to each other so that they display correctly
            level1.Content = level2;
            stackPanelResponse.Children.Add(level1);

            //enable/disable appropriate buttons
            btnConfirm.IsEnabled = true;
            btnMobilise.IsEnabled = false;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Fires when the user clicks the mobilise button.
        /// Reads the appliances that will be mobilised to the incident, and then passes control onto
        /// buildMobiliser() to display the mobilisation methods.
        /// </summary>
        private void Button_Click_Mobilise(object sender, RoutedEventArgs e)
        {
            List<string> appliancesToMobilise = new List<string>();

            //read callsign details if a checkbox is checked 
            foreach (ApplianceCheckBox cb in checkBoxes)
                if (cb.IsChecked == true)
                    appliancesToMobilise.Add(cb.ApplianceCallSign);

            //pass details of appliances to mobilise to the next stage
            buildMobiliser(appliancesToMobilise.ToArray());
        }

        /// <summary>
        /// Fires when a user has chosen the appliances to mobilise and their mobilisation methods
        /// and wishes to dispatch the appliances.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Confirm(object sender, RoutedEventArgs e)
        {
            //read the details for each checked checkbox and mobilise the resource using the selected mobilising method
            foreach (ApplianceCheckBox cb in checkBoxes)
                if (cb.IsChecked == true)
                    incident.AssignResource(cb.ApplianceCallSign, Properties.Settings.Default.LoggedInUserId, cb.MobilisingMethod);

            //close the window
            this.Close();
        }

        #endregion
    }
}
