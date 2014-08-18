using Prototype.Model.Gazetteer_Sub_System;
using Prototype.Model.Global_Container_Classes;
using System;
namespace Prototype.Model.Incident_Sub_System.Container_Classes
{
    /// <summary>
    /// Stores additional address information for a property, i.e. urgent messages and other related addresses.
    /// </summary>
    public class AdditionalAddressInfo
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">The additional info ID taken from the database</param>
        /// <param name="addresses">An array of related addresses.  If there are addresses stored in this property, then these should also be mobilised.</param>
        /// <param name="urgentMessages">An array of urgent messages for a property.</param>
        public AdditionalAddressInfo(int id, Address[] addresses, UrgentMessage urgentMessages)
        {
            Id = id;
            Addresses = addresses;
            UrgentMessages = urgentMessages;
        }

        /// <summary>
        /// The additional info ID taken from the database
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// An array of related addresses.  If there are addresses stored in this property, then these should also be mobilised.
        /// </summary>
        public Address[] Addresses { get; private set; }

        /// <summary>
        /// An array of urgent messages for a property.
        /// </summary>
        public UrgentMessage UrgentMessages { get; private set; }
    }
}
