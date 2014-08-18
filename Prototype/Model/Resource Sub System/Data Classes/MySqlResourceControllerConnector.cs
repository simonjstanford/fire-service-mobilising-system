using System;
using Prototype.Model.Global_Container_Classes;
using Prototype.Model.Resource_Sub_System.Container_Classes;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Windows;
using Prototype.Model.Resource_Sub_System.Data_Classes;
using Prototype.Model.Global_Classes;

namespace Prototype.Model.Resource_Sub_System
{
    /// <summary>
    /// Manages a collection of resources.  Provides methods for retrieving specific resources and general information.
    /// </summary>
    public class MySqlResourceControllerConnector : BaseMySqlConnector, IResourceControllerDB
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">The name of the database</param>
        /// <param name="serverLocation">The server IP Address</param>
        /// <param name="sqlUsername">The database username</param>
        /// <param name="sqlPassword">The database password</param>
        /// <param name="resourceDatabase">A reference to the object that implements IResourceDB that will be used by all resources to access the database.</param>
        /// <param name="applianceDatabase">A reference to the  object that implements IApplianceDB that will be used by all appliances to access the database.</param>
        /// <param name="officerDatabase">A reference to the  object that implements IOfficerDB that will be used by all appliances to access the database.</param>
        /// <param name="baseDatabase">A reference to the  object that implements IBaseDB that will be used to access the base loction information.</param>
        public MySqlResourceControllerConnector(string database, string serverLocation, string sqlUsername, string sqlPassword)
            : base(database: database, serverLocation: serverLocation, sqlUsername: sqlUsername, sqlPassword: sqlPassword)
        {
        }

        #region Get Appliance Data

