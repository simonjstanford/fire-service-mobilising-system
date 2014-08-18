using Prototype.Model.Global_Classes;
using Prototype.Model.Global_Container_Classes;
using Prototype.Model.Resource_Sub_System.Container_Classes;
using System;
using System.ComponentModel;
using System.Windows;

namespace Prototype.Model.Resource_Sub_System
{
    /// <summary>
    /// An abstract base class that provides generic functionality for appliances and officers.
    /// </summary>
    public abstract class Resource
    {
        #region Fields

        protected IResourceDB database; //the database connection object.  Referenced through an inteface to allow for polymorphism
        private string name;
        private string mobilePhoneNumber;
        private Base baseLocation;
        private Address currentLocation;
        private ResourceStatus currentStatus;
        private int assignedIncident;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="callSign">The unique identification code of the resource</param>
        /// <param name="database">The database connection interface for retrieving generic resource information</param>
        /// <param name="name">The name of the resource</param>
        /// <param name="mobilePhoneNumber">The resource's mobile phone number</param>
        /// <param name="baseLocation">The base location of the resource</param>
        /// <param name="currentLocation">The current location of the resource</param>
        /// <param name="currentStatus">The current resource status</param>
        public Resource(string callSign, string name, string mobilePhoneNumber, Base baseLocation, Address currentLocation, ResourceStatus currentStatus, int assignedIncident)
        {
            //assign parameters to fields
            CallSign = callSign;
            this.database = Tools.ResourceDB;

            this.name = name;
            this.mobilePhoneNumber = mobilePhoneNumber;
            this.baseLocation = baseLocation;
            this.currentLocation = currentLocation;
            this.currentStatus = currentStatus;
            this.assignedIncident = assignedIncident;
        }

        #region Properties

        //the resource call sign.  A unique identification code for each resource, this property 
        //is used to retrieve info from the database using the properties/methods below.
        public string CallSign { get; private set; }

        /// <summary>
        /// The name of the resource
        /// </summary>
        public virtual string Name
        {
            get { return name; }
            set
            {
                database.SetName(callSign: CallSign, newName: value);
                name = value;
            }
        }

        /// <summary>
        /// Returns the mobile phone number assigned to the resource
        /// </summary>
        public string MobilePhoneNumber
        {
            get { return mobilePhoneNumber; }
            set
            {
                database.SetMobilePhoneNumber(callSign: CallSign, newNumber: value);
                mobilePhoneNumber = value;
            }
        }

        /// <summary>
        /// Returns the base location assigned to the resource
        /// </summary>
        public Base Base
        {
            get { return baseLocation; }
            set
            {
                database.SetBaseInfo(callSign: CallSign, newBase: value);
                baseLocation = value;
            }

        }

        /// <summary>
        /// Returns the current location of the resource.
        /// See SetCurrentAddress() for the setter.
        /// </summary>
        public virtual Address CurrentAddress
        {
            get { return currentLocation; }
            set
            {
                database.SetCurrentAddress(CallSign, value, DateTime.Now, Properties.Settings.Default.LoggedInUserId);
                currentLocation = value;
            }
        }

        /// <summary>
        /// Returns the history of all resource movements and status changes.
        /// </summary>
        public LogDataView[] History
        {
            get { return database.GetHistory(callSign: CallSign); }
        }

        /// <summary>
        /// Returns an array of the various methods that can be used to mobilise the resource to an incident
        /// </summary>
        public string[] MobilisingMethods
        {
            get { return database.GetMobilisingMethods(callsign: CallSign); }
        }

        /// <summary>
        /// Returns the current status of the resource.
        /// See SetCurrentResourceStatus() for the setter
        /// </summary>
        public virtual ResourceStatus CurrentResourceStatus
        {
            get { return currentStatus; }
            set 
            {
                currentStatus = value;
                SetCurrentResourceStatus(value, DateTime.Now, Properties.Settings.Default.LoggedInUserId, assignedIncident); 
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the current status of the resource.
        /// </summary>
        /// <param name="code">The new status code</param>
        /// <param name="dateTime">The date and time the state was changed</param>
        /// <param name="userId">The user that has made the change</param>
        /// <param name="incidentNo">The incident number that the resource is currently attached to</param>
        /// <returns>true if the change was successfully made, otherwise false</returns>
        protected bool SetCurrentResourceStatus(ResourceStatus status, DateTime dateTime, int userId, int incidentNo)
        {
            //execute the change to the resource status
            if (database.SetCurrentResourceStatus(callSign: CallSign, code: status.Code, dateTime: dateTime, userId: userId))
            {
                //if the status code means that an incident log should be updated with the change, execute the appropriate method.
                switch (status.IncidentLogAction)
                {
                    case ResourceLogTime.Mobile:
                        database.SetToMobile(callSign: CallSign, dateTime: dateTime, incidentNo: incidentNo);
                        break;
                    case ResourceLogTime.OnScene:
                        database.SetToInAttendance(callSign: CallSign, dateTime: dateTime, incidentNo: incidentNo);
                        break;
                    case ResourceLogTime.Available:
                        database.SetToAvailable(callSign: CallSign, dateTime: dateTime, incidentNo: incidentNo);
                        break;
                    case ResourceLogTime.Finished:
                        database.SetToFinished(callSign: CallSign, dateTime: dateTime, incidentNo: incidentNo);
                        break;
                    //nothing needs to be done with 'alerted' as the alerted time is already entered into the incident_resource 
                    //table in MySqlIncidentDB.AssignResource().  It is better this way because MySqlIncientDB knows the trigger
                    case ResourceLogTime.Alerted: 
                    case ResourceLogTime.Ignore:
                    default:
                        break;
                }
                return true; //return true if the change was successful
            }
            else
                return false; //else false
        }

        #endregion
    }

}
