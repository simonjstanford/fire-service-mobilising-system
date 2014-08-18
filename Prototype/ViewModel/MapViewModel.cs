using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.ViewModel
{
    /// <summary>
    /// The view model responsible for display a large map
    /// </summary>
    class MapViewModel : WorkspaceViewModel
    {
        /// <summary>
        /// The text that appears in the tab header
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return "Map";
            }
        }

    }
}
