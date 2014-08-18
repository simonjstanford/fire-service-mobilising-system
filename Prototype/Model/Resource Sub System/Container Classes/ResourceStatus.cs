using System;

namespace Prototype.Model.Resource_Sub_System.Container_Classes
{
    /// <summary>
    /// A container class that stores resource status information
    /// </summary>
    public class ResourceStatus
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="code">The unique identification code for the resource status</param>
        /// <param name="description">A description of the resource status</param>
        /// <param name="isAvailable">A boolean value used to determine if the resource status 
        /// means that the resource is currently available to attend emergencies.</param>
        /// <param name="isMobile">A boolean value used to determine if the resource status
        /// means that the resource is at a base station (false) or not (true).
        /// Used to determine the available mobilising methods for a resource</param>
        /// <param name="incidentLogAction">An enumeration that details the effect the resource status has on the incident the 
        /// resource is currently attatched to (if any) - the incident updates the appliance 
        /// information section with the time that each resource attached to the incident is 
        /// changed to a status with each enumeration value.  This helps the user to see at a 
        /// glance the state of each appliance that was assigned to the incident.</param>
        public ResourceStatus(int code, string description, bool isAvailable, bool isMobile, ResourceLogTime incidentLogAction)
        {
            Code = code;
            Description = description;
            IsAvailable = isAvailable;
            IsMobile = isMobile;
            IncidentLogAction = incidentLogAction;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification code for the resource status
        /// </summary>
        public int Code { get; private set; }

        /// <summary>
        /// A description of the resource status
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// A boolean value used to determine if the resource status 
        /// means that the resource is currently available to attend emergencies.
        /// </summary>
        public bool IsAvailable { get; private set; }

        /// <summary>
        /// A boolean value used to determine if the resource status
        /// means that the resource is at a base station (false) or not (true).
        /// Used to determine the available mobilising methods for a resource
        /// </summary>
        public bool IsMobile { get; private set; }

        /// <summary>
        /// An enumeration that details the effect the resource status has on the incident the 
        /// resource is currently attatched to (if any) - the incident updates the appliance 
        /// information section with the time that each resource attached to the incident is 
        /// changed to a status with each enumeration value.  This helps the user to see at a 
        /// glance the state of each appliance that was assigned to the incident.
        /// </summary>
        public ResourceLogTime IncidentLogAction { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Override of ToString() to display the status code and a brief description.
        /// </summary>
        /// <returns>The formatted string</returns>
        public override string ToString()
        {
            return Code.ToString() + ": " + Description;
        }

        #endregion
    }

}
