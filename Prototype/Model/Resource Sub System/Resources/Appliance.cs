using Prototype.Model.Global_Classes;
using Prototype.Model.Global_Container_Classes;
using Prototype.Model.Resource_Sub_System.Container_Classes;
using System;
using System.ComponentModel;
using System.Linq;

namespace Prototype.Model.Resource_Sub_System
{
    /// <summary>
    /// A sub class that represents a fire service appliance
    /// </summary>
    public class Appliance : Resource
    {
        #region Fields

        //the database connection object for appliances.  Referenced through an inteface to allow for polymorphism
        private IApplianceDB applianceDatabase;

        //A container object that stores detailed appliance information
        private ApplianceInfo info;

        //A container object that stores a subset of appliance information. Used for efficiency to display a small amount of information.
        //The Appliance object needs a reference of it to notify when values are updated.
        private ApplianceDataView viewData;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="callsign">The unique identification code of the resource</param>
        /// <param name="info">A container object that stores detailed appliance information</param>
        public Appliance(string callsign, ApplianceInfo info)
            : base(callsign, info.Name, info.MobilePhoneNumber, info.Base, info.CurrentAddress, info.CurrentResourceStatus, info.AssignedIncident)
        {
            this.applianceDatabase = Tools.ApplianceDB;
            this.info = info;
            //find the related incidentDataView object
            this.viewData = Tools.Appliances.Single(x => x.CallSign == callsign);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the appliance officer in charge (OiC)
        /// </summary>
        public string OiC
        {
            get { return info.OiC; }
            set 
            { 
                applianceDatabase.SetOiC(callSign: CallSign, oic: value);
                info.OiC = value;
                viewData.OiC = value;
                viewData.OnPropertyChanged("OiC");
            }
        }

        /// <summary>
        /// Gets/sets the number of crew on the appliance
        /// </summary>
        public int NumberOfCrew
        {
            get { return info.NumberOfCrew; }
            set 
            { 
                applianceDatabase.SetNumberOfCrew(callSign: CallSign, numberOfCrew: value);
                info.NumberOfCrew = value;
                viewData.Crew = value;
                viewData.OnPropertyChanged("NumberOfCrew");
            }
        }

        /// <summary>
        /// Gets/sets the number of breathing apparatus wearers (BA) on the appliance
        /// </summary>
        public int NumberOfBA
        {
            get { return info.NumberOfBA; }
            set 
            { 
                applianceDatabase.SetNumberOfBA(callSign: CallSign, ba: value);
                info.NumberOfBA = value;
                viewData.BA = value;
                viewData.OnPropertyChanged("BA");
            }
        }

        /// <summary>
        /// Gets/sets the appliance type associated with this appliance.
        /// Appliance types determine what incidents an appliance is mobilised to - 
        /// if an incident requires 2 appliances of type 'Aerial', then this property is checked for all resources.  
        /// If it contains a type called 'Aerial' then the appliance is considered for mobilisation.
        /// </summary>
        public ApplianceType Type
        {
            get { return info.Type; }
            set 
            { 
                applianceDatabase.SetType(callSign: CallSign, type: value);
                info.Type = value;
                viewData.Type = value.Name;
                viewData.OnPropertyChanged("Type");
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Overrides ToString() to just print the appliance callsign
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return CallSign;
        }

        /// <summary>
        /// Overrides Resource.Name to update the ApplianceDataView object and notify it that a change has taken place
        /// This ensures that any associated Data Grid is also updated.
        /// </summary>
        public override string Name
        {
            get { return base.Name; }
            set
            {
                base.Name = value;
                viewData.Name = value;
                viewData.OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Overrides Resource.CurrentResourceStatus to update the ApplianceDataView object and notify it that a change has taken place
        /// This ensures that any associated Data Grid is also updated.
        /// </summary
        public override ResourceStatus CurrentResourceStatus
        {
            get
            {
                return base.CurrentResourceStatus;
            }
            set
            {
                base.CurrentResourceStatus = value;
                viewData.Status = value.Description;
                viewData.OnPropertyChanged("CurrentResourceStatus");

                if (value.IncidentLogAction == ResourceLogTime.Finished || value.IncidentLogAction == ResourceLogTime.Alerted)
                {
                    info = Tools.ApplianceDB.GetApplianceInfo(CallSign);
                    viewData.Incident = info.AssignedIncident;
                    viewData.OnPropertyChanged("Incident");
                }
            }
        }

        /// <summary>
        /// Overrides Resource.CurrentAddress to update the ApplianceDataView object and notify it that a change has taken place
        /// This ensures that any associated Data Grid is also updated.
        /// </summary
        public override Address CurrentAddress
        {
            get
            {
                return base.CurrentAddress;
            }
            set
            {
                base.CurrentAddress = value;
                viewData.Address = value.ToString();
                viewData.OnPropertyChanged("CurrentAddress");
            }
        }

        #endregion
    }
}
