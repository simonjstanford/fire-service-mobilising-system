using System;
using Prototype.Model.Global_Container_Classes;

namespace Prototype.Model.Resource_Sub_System.Container_Classes
{
    /// <summary>
    /// A container class that stores location information for a resource
    /// </summary>
    public class ResourceLocation 
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="address">The address where the resource has moved to</param>
        /// <param name="dateTime">The date and time that the resource moved</param>
        /// <param name="user">The user that made the change</param>
        public ResourceLocation(Address address, DateTime dateTime, string user)
        {
            Address = address;
            DateTime = dateTime;
            User = user;
        }

        /// <summary>
        /// The address where the resource has moved to
        /// </summary>
        public Address Address { get; private set; }

        /// <summary>
        /// The date and time that the resource moved
        /// </summary>
        public DateTime DateTime { get; private set; }

        /// <summary>
        /// The user that made the change
        /// </summary>
        public string User { get; private set; }
    }
}
