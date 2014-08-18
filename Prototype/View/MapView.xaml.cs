using Microsoft.Maps.MapControl.WPF;
using Prototype.Model;
using Prototype.Model.Global_Classes;
using Prototype.Model.Global_Container_Classes;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Prototype.View
{
    public partial class MapView : System.Windows.Controls.UserControl
    {
        public MapView()
        {
            InitializeComponent();

            CreateAppliances();
            CreateIncidents();
        }

        /// <summary>
        /// Creates Location objects and pushpins for all fire stations.
        /// </summary>
        private void CreateAppliances()
        {
            foreach (KeyValuePair<string, Point> app in Tools.ResourceControllerDB.GetAppliances("", false))
                largeMap.Children.Add(BingMapsConnector.CreatePin(new Location(app.Value.Y, app.Value.X), Brushes.Blue, app.Key));
        }

        /// <summary>
        /// Creates Location objects and pushpins for all open incidents.
        /// </summary>
        private void CreateIncidents()
        {
            foreach (KeyValuePair<int, Point> inc in Tools.IncidentControllerDB.GetAllOpenIncidents())
                largeMap.Children.Add(BingMapsConnector.CreatePin(new Location(inc.Value.Y, inc.Value.X), Brushes.Red, inc.Key.ToString()));
        }
    }
}