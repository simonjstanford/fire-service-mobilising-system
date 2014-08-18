using System;
namespace Prototype.Model.Resource_Sub_System
{
    /// <summary>
    /// Used in the ResourceStatus class, this enumeration details the effect the resource 
    /// status has on the incident the resource is currently attatched to (if any) - the 
    /// incident updates the appliance information section with the time that each resource 
    /// attached to the incident is changed to a status with each enumeration value.  This helps
    /// the user to see at a glance the state of each appliance that was assigned to the incident.
    /// </summary>
    public enum ResourceLogTime
    {
        Alerted = 0,
        Mobile = 1,
        OnScene = 2,
        Available = 3,
        Finished = 4,
        Ignore = 5,
    }
}
