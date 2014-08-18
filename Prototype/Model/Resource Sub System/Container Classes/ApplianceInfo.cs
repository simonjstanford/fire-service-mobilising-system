using Prototype.Model.Global_Container_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Model.Resource_Sub_System.Container_Classes
{
    /// <summary>
    /// A container class that stores information relating to a single appliance.  This object is created by the database when queried for
    /// appliance information and passed to an 'Appliance' object, which exposes the data.
    /// </summary>
    public class ApplianceInfo
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the appliance</param>
        /// <param name="mobileNo">The mobile phone number</param>
        /// <param name="baseLoc">The base location of the appliance</param>
        /// <param name="address">The current location of the appliance</param>
        /// <param name="status">The current appliance status</param>
        /// <param name="oic">The officer in charge (OiC) of the appliance</param>
        /// <param name="crew">The number of crew members riding the appliance</param>
        /// <param name="ba">The number of breathing apparatus (BA) available on the appliance</param>
        /// <param name="type">The type of appliance</param>
        /// <param name="assignedIncident">The incident that the resource is currently assigned to</param>
        public ApplianceInfo(string name, string mobileNo, Base baseLoc, Address address, ResourceStatus status, string oic, int crew, int ba, ApplianceType type, int assignedIncident)
        {
            Name = name;
            MobilePhoneNumber = mobileNo;
            Base = baseLoc;
            CurrentAddress = address;
            CurrentResourceStatus = status;
            OiC = oic;
            NumberOfCrew = crew;
            NumberOfBA = ba;
            Type = type;
            AssignedIncident = assignedIncident;
        }

        #region Properties

        /// <summary>
        /// The name of the resource
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Returns the mobile phone number assigned to the resource
        /// </summary>
        public string MobilePhoneNumber { get; set; }

        /// <summary>
        /// Returns the base location assigned to the resource
        /// </summary>
        public Base Base { get; set; }

        /// <summary>
        /// Returns the current location of the resource.
        /// See SetCurrentAddress() for the setter.
        /// </summary>
        public Address CurrentAddress { get; set; }

        /// <summary>
        /// Returns the current status of the resource.
        /// See SetCurrentResourceStatus() for the setter
        /// </summary>
        public ResourceStatus CurrentResourceStatus { get; set; }

        /// <summary>
        /// Gets/sets the appliance officer in charge (OiC)
        /// </summary>
        public string OiC { get; set; }

        /// <summary>
        /// Gets/sets the number of crew on the appliance
        /// </summary>
        public int NumberOfCrew { get; set; }

        /// <summary>
        /// Gets/sets the number of breathing apparatus wearers (BA) on the appliance
        /// </summary>
        public int NumberOfBA { get; set; }

        /// <summary>
        /// Gets/sets the appliance type associated with this appliance.
        /// Appliance types determine what incidents an appliance is mobilised to - 
        /// if an incident requires 2 appliances of type 'Aerial', then this property is checked for all resources.  
        /// If it contains a type called 'Aerial' then the appliance is considered for mobilisation.
        /// </summary>
        public ApplianceType Type { get; set; }

        /// <summary>
        /// Gets/sets the incident number that the resource is currently assigned to
        /// </summary>
        public int AssignedIncident { get; set; }

        #endregion
    }
}
