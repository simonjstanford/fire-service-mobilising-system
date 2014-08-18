using System;
using Prototype.Model;
using Prototype.Model.Global_Container_Classes;
using Prototype.Model.Resource_Sub_System.Data_Classes;
using Prototype.Model.Global_Classes;

namespace Prototype.Model.Resource_Sub_System.Container_Classes
{
    /// <summary>
    /// A container class that stores appliance base information.
    /// </summary>
    public class Base : IEquatable<Base>
    {
        #region Fields

        private IBaseDB database; //the database connection to retrieve base loction info from
        private string officeNo; //backing fields for OfficePhoneNumber
        private Address address; //backing fields for Address
        private string name; //backing fields for Name

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor - used when you don't already have base information
        /// </summary>
        /// <param name="id">The unique ID for the base</param>
        /// <param name="database">An object that provides a method to update/retrieve information from the database</param>
        public Base(int id)
        {
            //assign fields
            ID = id;
            this.database = Tools.BaseLoctionDB;

            //retrieve info from the database
            database.GetBaseInfo(id, out officeNo, out name, out address);
        }

        /// <summary>
        /// Constructor - used when you already have base information
        /// </summary>
        /// <param name="id"></param>
        /// <param name="officeNo"></param>
        /// <param name="address"></param>
        /// <param name="name"></param>
        public Base(int id, string officeNo, Address address, string name)
        {
            //assign fields
            ID = id;
            this.officeNo = officeNo;
            this.address = address;
            this.name = name;

            //create the database object
            string dbName = Properties.Settings.Default.DatabaseName;
            string ip = Properties.Settings.Default.ServerIP;
            string username = Properties.Settings.Default.Username;
            string password = Properties.Settings.Default.Password;
            this.database = new MySqlBaseLocationConnector(dbName, ip, username, password);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The unique ID for this base location.  Used to retrieve further details.
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// The office phone number for the base location.
        /// </summary>
        public string OfficePhoneNumber
        {
            get { return officeNo; }
            set
            {
                database.SetOfficePhoneNumber(ID, value);
                officeNo = value;
            }
        }

        /// <summary>
        /// The address of the base location.
        /// </summary>
        public Address Address
        {
            get { return address; }
            set
            {
                database.SetAddress(ID, value);
                address = value;
            }
        }

        /// <summary>
        /// The name of the base location
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                database.SetName(ID, value);
                name = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Override of ToString() to return the name of the base
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Implementation of IEquatable - tests if two Base objects are equal based on their ID
        /// </summary>
        /// <param name="other">The other Base object to test</param>
        /// <returns>True if the objects are matching, false otherwise</returns>
        public bool Equals(Base other)
        {
            if (this.ID == other.ID)
                return true;
            else
                return false;
        }

        #endregion
    }

}