        /// <summary>
        /// Retrieves a list of all appliances.
        /// </summary>
        /// <returns>A list of objects, each representing a single Appliance.  For efficiency, this is only a subset of information - use the 'Appliance' object for greater detail</returns>
        public ApplianceDataView[] GetAppliances(bool onlyAttached)
        {
            string statement = string.Empty;

            if (!onlyAttached)
                statement = "SELECT resource.CallSign, resource.Name, appliance.OfficerInCharge, appliance.CrewNumber, appliance.BaNumber, appliance.ApplianceTypeName," + Environment.NewLine +
                            "address.building, address.number, address.street, address.town, address.postcode, status.Description, resource.AttachedIncidentNumber" + Environment.NewLine +
                            "FROM resource, appliance, status, resource_status, address_resource, address" + Environment.NewLine +
                            "WHERE resource.CallSign = appliance.CallSign" + Environment.NewLine +
                            "AND resource.CallSign = resource_status.ResourceCallSign" + Environment.NewLine +
                            "AND resource_status.StatusCode = status.Code" + Environment.NewLine +
                            "AND resource_status.DateTime =  (SELECT MAX(resource_status.DateTime) FROM resource_status WHERE resource_status.ResourceCallSign = resource.CallSign)" + Environment.NewLine +
                            "AND address_resource.AddressId = address.Id" + Environment.NewLine +
                            "AND address_resource.ResourceCallSign = resource.CallSign" + Environment.NewLine +
                            "AND Address_Resource.DateTime = (SELECT MAX(Address_Resource.DateTime) FROM Address_Resource WHERE Address_Resource.ResourceCallSign = resource.CallSign)" + Environment.NewLine +
                            "ORDER BY resource.CallSign;";
            else
                statement = "SELECT resource.CallSign, resource.Name, appliance.OfficerInCharge, appliance.CrewNumber, appliance.BaNumber, appliance.ApplianceTypeName," + Environment.NewLine +
                            "address.building, address.number, address.street, address.town, address.postcode, status.Description, resource.AttachedIncidentNumber" + Environment.NewLine +
                            "FROM resource, appliance, status, resource_status, address_resource, address" + Environment.NewLine +
                            "WHERE resource.CallSign = appliance.CallSign" + Environment.NewLine +
                            "AND resource.CallSign = resource_status.ResourceCallSign" + Environment.NewLine +
                            "AND resource_status.StatusCode = status.Code" + Environment.NewLine +
                            "AND resource_status.DateTime =  (SELECT MAX(resource_status.DateTime) FROM resource_status WHERE resource_status.ResourceCallSign = resource.CallSign)" + Environment.NewLine +
                            "AND address_resource.AddressId = address.Id" + Environment.NewLine +
                            "AND address_resource.ResourceCallSign = resource.CallSign" + Environment.NewLine +
                            "AND Address_Resource.DateTime = (SELECT MAX(Address_Resource.DateTime) FROM Address_Resource WHERE Address_Resource.ResourceCallSign = resource.CallSign)" + Environment.NewLine +
                            "AND resource.AttachedIncidentNumber IS NOT NULL" + Environment.NewLine +
                            "ORDER BY resource.CallSign;";

            MySqlCommand command = new MySqlCommand(statement, connection);


            //the list of Appliance objects that will be returned
            List<ApplianceDataView> appliances = new List<ApplianceDataView>();

            //execute the query
            MySqlDataReader myReader = null;
            try
            {
                // open the connection only if it is currently closed
                if (command.Connection.State == System.Data.ConnectionState.Closed)
                    command.Connection.Open();

                //close the reader if it currently exists
                if (myReader != null && !myReader.IsClosed)
                    myReader.Close();

                myReader = command.ExecuteReader();

                //read the data sent back from MySQL server and create the ApplianceDataView objects
                while (myReader.Read())
                {
                    string callSign = myReader.GetString(0);
                    string name = myReader.GetString(1);
                    string oic = myReader.GetString(2);
                    int crew = myReader.GetInt32(3);
                    int ba = myReader.GetInt32(4);
                    string type = myReader.GetString(5);

                    string status = string.Empty;
                    if (!myReader.IsDBNull(11)) status = myReader.GetString(11);

                    int incidentNo = -1;
                    if (!myReader.IsDBNull(12)) incidentNo = myReader.GetInt32(12);

                    #region Address Data

                    string building = string.Empty;
                    if (!myReader.IsDBNull(6)) building = myReader.GetString(6);

                    string number = string.Empty;
                    if (!myReader.IsDBNull(7)) number = myReader.GetString(7);

                    string street = string.Empty;
                    if (!myReader.IsDBNull(8)) street = myReader.GetString(8);

                    string town = string.Empty;
                    if (!myReader.IsDBNull(9)) town = myReader.GetString(9);

                    string postcode = string.Empty;
                    if (!myReader.IsDBNull(10)) postcode = myReader.GetString(10);

                    #endregion

                    appliances.Add(new ApplianceDataView(callSign, name, oic, crew, ba, type, building, number, street, town, postcode, status, incidentNo));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (myReader != null)
                    myReader.Close();
                command.Connection.Close();
            }
            return appliances.ToArray();
        }

        /// <summary>
        /// Retrieves a list of appliances, optionally filtered by appliance type and availability.
        /// </summary>
        /// <param name="filterType">Only appliances with a matching type name will be returned. Set to NULL if all appliances are required.</param>
        /// <param name="onlyAvailable">Only appliances that are available will be returned.</param>
        /// <returns>A Dictionary object with the resource call sign as the key and its current longitude and latitude stores as a Point objects X and Y co-ordinates, respectively</returns>
        public Dictionary<string, Point> GetAppliances(string filterType, bool onlyAvailable)
        {
            //the object that will be returned
            Dictionary<string, Point> data = new Dictionary<string, Point>();

            MySqlCommand command;
            string statement = "";

            //dependent on the arguments passed by the user, create the select statement
            if (filterType != "")
            {
                if (onlyAvailable)
                    statement = "SELECT resource.CallSign, address.latitude, address.longitude" + Environment.NewLine +
                                "FROM resource, appliance, address_resource, address, status, resource_status" + Environment.NewLine +
                                "WHERE resource.CallSign = appliance.CallSign" + Environment.NewLine +
                                "AND resource.CallSign = resource_status.ResourceCallSign" + Environment.NewLine +
                                "AND resource.CallSign = address_resource.ResourceCallSign" + Environment.NewLine +
                                "AND Address_Resource.DateTime = (SELECT MAX(Address_Resource.DateTime) FROM Address_Resource WHERE Address_Resource.ResourceCallSign = resource.CallSign)" + Environment.NewLine +
                                "AND address_resource.AddressId = address.Id" + Environment.NewLine +
                                "AND resource_status.DateTime =  (SELECT MAX(resource_status.DateTime) FROM resource_status WHERE resource_status.ResourceCallSign = resource.CallSign)" + Environment.NewLine +
                                "AND resource_status.StatusCode = status.Code" + Environment.NewLine +
                                "AND appliance.ApplianceTypeName = @type" + Environment.NewLine +
                                "AND status.IsAvailable = true" + Environment.NewLine +
                                "ORDER BY resource.CallSign;";
                else
                    statement = "SELECT resource.CallSign, address.latitude, address.longitude" + Environment.NewLine +
                                "FROM resource, appliance, address_resource, address, status, resource_status" + Environment.NewLine +
                                "WHERE resource.CallSign = appliance.CallSign" + Environment.NewLine +
                                "AND resource.CallSign = resource_status.ResourceCallSign" + Environment.NewLine +
                                "AND resource.CallSign = address_resource.ResourceCallSign" + Environment.NewLine +
                                "AND Address_Resource.DateTime = (SELECT MAX(Address_Resource.DateTime) FROM Address_Resource WHERE Address_Resource.ResourceCallSign = resource.CallSign)" + Environment.NewLine +
                                "AND address_resource.AddressId = address.Id" + Environment.NewLine +
                                "AND resource_status.DateTime =  (SELECT MAX(resource_status.DateTime) FROM resource_status WHERE resource_status.ResourceCallSign = resource.CallSign)" + Environment.NewLine +
                                "AND resource_status.StatusCode = status.Code" + Environment.NewLine +
                                "AND appliance.ApplianceTypeName = @type" + Environment.NewLine +
                                "ORDER BY resource.CallSign;";
            }
            else
            {
                if (onlyAvailable)
                    statement = "SELECT resource.CallSign, address.latitude, address.longitude" + Environment.NewLine +
                                "FROM resource, appliance, address_resource, address, status, resource_status" + Environment.NewLine +
                                "WHERE resource.CallSign = appliance.CallSign" + Environment.NewLine +
                                "AND resource.CallSign = resource_status.ResourceCallSign" + Environment.NewLine +
                                "AND resource.CallSign = address_resource.ResourceCallSign" + Environment.NewLine +
                                "AND Address_Resource.DateTime = (SELECT MAX(Address_Resource.DateTime) FROM Address_Resource WHERE Address_Resource.ResourceCallSign = resource.CallSign)" + Environment.NewLine +
                                "AND address_resource.AddressId = address.Id" + Environment.NewLine +
                                "AND resource_status.DateTime =  (SELECT MAX(resource_status.DateTime) FROM resource_status WHERE resource_status.ResourceCallSign = resource.CallSign)" + Environment.NewLine +
                                "AND resource_status.StatusCode = status.Code" + Environment.NewLine +
                                "AND status.IsAvailable = true" + Environment.NewLine +
                                "ORDER BY resource.CallSign;";
                else
                    statement = "SELECT resource.CallSign, address.latitude, address.longitude" + Environment.NewLine +
                                "FROM resource, appliance, address_resource, address, status, resource_status" + Environment.NewLine +
                                "WHERE resource.CallSign = appliance.CallSign" + Environment.NewLine +
                                "AND resource.CallSign = resource_status.ResourceCallSign" + Environment.NewLine +
                                "AND resource.CallSign = address_resource.ResourceCallSign" + Environment.NewLine +
                                "AND Address_Resource.DateTime = (SELECT MAX(Address_Resource.DateTime) FROM Address_Resource WHERE Address_Resource.ResourceCallSign = resource.CallSign)" + Environment.NewLine +
                                "AND address_resource.AddressId = address.Id" + Environment.NewLine +
                                "AND resource_status.DateTime =  (SELECT MAX(resource_status.DateTime) FROM resource_status WHERE resource_status.ResourceCallSign = resource.CallSign)" + Environment.NewLine +
                                "AND resource_status.StatusCode = status.Code" + Environment.NewLine +
                                "ORDER BY resource.CallSign;";
            }

            command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@type", filterType);

            //execute the query
            MySqlDataReader myReader = null;
            try
            {
                // open the connection only if it is currently closed
                if (command.Connection.State == System.Data.ConnectionState.Closed)
                    command.Connection.Open();

                //close the reader if it currently exists
                if (myReader != null && !myReader.IsClosed)
                    myReader.Close();

                myReader = command.ExecuteReader();

                //read the data sent back from MySQL server
                while (myReader.Read())
                {
                    string callSign = myReader.GetString(0);

                    double latitude = double.MinValue;
                    if (!myReader.IsDBNull(1)) latitude = myReader.GetDouble(1);

                    double longitude = double.MinValue;
                    if (!myReader.IsDBNull(2)) longitude = myReader.GetDouble(2);

                    data.Add(callSign, new Point(longitude, latitude));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (myReader != null)
                    myReader.Close();
                command.Connection.Close();
            }
            return data;
        }

        /// <summary>
        /// Retrieves a resource using its unique call sign
        /// </summary>
        /// <param name="callSign">The call sign to retrieve</param>
        /// <returns>An object representing the resource.  
        /// Note that this is the abstract base class and the returned object should be cast to an Appliance or Officer object.</returns>
        public Appliance GetAppliance(string callSign)
        {
            string statement = "SELECT * FROM  resource, appliance, address_resource, address, resource_status, status, ApplianceType, Base" + Environment.NewLine +
                               "INNER JOIN address AS base_address ON base.Id = base_address.Id" + Environment.NewLine +
                               "WHERE resource.CallSign = appliance.CallSign" + Environment.NewLine +
                               "AND resource.CallSign = address_resource.ResourceCallSign" + Environment.NewLine +
                               "AND address_resource.DateTime = ((SELECT MAX(Address_Resource.DateTime) FROM Address_Resource WHERE Address_Resource.ResourceCallSign = resource.CallSign))" + Environment.NewLine +
                               "AND address_resource.AddressId = address.Id" + Environment.NewLine +
                               "AND resource.CallSign = resource_status.ResourceCallSign" + Environment.NewLine +
                               "AND resource_status.StatusCode = status.Code" + Environment.NewLine +
                               "AND resource_status.DateTime = (SELECT MAX(resource_status.DateTime) FROM resource_status WHERE resource_status.ResourceCallSign = resource.CallSign)" + Environment.NewLine +
                               "AND appliance.ApplianceTypeName = ApplianceType.Name" + Environment.NewLine +
                               "AND resource.BaseLocationId = base.Id" + Environment.NewLine +
                               "AND resource.CallSign = @callsign;";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@callsign", callSign);

            //execute the query
            MySqlDataReader myReader = null;
            try
            {
                // open the connection only if it is currently closed
                if (command.Connection.State == System.Data.ConnectionState.Closed)
                    command.Connection.Open();

                //close the reader if it currently exists
                if (myReader != null && !myReader.IsClosed)
                    myReader.Close();

                myReader = command.ExecuteReader();

                //read the data sent back from MySQL server
                while (myReader.Read())
                {
                    string name = myReader.GetString(2);

                    string mobile = string.Empty;
                    if (!myReader.IsDBNull(3)) mobile = myReader.GetString(3);

                    int attachedIncident = -1;
                    if (!myReader.IsDBNull(4)) attachedIncident = myReader.GetInt32(4);

                    string oic = string.Empty;
                    if (!myReader.IsDBNull(7)) oic = myReader.GetString(7);

                    int crew = -1;
                    if (!myReader.IsDBNull(8)) crew = myReader.GetInt32(8);

                    int ba = -1;
                    if (!myReader.IsDBNull(9)) ba = myReader.GetInt32(9);

                    #region Address Data

                    int addressId = -1;
                    if (!myReader.IsDBNull(14)) addressId = myReader.GetInt32(14);

                    string building = string.Empty;
                    if (!myReader.IsDBNull(15)) building = myReader.GetString(15);

                    string number = string.Empty;
                    if (!myReader.IsDBNull(16)) number = myReader.GetString(16);

                    string street = string.Empty;
                    if (!myReader.IsDBNull(17)) street = myReader.GetString(17);

                    string town = string.Empty;
                    if (!myReader.IsDBNull(18)) town = myReader.GetString(18);

                    string postcode = string.Empty;
                    if (!myReader.IsDBNull(19)) postcode = myReader.GetString(19);

                    string county = string.Empty;
                    if (!myReader.IsDBNull(20)) county = myReader.GetString(20);

                    double latitude = myReader.GetDouble(21);
                    double longitude = myReader.GetDouble(22);

                    Address address = new Address(addressId, building, number, street, town, postcode, county, longitude, latitude);

                    #endregion

                    #region Status Data

                    int code = -1;
                    if (!myReader.IsDBNull(27)) code = myReader.GetInt32(27);

                    string codeDescription = string.Empty;
                    if (!myReader.IsDBNull(28)) codeDescription = myReader.GetString(28);

                    bool isAvail = false;
                    if (!myReader.IsDBNull(29)) isAvail = myReader.GetBoolean(29);

                    bool isMob = false;
                    if (!myReader.IsDBNull(30)) isMob = myReader.GetBoolean(30);

                    ResourceLogTime log = ResourceLogTime.Ignore;
                    if (!myReader.IsDBNull(31)) log = (ResourceLogTime)myReader.GetInt32(31);

                    ResourceStatus status = new ResourceStatus(code, codeDescription, isAvail, isMob, log);

                    #endregion

                    #region Appliance Type Data

                    string typeName = string.Empty;
                    if (!myReader.IsDBNull(32)) typeName = myReader.GetString(32);

                    string typeDescription = string.Empty;
                    if (!myReader.IsDBNull(33)) typeDescription = myReader.GetString(33);

                    ApplianceType type = new ApplianceType(typeName, typeDescription);

                    #endregion

                    #region Base Data

                    int baseId = -1;
                    if (!myReader.IsDBNull(34)) baseId = myReader.GetInt32(34);

                    int baseAddressId = -1;
                    if (!myReader.IsDBNull(35)) baseAddressId = myReader.GetInt32(35);

                    string office = string.Empty;
                    if (!myReader.IsDBNull(36)) office = myReader.GetString(36);

                    string baseName = string.Empty;
                    if (!myReader.IsDBNull(37)) name = myReader.GetString(37);


                    string baseBuilding = string.Empty;
                    if (!myReader.IsDBNull(39)) building = myReader.GetString(39);

                    string baseNumber = string.Empty;
                    if (!myReader.IsDBNull(40)) number = myReader.GetString(40);

                    string baseStreet = string.Empty;
                    if (!myReader.IsDBNull(41)) street = myReader.GetString(41);

                    string baseTown = string.Empty;
                    if (!myReader.IsDBNull(42)) town = myReader.GetString(42);

                    string basePostcode = string.Empty;
                    if (!myReader.IsDBNull(43)) postcode = myReader.GetString(43);

                    string baseCounty = string.Empty;
                    if (!myReader.IsDBNull(44)) county = myReader.GetString(44);

                    double baseLatitude = myReader.GetDouble(45);
                    double baseLongitude = myReader.GetDouble(46);

                    Address baseAddress = new Address(baseId, baseBuilding, baseNumber, baseStreet, baseTown, basePostcode, baseCounty, baseLongitude, baseLatitude);
                    Base baseLocation = new Base(baseId, office, baseAddress, baseName);

                    #endregion

                    ApplianceInfo info = new ApplianceInfo(name, mobile, baseLocation, address, status, oic, crew, ba, type, attachedIncident);
                    return new Appliance(callSign, info);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (myReader != null)
                    myReader.Close();
                command.Connection.Close();
            }
            return null;
        }

        /// <summary>
        /// Retrieves a list of all appliance types.
        /// </summary>
        /// <returns>A list of objects, each representing a single appliance type.</returns>
        public ApplianceType[] GetAllApplianceTypes()
        {
            string statement = "SELECT * FROM ApplianceType;"; //the SELECT statement

            //the object that will execute the query
            MySqlCommand command = new MySqlCommand(statement, connection);

            //list that will return the appliance types
            List<ApplianceType> data = new List<ApplianceType>();

            //execute the query
            MySqlDataReader myReader = null;
            try
            {
                // open the connection only if it is currently closed
                if (command.Connection.State == System.Data.ConnectionState.Closed)
                    command.Connection.Open();

                //close the reader if it currently exists
                if (myReader != null && !myReader.IsClosed)
                    myReader.Close();

                myReader = command.ExecuteReader();

                //read the data sent back from MySQL server
                while (myReader.Read())
                {
                    string name = myReader.GetString(0);
                    string description = myReader.GetString(1);
                    data.Add(new ApplianceType(name, description));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (myReader != null)
                    myReader.Close();
                command.Connection.Close();
            }
            return data.ToArray();
        }

        #endregion

        #region Get Resource Data

        /// <summary>
        /// Retrieves a list of all resource states.
        /// </summary>
        /// <returns>A list of objects, each representing a single resource state.</returns>
        public ResourceStatus[] GetAllResourceStates()
        {
            //holds the resource states
            List<ResourceStatus> data = new List<ResourceStatus>();

            //the SELECT statement
            string statement = "SELECT * FROM " + database + ".Status;";

            //the object that will execute the query
            MySqlCommand command = new MySqlCommand(statement, connection);

            //execute query
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
                    int code = myReader.GetInt32(0);
                    string description = myReader.GetString(1);
                    bool isAvailable = myReader.GetBoolean(2);
                    bool isMobile = myReader.GetBoolean(3);

                    ResourceLogTime log = ResourceLogTime.Ignore;
                    try
                    {
                        log = (ResourceLogTime)myReader.GetInt32(4);
                    }
                    catch (Exception) { }

                    data.Add(new ResourceStatus(code, description, isAvailable, isMobile, log));
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

            return data.ToArray();
        }

        /// <summary>
        /// Retrieves a list of all addresses that are connected to resources
        /// </summary>
        /// <returns>An array of objects, each representing a single address</returns>
        public Address[] GetAllResourceAddresses()
        {
            //the select statement
            string statement = "SELECT * FROM Address";

            //will hold the results
            List<Address> data = new List<Address>();

            //object that will execute the query
            MySqlCommand command = new MySqlCommand(statement, connection);

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
                    int addressId = myReader.GetInt32(0);

                    string building = string.Empty;
                    if (!myReader.IsDBNull(1)) building = myReader.GetString(1);

                    string number = string.Empty;
                    if (!myReader.IsDBNull(2)) number = myReader.GetString(2);

                    string street = string.Empty;
                    if (!myReader.IsDBNull(3)) street = myReader.GetString(3);

                    string town = string.Empty;
                    if (!myReader.IsDBNull(4)) town = myReader.GetString(4);

                    string postcode = string.Empty;
                    if (!myReader.IsDBNull(5)) postcode = myReader.GetString(5);

                    string county = string.Empty;
                    if (!myReader.IsDBNull(6)) county = myReader.GetString(6);

                    double longitude = myReader.GetDouble(8);
                    double latitude = myReader.GetDouble(7);

                    data.Add(new Address(addressId, building, number, street, town, postcode, county, longitude, latitude));
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

            return data.ToArray();
        }

        /// <summary>
        /// Retrieves a list of all Fire Service bases stored in the database
        /// </summary>
        /// <returns>An array of objects representing a single base.</returns>
        public Base[] GetAllResourceBases()
        {
            List<Base> data = new List<Base>();

            //build and execute command
            string statement = "SELECT * " +
                               "FROM  " + database + ".Base " +
                               "INNER JOIN  " + database + ".Address " +
                               "ON  " + database + ".Base.Id =  " + database + ".Address.Id;";

            MySqlCommand command = new MySqlCommand(statement, connection);

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

                    //fields that hold the results of the database query
                    int baseId = myReader.GetInt32(0);

                    string officePhone = string.Empty;
                    if (!myReader.IsDBNull(2)) officePhone = myReader.GetString(2);

                    string name = myReader.GetString(3);

                    int addressId = myReader.GetInt32(4);

                    string building = string.Empty;
                    if (!myReader.IsDBNull(5)) building = myReader.GetString(5);

                    string number = string.Empty;
                    if (!myReader.IsDBNull(6)) number = myReader.GetString(6);

                    string street = string.Empty;
                    if (!myReader.IsDBNull(7)) street = myReader.GetString(7);

                    string town = string.Empty;
                    if (!myReader.IsDBNull(8)) town = myReader.GetString(8);

                    string postcode = string.Empty;
                    if (!myReader.IsDBNull(9)) postcode = myReader.GetString(9);

                    string county = string.Empty;
                    if (!myReader.IsDBNull(10)) county = myReader.GetString(10);

                    double longitude = myReader.GetDouble(11);
                    double latitude = myReader.GetDouble(12);

                    Address address = new Address(addressId, building, number, street, town, postcode, county, longitude, latitude);

                    data.Add(new Base(baseId));
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

            //MessageBox.Show(data.ToString());
            return data.ToArray();
        }

        #endregion
    }

}
