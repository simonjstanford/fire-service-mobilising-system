using Prototype.Model.Gazetteer_Sub_System;
using System;
using System.Windows;

namespace Prototype.Model.Global_Container_Classes
{
    /// <summary>
    /// A container class that stores all relevant information related to a specific address in the world.
    /// </summary>
    public class Address : IEquatable<Address>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="building">The building name</param>
        /// <param name="number">The building number</param>
        /// <param name="street">The street component of the address</param>
        /// <param name="town">The town component of the address</param>
        /// <param name="postcode">The postcode component of the address</param>
        /// <param name="county">The county component of the address</param>
        /// <param name="longitude">The longitude co-ordinate of the address</param>
        /// <param name="latitude">The latidude co-ordinate of the address</param>
        /// <param name="classification">The classification of the address.  This is used to change the Fire Service attendance for the same incident type to different addresses.</param>
        /// <param name="additionalAddressInfo">Any additional address information, e.g. extra locations that resources should be mobilised to and any urgent messages for the address.</param>
        public Address(int id, string building, string number, string street, string town, string postcode, string county, double longitude, double latitude)
        {
            //assign values to address components
            ID = id;
            Building = building;
            Number = number;
            Street = street;
            Town = town;
            Postcode = postcode;
            County = county;
            Longitude = longitude;
            Latitude = latitude;

            //build the NumberStreet property.  Using a getter method that builds this value on request does not work with WPF binding
            if (Number != null)
                NumberStreet = number + " " + street;
            else
                NumberStreet = street;
        }

        #region Properties

        //Properties that expose specific components of the address
        public int ID { get; private set; }
        public string Building { get; private set; }
        public string Number { get; private set; }
        public string Street { get; private set; }
        public string Town { get; private set; }
        public string Postcode { get; private set; }
        public string County { get; private set; }

        /// <summary>
        /// An amalgamation of the number and street properties
        /// </summary>
        public string NumberStreet { get; private set; }

        //Properties for the position of the address in the world
        public double Longitude { get; private set; }
        public double Latitude { get; private set; }

        /// <summary>
        /// The classification of the address.  This is used to change the Fire Service attendance for the same incident type to different addresses.
        /// </summary>
        public string ClassificationName { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Overrides ToString() to display the full address.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string address = "";

            if (!string.IsNullOrWhiteSpace(Building))
                address += Building + ", ";

            if (!string.IsNullOrWhiteSpace(Number))
                address += Number + " ";

            if (!string.IsNullOrWhiteSpace(Street))
                address += Street + ", ";

            if (!string.IsNullOrWhiteSpace(Town))
                address += Town;

            if (!string.IsNullOrWhiteSpace(Postcode))
                address += ", " + Postcode;

            return address;
        }

        /// <summary>
        /// Implementation of IEquatable - tests if two Address objects are equal based on their longitude and latitude
        /// </summary>
        /// <param name="other">The other Address object to test</param>
        /// <returns>True if the objects are matching, false otherwise</returns>
        public bool Equals(Address other)
        {
            if (this.Latitude == other.Latitude && this.Longitude == other.Longitude)
                return true;
            else
                return false;
        }

        #endregion
    }
}
