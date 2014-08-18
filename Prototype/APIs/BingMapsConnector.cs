using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Prototype
{
    class BingMapsConnector
    {
        /// <summary>
        /// Creates a pushpin on the map.
        /// </summary>
        /// <param name="location">The location where the pushpin should go on the map.</param>
        /// <param name="brush">The colour of the pushpin</param>
        /// <param name="toolTip">The pushpin tooltip - text that appears when the user hovers over it with a mouse</param>
        /// <returns>A reference to the created pushpin</returns>
       public static Pushpin CreatePin(Location location, SolidColorBrush brush, string toolTip)
        {
            Pushpin pin = new Pushpin();
            pin.Location = location;
            pin.Background = brush;
            pin.ToolTip = toolTip;
            return pin;
        }
    }
}
