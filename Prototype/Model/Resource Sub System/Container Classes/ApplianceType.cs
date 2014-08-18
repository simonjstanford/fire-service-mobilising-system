using System;
namespace Prototype.Model.Resource_Sub_System.Container_Classes
{
    /// <summary>
    /// A container object that stores appliance type information.  An appliance is assigned one type, and this dictates 
    /// what incidents it will be sent to. For example - if a bin fire needs 1 appliance of type 'Pump', the system will 
    /// search through all the appliances with a matching appliance type name.  Only those of appliance type 'Pump' will 
    /// then be considered for the emergency.
    /// </summary>
    public class ApplianceType
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the appliance type</param>
        /// <param name="description">A description of the appliance type</param>
        public ApplianceType(string name, string description)
        {
            Name = name;
            Description = description;
        }

        //the name of the appliance type
        public string Name { get; private set; }

        //a description of the appliance type
        public string Description { get; private set; }

        /// <summary>
        /// Overrides ToString() to display the appliance type name
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }

}
