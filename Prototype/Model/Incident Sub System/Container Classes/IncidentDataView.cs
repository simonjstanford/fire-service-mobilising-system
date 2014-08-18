using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Model.Incident_Sub_System.Container_Classes
{
    /// <summary>
    /// A container class that stores a subset of information for a single incident to display in a user interface, e.g. a Data Grid.
    /// This class is used for efficiency - only information for a single incident that is useful to display in a list of many incidents is retrieved
    /// from the database.  Use the 'Incident' object to retrieve full incident details.
    /// </summary>
    public class IncidentDataView : INotifyPropertyChanged
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="incNo">The unique incident number that is used to identify an incident.</param>
        /// <param name="dateTime">The date and time that the original call for the incident was placed.</param>
        /// <param name="type">The incident type name. For efficiency, this is not a full 'IncidentType' object</param>
        /// <param name="building">The 'building' section of the address.</param>
        /// <param name="number">The 'house number' section of the address.</param>
        /// <param name="street">The 'street' section of the address.</param>
        /// <param name="town">The 'town' section of the address.</param>
        /// <param name="postcode">The 'postcode' section of the address.</param>
        /// <param name="operatorName">The name of the operator that logged the incident</param>
        public IncidentDataView(int incNo, DateTime dateTime, string type, string building, string number, string street, string town, string postcode, string operatorName, DateTime stop, DateTime closed)
        {
            //assign values to the properties
            IncidentNumber = incNo;
            CallTime = dateTime.ToString();
            Type = type;
            OperatorName = operatorName;
            StopTime = stop;
            ClosedTime = closed;

            //build the address property using provided arguments
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
        /// The unique incident number that is used to identify an incident.
        /// </summary>
        public int IncidentNumber { get; set; }

        /// <summary>
        /// The date and time that the original call for the incident was placed.
        /// </summary>
        public string CallTime { get; set; }

        /// <summary>
        /// The incident type name. For efficiency, this is not a full 'IncidentType' object
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// A string representing the address of the incident.  Again, for efficiency this is not a full 'Address' object
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The name of the operator that logged the incident
        /// </summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// The time the stop for the incident was sent
        /// </summary>
        public DateTime StopTime { get; set; }

        /// <summary>
        /// The time the incident was closed
        /// </summary>
        public DateTime ClosedTime { get; set; }

        public string State
        {
            get 
            {
                if (ClosedTime != DateTime.MinValue)
                    return "Closed";
                else if (StopTime != DateTime.MinValue)
                    return "Stop Received";
                else
                    return "Open"; 
            }
        }
        

        #endregion

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
