using Prototype.Model.Global_Classes;
using Prototype.Model.Global_Container_Classes;
using Prototype.Model.Incident_Sub_System.Container_Classes;
using Prototype.Model.Incident_Sub_System.Data_Classes;
using Prototype.Model.Incident_Sub_System.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Prototype.Model.Incident_Sub_System
{
    /// <summary>
    /// Represents a single emergency incident that the Fire Service is called to.  
    /// Exposes properties that provide details of the incident as well as a method of updating some details
    /// on the database.
    /// </summary>
    public class Incident
    {
        #region Fields

        //private IncidentController incidentController;
        private IIncidentDB database; //object to retrieve/update details for a specific incident

        //container object that stores information about the incident
        //this is either passed to the Incident object in the constructor, or created by the incident
        private IncidentInfo info;

        //the object relating to this incident that is displayed in a datagrid view.  
        //The Incident object keeps this reference to notify it when a change has been made
        private IncidentDataView view;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="incidentNo">The unique incident number of the incident</param>
        /// <param name="database">The connection to the database that provides functionality for retrieving/updating data</param>
        public Incident(int incidentNo)
        {
            //assign parameters
            this.database = Tools.IncidentDB;
            IncidentNumber = incidentNo;

            //retrieve Incident information from the database
            info = database.GetAllDetails(IncidentNumber);

            //find the related incidentDataView object
            this.view = Tools.Incidents.Single(x => x.IncidentNumber == incidentNo);
        }

        /// <summary>
        /// Constructor - used when an IncidentInfo object HAS been created
        /// </summary>
        /// <param name="incidentNumber">The unique incident number of the incident</param>
        /// <param name="database">The connection to the database that provides functionality for retrieving/updating data</param>
        /// <param name="info">Container object that stores information about the incident</param>
        public Incident(int incidentNumber, IncidentInfo info)
        {
            IncidentNumber = incidentNumber;
            this.database = Tools.IncidentDB;
            this.info = info;
            this.view = new IncidentDataView(incidentNumber, DateTime, Type.Name, Address.Building, Address.Number, Address.Street, Address.Town, Address.Postcode, Operator, StopTime, IncidentClosedTime);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The unique incident number of the incident
        /// </summary>
        public int IncidentNumber { get; private set; }

        /// <summary>
        /// The original caller of the incident
        /// </summary>
        public string Caller
        {
            get { return info.Caller; }
            set
            {
                database.SetCaller(IncidentNumber, value);
                info.Caller = value;
            }
        }

        /// <summary>
        /// The telephone exchange that passed the details of the caller 
        /// </summary>
        public string Exchange
        {
            get { return info.Exchange; }
            set
            {
                database.SetExchange(IncidentNumber, value);
                info.Exchange = value;
            }
        }

        /// <summary>
        /// The incident type
        /// </summary>
        public IncidentType Type
        {
            get { return info.Type; }
            set
            {
                database.SetIncidentType(IncidentNumber, value);
                info.Type = value;
                view.Type = value.Name;
                view.OnPropertyChanged("Type");
            }
        }

        /// <summary>
        /// Any other important details relating to the incident
        /// </summary>
        public string Details
        {
            get { return info.Details; }
            set
            {
                database.SetDetails(IncidentNumber, value);
                info.Details = value;
            }
        }

        /// <summary>
        /// The address of the incident
        /// </summary>
        public Address Address
        {
            get { return info.Address; }
        }

        /// <summary>
        /// Any additional information stored in the database for the incident address
        /// </summary>
        public AdditionalAddressInfo AdditionalAddressInfo
        {
            get { return info.AdditionalAddressInfo; }
        }

        /// <summary>
        /// A summary of the incident - this is typically only completed when the incident is closed
        /// </summary>
        public string Summary
        {
            get { return info.Summary; }
            set
            {
                database.SetSummary(IncidentNumber, value);
                info.Summary = value;
            }
        }

        /// <summary>
        /// The officer if charge (OIC) of the incident
        /// </summary>
        public string OiC
        {
            get { return info.OiC; }
            set
            {
                database.SetOiC(IncidentNumber, value);
                info.OiC = value;
            }
        }

        /// <summary>
        /// Details of resources that were assigned to the incident
        /// </summary>
        public AssignedResource[] AssignedResources
        {
            get { return info.AssignedResources; }
        }

        /// <summary>
        /// The name of the operator that created the incident
        /// </summary>
        public string Operator
        {
            get { return info.Operator; }
        }

        #endregion

        #region DateTime Properties

        /// <summary>
        /// The date and time that the incident was originally reported
        /// </summary>
        public DateTime DateTime
        {
            get { return info.DateTime; }
        }

        /// <summary>
        /// The 'stop' time of the incident.  This is the time the emergency is declared over by the Fire Service
        /// </summary>
        public DateTime StopTime
        {
            get
            {
                info = database.GetAllDetails(IncidentNumber);
                return info.StopTime;
            }
            set
            {
                info.StopTime = value;

                view.StopTime = value;
                view.OnPropertyChanged("StopTime");
                view.OnPropertyChanged("State");

            }
        }

        /// <summary>
        /// The time the incident was closed on the system.  Typically, this is following a 
        /// stop message being placed, all resources returning and a summary being completed.
        /// </summary>
        public DateTime IncidentClosedTime
        {
            get { return info.IncidentClosedTime; }
            set
            {
                info.IncidentClosedTime = value;
                view.ClosedTime = value;
                view.OnPropertyChanged("ClosedTime");
                view.OnPropertyChanged("State");
            }
        }

        #endregion

        #region Formatted DateTime Properties

        /// <summary>
        /// Returns a UK formatted string representation of the DateTime property.
        /// If the value is DateTime.MinValue then an empty string is returned.
        /// </summary>
        public string DateTimeFormatted
        {
            get
            {
                if (info.DateTime != DateTime.MinValue)
                    return DateTime.ToString();
                else
                    return "";
            }
        }

        /// <summary>
        /// Returns a UK formatted string representation of the StppTime property.
        /// If the value is DateTime.MinValue then an empty string is returned.
        /// </summary>
        public string StopTimeFormatted
        {
            get
            {
                if (StopTime != DateTime.MinValue)
                    return StopTime.ToString();
                else
                    return "";
            }
        }

        /// <summary>
        /// Returns a UK formatted string representation of the IncidentClosed property.
        /// If the value is DateTime.MinValue then an empty string is returned.
        /// </summary>
        public string IncidentClosedTimeFormatted
        {
            get
            {
                if (IncidentClosedTime != DateTime.MinValue)
                    return IncidentClosedTime.ToString();
                else
                    return "";
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Allows a user to record a message in the database related to this incident
        /// </summary>
        /// <param name="user">The user that placed the message</param>
        /// <param name="text">The message text</param>
        /// <param name="type">The type of message, for example a standard message or 'stop' message</param>
        /// <param name="callSign">The callsign that placed the message</param>
        /// <returns>True if the message was insert successfully, false otherwise</returns>
        public bool EnterMessage(int user, string text, string type, string callSign)
        {
            if (type == "Stop")
                StopTime = DateTime.Now;

            return database.EnterMessage(IncidentNumber, user, text, type, callSign);
        }

        /// <summary>
        /// Allows a user to assign a resource to this incident
        /// </summary>
        /// <param name="callSign">The callsign of the resource to assign</param>
        /// <param name="user">The user that is assigning the resource</param>
        /// <param name="trigger">The method used to contact the resoure - for example station bells or voice contact</param>
        /// <returns>True if the resource was assigned successfully, false otherwise</returns>
        public bool AssignResource(string callSign, int user, string trigger)
        {
            return database.AssignResource(callSign, IncidentNumber, user, trigger);
        }

        /// <summary>
        /// Allows a user to close the incident.
        /// </summary>
        /// <returns>True if the incident was closed successfully, false otherwise</returns>
        public bool Close()
        {

            if (database.Close(IncidentNumber))
            {
                IncidentClosedTime = DateTime.Now;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Un-marks an incident as closed
        /// </summary>
        /// <returns>True if the incident was closed successfully, false otherwise</returns>
        public bool Reopen()
        {
            return database.Reopen(IncidentNumber);
        }

        /// <summary>
        /// Returns a log of all important events that have taken place for the incident,
        /// including messages, resource movements and the time the incident was closed.
        /// </summary>
        internal LogDataView[] History
        {
            get
            {
                //if the incident is not yet closed, just return the standard history
                if (IncidentClosedTime == DateTime.MinValue)
                    return database.GetHistory(IncidentNumber);
                else
                {
                    //else append the time the incident was closed to the history
                    List<LogDataView> log = new List<LogDataView>(database.GetHistory(IncidentNumber));
                    log.Add(new LogDataView(string.Empty, IncidentClosedTime, "Incident closed"));
                    return log.ToArray();
                }
            }
        }

        #endregion
    }

}
