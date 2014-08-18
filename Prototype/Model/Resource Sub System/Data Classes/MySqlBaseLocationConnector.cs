using MySql.Data.MySqlClient;
using Prototype.Model.Global_Classes;
using Prototype.Model.Global_Container_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Prototype.Model.Resource_Sub_System.Data_Classes
{
    /// <summary>
    /// Provides functionality to retrieve information from a MySQL database for a specific base location.
    /// </summary>
    public class MySqlBaseLocationConnector : BaseMySqlConnector, IBaseDB
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">The name of the database</param>
        /// <param name="serverLocation">The server IP Address</param>
        /// <param name="sqlUsername">The database username</param>
        /// <param name="sqlPassword">The database password</param>
        public MySqlBaseLocationConnector(string database, string serverLocation, string sqlUsername, string sqlPassword)
            : base(database: database, serverLocation: serverLocation, sqlUsername: sqlUsername, sqlPassword: sqlPassword)
        { }

        #region Get Methods

        /// <summary>
        /// Retrieves all information relating to a specific Fire Service base loction.
        /// </summary>
        /// <param name="baseId">The unique ID of the base to retrieve information for.</param>
        /// <param name="officeNo">The base office phone number</param>
        /// <param name="baseName">The base name</param>
        /// <param name="baseAddress">The base address</param>
        public void GetBaseInfo(int baseId, out string officeNo, out string baseName, out Address baseAddress)
        {
            string statement = "SELECT * FROM base" + Environment.NewLine +
                               "INNER JOIN address ON base.AddressId = address.Id" + Environment.NewLine +
                               "WHERE base.Id = @baseId;";

            //assign default values
            officeNo = string.Empty;
            baseName = string.Empty;
            baseAddress = null;

            //object that will execute the query
            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@baseId", baseId);

            //execute
            MySqlDataReader myReader = null; //reads the database data
            try
            {
                // open the connection only if it is currently closed
                if (command.Connection.State == System.Data.ConnectionState.Closed)
                    command.Connection.Open();

                //close the reader if it currently exists
                if (myReader != null && !myReader.IsClosed)
                    myReader.Close();

                myReader = command.ExecuteReader(); //execute the select statement

                //read the data sent back from MySQL server
                while (myReader.Read())
                {

                    if (!myReader.IsDBNull(2)) officeNo = myReader.GetString(2);

                    baseName = myReader.GetString(3);

                    #region Base Address Data

                    int baseAddressId = myReader.GetInt32(4);

                    string baseBuilding = string.Empty;
                    if (!myReader.IsDBNull(5)) baseBuilding = myReader.GetString(5);

                    string baseNumber = string.Empty;
                    if (!myReader.IsDBNull(6)) baseNumber = myReader.GetString(6);

                    string baseStreet = string.Empty;
                    if (!myReader.IsDBNull(7)) baseStreet = myReader.GetString(7);

                    string baseTown = string.Empty;
                    if (!myReader.IsDBNull(8)) baseTown = myReader.GetString(8);

                    string basePostcode = string.Empty;
                    if (!myReader.IsDBNull(9)) basePostcode = myReader.GetString(9);

                    string baseCounty = string.Empty;
                    if (!myReader.IsDBNull(10)) baseCounty = myReader.GetString(10);

                    double baseLong = myReader.GetDouble(11);
                    double baseLat = myReader.GetDouble(12);

                    baseAddress = new Address(baseAddressId, baseBuilding, baseNumber, baseStreet, baseTown, basePostcode, baseCounty, baseLong, baseLat);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                MessageBox.Show(statement);
            }
            finally
            {
                //close the connections
                if (myReader != null)
                    myReader.Close();
                command.Connection.Close();
            }
        }

        #endregion

        #region Set Methods

        /// <summary>
        /// Sets the office phone number for the specified base ID
        /// </summary>
        /// <param name="baseId">The base to change</param>
        /// <param name="newNumber">The new phone number</param>
        /// <returns>True if the change was successful, false otherwise</returns>
        public bool SetOfficePhoneNumber(int baseId, string newNumber)
        {
            //build the SQL command
            string statement = "UPDATE Base" + Environment.NewLine +
                               "SET OfficePhoneNumber = @newNumber" + Environment.NewLine +
                               "WHERE Id = @baseId;";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@baseId", baseId);
            command.Parameters.AddWithValue("@newNumber", newNumber);

            return executeNonQuery(command);
        }

        #endregion

        /// <summary>
        /// Sets the address for the specified base ID
        /// </summary>
        /// <param name="baseId">The base to change</param>
        /// <param name="newAddress">The new address</param>
        /// <returns>True if the change was successful, false otherwise</returns>
        public bool SetAddress(int baseId, Address newAddress)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the name for the specified base ID
        /// </summary>
        /// <param name="baseId">The base to change</param>
        /// <param name="newName">The new name</param>
        /// <returns>True if the change was successful, false otherwise</returns>
        public bool SetName(int baseId, string newName)
        {
            throw new NotImplementedException();
        }
    }
}
