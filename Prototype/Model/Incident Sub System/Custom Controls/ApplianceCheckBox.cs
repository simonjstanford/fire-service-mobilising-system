using Prototype.Model.Resource_Sub_System;
using Prototype.Model.Resource_Sub_System.Container_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Prototype.Model.Incident_Sub_System.Custom_Controls
{
    /// <summary>
    /// Extends the CheckBox control to enable storing of relevant information against the checkbox
    /// </summary>
    class ApplianceCheckBox : CheckBox
    {
        #region Constructors

        /// <summary>
        /// Constructor - used when the checkbox will be used to display the travel times of appliances
        /// </summary>
        /// <param name="appliance">The unique appliance call sign associated with the checkbox</param>
        /// <param name="travelTime">The travel time in seconds that the associated call sign will take to reach the incident</param>
        public ApplianceCheckBox(string appliance, int travelTime)
            : base()
        {
            //assign values
            ApplianceCallSign = appliance;
            TravelTime = travelTime;
            MobilisingMethod = string.Empty;  //this property is not used, so set a default value
        }

        /// <summary>
        /// Constructor - used when the checkbox is used to display the mobilising methods of appliances
        /// </summary>
        /// <param name="appliance">The unique appliance call sign associated with the checkbox</param>
        /// <param name="mobilisingMethod">A single method of contacting the associated call sign to respond to an emergency</param>
        public ApplianceCheckBox(string appliance, string mobilisingMethod)
            : base()
        {
            //assign values
            ApplianceCallSign = appliance;
            TravelTime = -1; //this property is not used, so set a default value
            MobilisingMethod = mobilisingMethod;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The unique appliance call sign associated with the checkbox
        /// </summary>
        public string ApplianceCallSign { get; private set; }

        /// <summary>
        /// The travel time in seconds that the associated call sign will take to reach the incident
        /// </summary>
        public int TravelTime { get; private set; }

        /// <summary>
        /// A single method of contacting the associated call sign to respond to an emergency
        /// </summary>
        public string MobilisingMethod { get; private set; }

        #endregion
    }
}
