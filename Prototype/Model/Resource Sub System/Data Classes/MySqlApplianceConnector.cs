using MySql.Data.MySqlClient;
using Prototype.Model.Global_Classes;
using Prototype.Model.Global_Container_Classes;
using Prototype.Model.Resource_Sub_System.Container_Classes;
using System;
using System.Windows;
namespace Prototype.Model.Resource_Sub_System
{
    /// <summary>
    /// Provides functionality to retrieve information from a MySQL database for a specific appliance.
    /// </summary>
    public class MySqlApplianceConnector : BaseMySqlConnector, IApplianceDB
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">The name of the database</param>
        /// <param name="serverLocation">The server IP Address</param>
        /// <param name="sqlUsername">The database username</param>
        /// <param name="sqlPassword">The database password</param>
        public MySqlApplianceConnector(string database, string serverLocation, string sqlUsername, string sqlPassword)
            : base(database, serverLocation, sqlUsername, sqlPassword)
        {
            //all parameters are sent to the base class
        }

        #region Get Methods

        /// <summary>
        /// Retrieves all appliance information from the database.
        /// </summary>
        /// <param name="callSign">The unique appliance call sign to retrieve the information for.</param>
        /// <returns>An container class that stores all the information</returns>
        public ApplianceInfo GetApplianceInfo(string callSign)
        {
            string statement = "SELECT resource.Name, resource.MobilePhoneNumber, " + Environment.NewLine +
                               "base.id, base.name, base.OfficePhoneNumber," + Environment.NewLine +
                               "base_address.id, base_address.building, base_address.number, base_address.street, base_address.town, base_address.postcode, base_address.county, base_address.longitude, base_address.latitude," + Environment.NewLine +
                               "current_address.id, current_address.building, current_address.number, current_address.street, current_address.town, current_address.postcode, current_address.county, current_address.longitude, current_address.latitude," + Environment.NewLine +
                               "status.code, status.Description, status.IsAvailable, status.IsMobile, status.ResourceLogEnumeration, resource_status.DateTime, resource_status.OperatorId," + Environment.NewLine +
                               "appliance.OfficerInCharge, appliance.CrewNumber, appliance.BaNumber," + Environment.NewLine +
                               "appliancetype.Name, appliancetype.Description, resource.AttachedIncidentNumber" + Environment.NewLine +
                               "FROM resource" + Environment.NewLine +
                               "INNER JOIN base ON resource.BaseLocationId = base.Id" + Environment.NewLine +
                               "INNER JOIN address AS base_address ON base.AddressId = base_address.Id" + Environment.NewLine +
                               "INNER JOIN Address_Resource ON Address_Resource.ResourceCallSign = resource.CallSign" + Environment.NewLine +
                               "INNER JOIN address AS current_address ON address_resource.AddressId = current_address.Id" + Environment.NewLine +
                               "INNER JOIN resource_status ON resource_status.ResourceCallSign = resource.CallSign" + Environment.NewLine +
                               "INNER JOIN status ON resource_status.StatusCode = status.Code" + Environment.NewLine +
                               "INNER JOIN appliance ON resource.CallSign = appliance.CallSign" + Environment.NewLine +
                               "INNER JOIN appliancetype ON appliance.ApplianceTypeName = appliancetype.Name" + Environment.NewLine +
                               "WHERE Address_Resource.DateTime = (SELECT MAX(DateTime) FROM Address_Resource WHERE ResourceCallSign = resource.CallSign)" + Environment.NewLine +
                               "AND resource_status.DateTime = (SELECT MAX(DateTime) FROM resource_status WHERE ResourceCallSign = resource.CallSign)" + Environment.NewLine +
                               "AND resource.CallSign = @callsign;";


            //object that will execute the query
            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@callsign", callSign);

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

                //read the data sent back from MySQL server and create the Appliance objects
                while (myReader.Read())
                {
                    //read all the data returned from the database - each information area is split into a different region below

                    string resourceName = myReader.GetString(0);
                    string mobileNo = myReader.GetString(1);

                    #region Base Address Data

                    int baseId = myReader.GetInt32(2);
                    string baseName = myReader.GetString(3);
                    string officeNo = myReader.GetString(4);
                    int baseAddressId = myReader.GetInt32(5);

                    string baseBuilding = string.Empty;
                    if (!myReader.IsDBNull(6)) baseBuilding = myReader.GetString(6);

                    string baseNumber = string.Empty;
                    if (!myReader.IsDBNull(7)) baseNumber = myReader.GetString(7);

                    string baseStreet = string.Empty;
                    if (!myReader.IsDBNull(8)) baseStreet = myReader.GetString(8);

                    string baseTown = string.Empty;
                    if (!myReader.IsDBNull(9)) baseTown = myReader.GetString(9);

                    string basePostcode = string.Empty;
                    if (!myReader.IsDBNull(10)) basePostcode = myReader.GetString(10);

                    string baseCounty = string.Empty;
                    if (!myReader.IsDBNull(11)) baseCounty = myReader.GetString(11);

                    double baseLong = myReader.GetDouble(12);
                    double baseLat = myReader.GetDouble(13);

                    Address baseAddress = new Address(baseAddressId, baseBuilding, baseNumber, baseStreet, baseTown, basePostcode, baseCounty, baseLong, baseLat);

                    Base baseLocation = new Base(baseId, officeNo, baseAddress, baseName);

                    #endregion


                    #region Current Address Data

                    int currentId = myReader.GetInt32(14);

                    string currentBuilding = string.Empty;
                    if (!myReader.IsDBNull(15)) currentBuilding = myReader.GetString(15);

                    string currentNumber = string.Empty;
                    if (!myReader.IsDBNull(16)) currentNumber = myReader.GetString(16);

                    string currentStreet = string.Empty;
                    if (!myReader.IsDBNull(17)) currentStreet = myReader.GetString(17);

                    string currentTown = string.Empty;
                    if (!myReader.IsDBNull(18)) currentTown = myReader.GetString(18);

                    string currentPostcode = string.Empty;
                    if (!myReader.IsDBNull(19)) currentPostcode = myReader.GetString(19);

                    string currentCounty = string.Empty;
                    if (!myReader.IsDBNull(20)) currentCounty = myReader.GetString(20);

                    double currentLong = myReader.GetDouble(21);
                    double currentLat = myReader.GetDouble(22);

                    Address currentAddress = new Address(currentId, currentBuilding, currentNumber, currentStreet, currentTown, currentPostcode, currentCounty, currentLong, currentLat);

                    #endregion


                    #region Status Code Data

                    int statusCode = -1;
                    if (!myReader.IsDBNull(23)) statusCode = myReader.GetInt32(23);

                    string statusDescription = string.Empty;
                    if (!myReader.IsDBNull(24)) statusDescription = myReader.GetString(24);

                    bool isAvail = false;
                    if (!myReader.IsDBNull(25)) isAvail = myReader.GetBoolean(25);

                    bool isMobile = false;
                    if (!myReader.IsDBNull(26)) isMobile = myReader.GetBoolean(26);

                    int resourceLogEnumeration = -1;
                    if (!myReader.IsDBNull(27)) resourceLogEnumeration = myReader.GetInt32(27);

                    ResourceLogTime incidentLogAction = ResourceLogTime.Ignore;
                    if (Enum.IsDefined(typeof(ResourceLogTime), resourceLogEnumeration))
                        incidentLogAction = (ResourceLogTime)resourceLogEnumeration;

                    ResourceStatus status = new ResourceStatus(statusCode, statusDescription, isAvail, isMobile, incidentLogAction);

                    #endregion


                    #region Appliance Data

                    string oic = string.Empty;
                    if (!myReader.IsDBNull(30)) oic = myReader.GetString(30);

                    int crew = -1;
                    if (!myReader.IsDBNull(31)) crew = myReader.GetInt32(31);

                    int ba = -1;
                    if (!myReader.IsDBNull(32)) ba = myReader.GetInt32(32);

                    string applianceTypeName = string.Empty;
                    if (!myReader.IsDBNull(33)) applianceTypeName = myReader.GetString(33);

                    string applianceTypeDescription = string.Empty;
                    if (!myReader.IsDBNull(34)) applianceTypeDescription = myReader.GetString(34);

                    int assignedIncident = -1;
                    if (!myReader.IsDBNull(35)) assignedIncident = myReader.GetInt32(35);

                    ApplianceType type = new ApplianceType(applianceTypeName, applianceTypeDescription);

                    #endregion

                    return new ApplianceInfo(resourceName, mobileNo, baseLocation, currentAddress, status, oic, crew, ba, type, assignedIncident);
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

            return null;
        }

        #endregion

        #region Set Methods

        /// <summary>
        /// Sets the appliance officer in charge (OiC) of a specific callsign
        /// </summary>
        /// <param name="callSign">The callsign to change</param>
        /// <param name="oic">The new OiC for the provided callsign</param>
        /// <returns>True if the change was succesful, otherwise false</returns>
        public bool SetOiC(string callSign, string oic)
        {
            //build the SQL command
            string statement = "UPDATE Appliance " +
                               "SET OfficerInCharge = @oic " +
                               "WHERE CallSign = @callsign;";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@callsign", callSign);
            command.Parameters.AddWithValue("@oic", oic);

            return executeNonQuery(command);
        }

        /// <summary>
        /// Sets the number of crew on the appliance for a specific callsign
        /// </summary>
        /// <param name="callSign">The callsign to change</param>
        /// <param name="numberOfCrew">The new number of crew for the provided callsign</param>
        /// <returns>True if the change was succesful, otherwise false</returns>
        public bool SetNumberOfCrew(string callSign, int numberOfCrew)
        {
            //build the SQL command
            string statement = "UPDATE Appliance " +
                               "SET CrewNumber = @newNumber " +
                               "WHERE CallSign = @callsign;";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@callsign", callSign);
            command.Parameters.AddWithValue("@newNumber", numberOfCrew);

            return executeNonQuery(command);
        }

        /// <summary>
        /// Sets the number of breathing apparatus wearers (BA) on the appliance for a specific callsign
        /// </summary>
        /// <param name="callSign">The callsign to change</param>
        /// <param name="ba">The new number of BA for the provided callsign</param>
        /// <returns>True if the change was succesful, otherwise false</returns>
        public bool SetNumberOfBA(string callSign, int ba)
        {
            //build the SQL command
            string statement = "UPDATE Appliance " +
                               "SET BaNumber = @newNumber " +
                               "WHERE CallSign = @callsign;";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@callsign", callSign);
            command.Parameters.AddWithValue("@newNumber", ba);

            return executeNonQuery(command);
        }

        /// <summary>
        /// Sets the appliance type associated with this appliance.
        /// </summary>
        /// <param name="callSign">The callsign to change</param>
        /// <param name="type">The new appliance type</param>
        /// <returns>True if the change was successful, otherwise false.</returns>
        /// <remarks>
        /// Appliance types determine what incidents an appliance is mobilised to - 
        /// if an incident requires 2 appliances of type 'Aerial', then this property is checked for all resources.  
        /// If it contains a type called 'Aerial' then the appliance is considered for mobilisation.
        /// </remarks>
        public bool SetType(string callSign, ApplianceType type)
        {
            //build the SQL command
            string statement = "UPDATE Appliance " +
                               "SET ApplianceTypeName = @type " +
                               "WHERE CallSign = @callsign;";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@callsign", callSign);
            command.Parameters.AddWithValue("@type", type);

            return executeNonQuery(command);
        }

        #endregion
    }
}
