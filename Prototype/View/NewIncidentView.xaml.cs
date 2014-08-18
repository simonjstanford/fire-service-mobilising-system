using Microsoft.Maps.MapControl.WPF;
using Prototype.Model;
using Prototype.Model.Gazetteer_Sub_System;
using Prototype.Model.Global_Classes;
using Prototype.Model.Global_Container_Classes;
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

namespace Prototype.View
{
    /// <summary>
    /// Interaction logic for NewIncidentView.xaml
    /// </summary>
    public partial class NewIncidentView : UserControl
    {
        public NewIncidentView()
        {
            InitializeComponent();

            CreateStations(); //draw the station pins on the map
            CreateIncidents(); //draw the open incident pins on the map
        }

        //The BIng Map WPF control does not support the MVVM pattern as you cannot use binding. 
        //This means that all logic for the map must be coded here.
        #region Bing Map Logic

        /// <summary>
        /// Moves and zooms the map to the address selected by the user.
        /// </summary>
        private void addressListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //first remove any pushpins from previous user searches that are on the map
            //this works as pins for addresses the user searches for are yellow
            removePushPins(Brushes.Yellow);

            if (addressListBox.SelectedItem != null) //only run if an address has been selected
            {
                Address address = (Address)addressListBox.SelectedItem;

                //mark the newly selected address on the map 
                zoomToLocationOnMap(address.Longitude, address.Latitude, 16, address.ToString());

                button_Calculate.IsEnabled = true;
                stackPanelCallCollectionResources.Children.Clear();
            }
        }

        /// <summary>
        /// Removes all pins from all maps that have the specified background colour
        /// </summary>
        /// <param name="brush">The background colour of the pins to remove</param>
        private void removePushPins(SolidColorBrush brush)
        {
            //search through all the maps and remove all pins with the colour specified by the user
            List<Pushpin> pinsToRemove = new List<Pushpin>();
            foreach (Pushpin pin in incidentMap.Children)
                if (pin.Background == brush)
                    pinsToRemove.Add(pin);

            foreach (Pushpin pin in pinsToRemove)
                incidentMap.Children.Remove(pin);
        }

        /// <summary>
        /// Zoom map to a location
        /// </summary>
        /// <param name="location">The location to zoom to</param>
        /// <param name="zoomLevel">The zoom level</param>
        private void zoomToLocationOnMap(double longitude, double latitude, int zoomLevel, string name = "")
        {
            //create the Bing Location object from the selected address
            Location selectedLocation = new Location(latitude, longitude);

            //set the map to the address and zoom in

            incidentMap.Center = selectedLocation;
            incidentMap.ZoomLevel = zoomLevel;

            if (name != "")
                incidentMap.Children.Add(BingMapsConnector.CreatePin(selectedLocation, Brushes.Yellow, name));
        }

        /// <summary>
        /// Creates Location objects and pushpins for all fire stations.
        /// </summary>
        private void CreateStations()
        {
            foreach (KeyValuePair<string, Point> app in Tools.ResourceControllerDB.GetAppliances("", false))
                incidentMap.Children.Add(BingMapsConnector.CreatePin(new Location(app.Value.Y, app.Value.X), Brushes.Blue, app.Key));
        }

        /// <summary>
        /// Creates Location objects and pushpins for all open incidents.
        /// </summary>
        private void CreateIncidents()
        {
            foreach (KeyValuePair<int, Point> inc in Tools.IncidentControllerDB.GetAllOpenIncidents())
                incidentMap.Children.Add(BingMapsConnector.CreatePin(new Location(inc.Value.Y, inc.Value.X), Brushes.Red, inc.Key.ToString()));
        }

        #endregion
    }
}
