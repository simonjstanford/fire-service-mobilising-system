using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Prototype.Model.Resource_Sub_System.Container_Classes
{
    /// <summary>
    /// A container class that stores a subset of information for a single appliance to display in a user interface, e.g. a Data Grid.
    /// This class is used for efficiency - only information for a single appliance that is useful to display in a list of many appliances is retrieved
    /// from the database.  Use the 'Appliance' object to retrieve full incident details.
    /// </summary>
    public class ApplianceDataView : INotifyPropertyChanged
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="callSign">The unique identification code of the resource</param>
        /// <param name="name">The name of the resource</param>
        /// <param name="oic">Gets/sets the appliance officer in charge (OiC)</param>
        /// <param name="crew">Gets/sets the number of crew on the appliance</param>
        /// <param name="ba">Gets/sets the number of breathing apparatus wearers (BA) on the appliance</param>
        /// <param name="type">Gets/sets the appliance type associated with this appliance.</param>
        /// <param name="building">The 'building' component of the address</param>
        /// <param name="number">The 'house number' component of the address</param>
        /// <param name="street">The 'street' component of the address</param>
        /// <param name="town">The 'town' component of the address</param>
        /// <param name="postcode">The 'postcode' component of the address</param>
        /// <param name="status">The current status of the resource</param>
        /// <param name="assignedIncident">The incident number that the appliance is currently assigned to.  This will be -1 if not assigned to any incident</param>
        public ApplianceDataView(string callSign, string name, string oic, int crew, int ba, string type, string building, string number, string street, string town, string postcode, string status, int assignedIncident)
        {
            //assign parameters values to fields
            CallSign = callSign;
            Name = name;
            OiC = oic;
            Crew = crew;
            BA = ba;
            Type = type;
            Status = status;
            Incident = assignedIncident;

            //build the address from the provided address components.
            string address = "";

            if (!string.IsNullOrWhiteSpace(building))
                address += building + ", ";

            if (!string.IsNullOrWhiteSpace(number))
                address += number + " ";

            if (!string.IsNullOrWhiteSpace(street))
                address += street + ", ";

            if (!string.IsNullOrWhiteSpace(town))
                address += town;

            if (!string.IsNullOrWhiteSpace(postcode))
                address += ", " + postcode;

            Address = address;
        }

        #region Properties

        /// <summary>
        /// The unique identification code of the resource
        /// </summary>
        public string CallSign { get; set; }

        /// <summary>
        /// The name of the resource
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/sets the appliance officer in charge (OiC)
        /// </summary>
        public string OiC { get; set; }

        /// <summary>
        /// Gets/sets the number of crew on the appliance
        /// </summary>
        public int Crew { get; set; }

        /// <summary>
        /// Gets/sets the number of breathing apparatus wearers (BA) on the appliance
        /// </summary>
        public int BA { get; set; }

        /// <summary>
        /// Gets/sets the appliance type associated with this appliance.
        /// Appliance types determine what incidents an appliance is mobilised to - 
        /// if an incident requires 2 appliances of type 'Aerial', then this property is checked for all resources.  
        /// If it contains a type called 'Aerial' then the appliance is considered for mobilisation.
        /// For efficiency, this is not the full 'ApplianceType' object.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// A string representing the current location of the appliance.
        /// For efficiency, this is not a full 'Address' object.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// A string describing the current status of the object.
        /// For efficiency, this is not the full 'ResourceStatus' object.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The incident number the appliance is currently assigned to.  
        /// This value will be -1 if not assigned to any incident,
        /// </summary>
        public int Incident { get; set; }

        /// <summary>
        /// A formatted string representing the incident number the appliance is currenlty assigned to.
        /// The returned value will be an empty string if the appliance is not assigned to an incident.
        /// </summary>
        public string IncidentFormatted
        {
            get 
            {
                if (Incident != -1)
                    return Incident.ToString();
                else
                    return string.Empty;
            }
        }

        #endregion

        /// <summary>
        /// Overrides ToString() to display just the callsign
        /// </summary>
        /// <returns>The callsign of the appliance</returns>
        public override string ToString()
        {
            return CallSign;
        }

        //Used to enable WPF two way binding of controls
        #region INotifyPropertyChanged Implementation

        //from http://msdn.microsoft.com/en-us/library/ms743695(v=vs.110).aspx
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event 
        public void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}
