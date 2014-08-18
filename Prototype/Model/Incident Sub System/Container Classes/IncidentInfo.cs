using Prototype.Model.Global_Classes;
using Prototype.Model.Global_Container_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Model.Incident_Sub_System.Container_Classes
{
    /// <summary>
    /// A container class that stores information relating to a single incident.  This object is created by the database when queried for
    /// incident information and passed to an 'Incident' object, which exposes the data.
    /// </summary>
    public class IncidentInfo
    {
        private int incidentNumber;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dateTime">The date and time of the incident</param>
        /// <param name="caller">The original caller for the incident</param>
        /// <param name="exchange">The telephone exchange that passed the details of the caller</param>
        /// <param name="type">The incident type</param>
        /// <param name="details">Any other important details relating to the incident</param>
        /// <param name="address">The address of the incident</param>
        /// <param name="additionalAddressInfo">Any additional information stored in the database for the incident address</param>
        /// <param name="summary">A summary of the incident - this is typically only completed when the incident is closed</param>
        /// <param name="oic">The officer if charge (OIC) of the incident</param>
        /// <param name="assignedResources">Details of resources that were assigned to the incident</param>
        /// <param name="operatorName">The name of the operator that created the incident</param>
        /// <param name="stopTime">The 'stop' time of the incident.  This is the time the emergency is declared over by the Fire Service</param>
        /// <param name="incidentClosedTime">The time the incident was closed on the system.  Typically, this is following a 
        /// stop message being placed, all resources returning and a summary being completed.</param>
        public IncidentInfo(int incidentNumber, DateTime dateTime, string caller, string exchange, IncidentType type, string details,
                      Address address, AdditionalAddressInfo additionalAddressInfo, string summary, string oic,
                      AssignedResource[] assignedResources, string operatorName, DateTime stopTime, DateTime incidentClosedTime)
        {
            //assign parameters to properties.
            this.incidentNumber = incidentNumber;
            DateTime = dateTime;
            Caller = caller;
            Exchange = exchange;
            Type = type;
            Details = details;
            Address = address;
            AdditionalAddressInfo = additionalAddressInfo;
            Summary = summary;
            OiC = oic;
            //AssignedResources = assignedResources;
            Operator = operatorName;
            StopTime = stopTime;
            IncidentClosedTime = incidentClosedTime;
        }

        #region Properties

        /// <summary>
        /// The date and time of the incident.
        /// </summary>
        public DateTime DateTime { get; private set; }

        /// <summary>
        /// The original caller for the incident
        /// </summary>
        public string Caller { get; set; }

        /// <summary>
        /// The telephone exchange that passed the details of the caller
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// The incident type
        /// </summary>
        public IncidentType Type { get; set; }

        /// <summary>
        /// Any other important details relating to the incident
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// The address of the incident
        /// </summary>
        public Address Address { get; private set; }

        /// <summary>
        /// Any additional information stored in the database for the incident address
        /// </summary>
        public AdditionalAddressInfo AdditionalAddressInfo { get; private set; }

        /// <summary>
        /// A summary of the incident - this is typically only completed when the incident is closed
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// The officer if charge (OIC) of the incident
        /// </summary>
        public string OiC { get; set; }

        /// <summary>
        /// Details of resources that were assigned to the incident
        /// </summary>
        public AssignedResource[] AssignedResources 
        { 
            get
            {
                return Tools.IncidentDB.GetAssignedResources(incidentNumber);
            }
        }

        /// <summary>
        /// The name of the operator that created the incident
        /// </summary>
        public string Operator { get; private set; }

        /// <summary>
        /// The 'stop' time of the incident.  This is the time the emergency is declared over by the Fire Service
        /// </summary>
        public DateTime StopTime { get; set; }

        /// <summary>
        /// The time the incident was closed on the system.  Typically, this is following a 
        /// stop message being placed, all resources returning and a summary being completed.
        /// </summary>
        public DateTime IncidentClosedTime { get; set; }

        #endregion
    }
}
