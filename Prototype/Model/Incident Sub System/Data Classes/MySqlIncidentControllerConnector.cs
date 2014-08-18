using MySql.Data.MySqlClient;
using Prototype.Model.Global_Classes;
using Prototype.Model.Global_Container_Classes;
using Prototype.Model.Incident_Sub_System.Container_Classes;
using Prototype.Model.Incident_Sub_System.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using Prototype.Model.Resource_Sub_System.Container_Classes;

namespace Prototype.Model.Incident_Sub_System.Data_Classes
{
    /// <summary>
    /// Manages a collection of incidents.  Provides methods for creating incidents and retrieving a specific incident, a collection of incidents and general information.
    /// </summary>
    public class MySqlIncidentControllerConnector : BaseMySqlConnector, IIncidentControllerDB
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">The name of the database</param>
        /// <param name="serverLocation">The server IP Address</param>
        /// <param name="sqlUsername">The database username</param>
        /// <param name="sqlPassword">The database password</param>
        public MySqlIncidentControllerConnector(string database, string serverLocation, string sqlUsername, string sqlPassword)
            : base(database: database, serverLocation: serverLocation, sqlUsername: sqlUsername, sqlPassword: sqlPassword)
        {
        }

        #region Get/Create Incident Data

        /// <summary>
        /// Records a new incident in the database.
        /// </summary>
        /// <param name="user">The logged in user that has created the incident</param>
        /// <param name="caller">The original caller for the incident</param>
        /// <param name="exchange">The exchange that passed the details of the caller</param>
        /// <param name="incidentType">The incident type of the emergency</param>
        /// <param name="address">The address of the emergency</param>
        /// <param name="details">Any other relevant details</param>
        /// <param name="infoId">The database ID of any relevant info.</param>
        /// <returns>An object representing the newly created incident.</returns>
        public Incident CreateIncident(int user, string caller, string exchange, IncidentType incidentType, Address address, string details, int infoId)
        {
            DateTime dateTime = DateTime.Now;

            ////insert the new incident
            string statement = "INSERT INTO Incident" + Environment.NewLine +
                               "VALUES (NULL, @userID, @building, @number, @street, @town, @postcode, @county, @longitude, @latitude, NULL, @type, @dateTime, @caller, @exchange, @details, NULL, NULL, NULL, NULL);";

            MySqlCommand incidentInsertCommand = new MySqlCommand(statement, connection);
            incidentInsertCommand.Parameters.AddWithValue("@userID", user);
            incidentInsertCommand.Parameters.AddWithValue("@building", address.Building);
            incidentInsertCommand.Parameters.AddWithValue("@number", address.Number);
            incidentInsertCommand.Parameters.AddWithValue("@street", address.Street);
            incidentInsertCommand.Parameters.AddWithValue("@town", address.Town);
            incidentInsertCommand.Parameters.AddWithValue("@postcode", address.Postcode);
            incidentInsertCommand.Parameters.AddWithValue("@county", address.County);
            incidentInsertCommand.Parameters.AddWithValue("@longitude", address.Longitude);
            incidentInsertCommand.Parameters.AddWithValue("@latitude", address.Latitude);
            incidentInsertCommand.Parameters.AddWithValue("@infoId", infoId);
            incidentInsertCommand.Parameters.AddWithValue("@type", incidentType.Name);
            incidentInsertCommand.Parameters.AddWithValue("@dateTime", dateTime);
            incidentInsertCommand.Parameters.AddWithValue("@caller", caller);
            incidentInsertCommand.Parameters.AddWithValue("@exchange", exchange);
            incidentInsertCommand.Parameters.AddWithValue("@details", details);

            bool result = executeNonQuery(incidentInsertCommand);

            //if the insert was successful, return a new Incident object for the newly created incident
            if (result)
            {
                int incidentId = -1;

                try
                {
                    string statement2 = "SELECT MAX(IncidentNumber) FROM Incident WHERE OperatorID = @userId;";
                    MySqlCommand fetchIdCommand = new MySqlCommand(statement2, connection);
                    fetchIdCommand.Parameters.AddWithValue("@userId", user);

                    incidentId = executeIntSelectStatement(fetchIdCommand);

                    //create a new IncidentDataView object to add to the list - this ensures that new incidents are displayed immediately in the incidents window
                    IncidentDataView newView = new IncidentDataView(incidentId, dateTime, incidentType.Name, address.Building, address.Number, address.Street, address.Town, 
                                                                    address.Postcode, Properties.Settings.Default.LoggedInUserName, DateTime.MinValue, DateTime.MinValue);
                    Tools.Incidents.Add(newView);

                    //finally, return the new incident
                    return new Incident(incidentId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to create incident:" + Environment.NewLine + ex.ToString() + Environment.NewLine + "incident number retrieved: " + incidentId);
                }
            }

            //if failed, return null
            return null;
        }

        /// <summary>
        /// Retrieves a subset of incident information to display in the all incidents tab DataSet.
        /// This method was designed for efficiency - only brief information on each incident is retrieved to ensure UI responsiveness
        /// </summary>
        /// <returns>An array of objects, each representing a subset of information relating to a single incident</returns>
        public IncidentDataView[] GetAllIncidents()
        {
            List<IncidentDataView> incidents = new List<IncidentDataView>();

            string statement = "SELECT incident.IncidentNumber, incident.DateTime, incident.IncidentTypeName, incident.building," + Environment.NewLine +
                               "incident.number, incident.street, incident.town, incident.postcode, operator.name, incident.StopRecieved, incident.IncidentClosed" + Environment.NewLine +
                               "FROM incident, operator" + Environment.NewLine +
                               "WHERE incident.OperatorId = operator.Id" + Environment.NewLine +
                               "ORDER BY IncidentNumber;";

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
                    int incidentNumber = myReader.GetInt32(0);
                    DateTime dateTime = myReader.GetDateTime(1);
                    string type = myReader.GetString(2);
                    string operatorId = myReader.GetString(8);

                    #region Address Data

                    string building = string.Empty;
                    if (!myReader.IsDBNull(3)) building = myReader.GetString(3);

                    string number = string.Empty;
                    if (!myReader.IsDBNull(4)) number = myReader.GetString(4);

                    string street = string.Empty;
                    if (!myReader.IsDBNull(5)) street = myReader.GetString(5);

                    string town = string.Empty;
                    if (!myReader.IsDBNull(6)) town = myReader.GetString(6);

                    string postcode = string.Empty;
                    if (!myReader.IsDBNull(7)) postcode = myReader.GetString(7);

                    #endregion

                    DateTime stop = DateTime.MinValue;
                    if (!myReader.IsDBNull(9)) stop = myReader.GetDateTime(9);

                    DateTime closed = DateTime.MinValue;
                    if (!myReader.IsDBNull(10)) closed = myReader.GetDateTime(10);


                    incidents.Add(new IncidentDataView(incidentNumber, dateTime, type, building, number, street, town, postcode, operatorId, stop, closed));
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

            return incidents.ToArray();
        }

        /// <summary>
        /// Retrieves the incident number and longitude/latitude co-ordinates of all incidents that are currently open.
        /// This is used to display a pin on the maps for each open incident.
        /// </summary>
        /// <returns>A dictionary when the key is the incident number and the value is a Point object detailing the longitude and latitude respectively</returns>
        public Dictionary<int, Point> GetAllOpenIncidents()
        {
            Dictionary<int, Point> incidents = new Dictionary<int, Point>();

            string statement = "SELECT IncidentNumber, longitude, latitude" + Environment.NewLine +
                               "FROM incident" + Environment.NewLine +
                               "WHERE IncidentClosed is NULL;";

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
                    int incidentNumber = myReader.GetInt32(0);
                    double longitude = myReader.GetDouble(1);
                    double latitude = myReader.GetDouble(2);

                    incidents.Add(incidentNumber, new Point(longitude, latitude));
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

            return incidents;
        }

        #endregion

        #region Get Incident & Message Type Data

        /// <summary>
        /// Retrieves a list of all incident types from the database
        /// </summary>
        /// <returns>An list of objects, each representing a single incident type</returns>
        public List<IncidentType> GetAllIncidentTypes()
        {
            //the select statement
            string statement = "SELECT IncidentType.Name, IncidentType.Description, Keyword.Keyword" + Environment.NewLine +
                               "FROM IncidentType, Keyword" + Environment.NewLine +
                               "WHERE IncidentType.Name = Keyword.IncidentTypeName" + Environment.NewLine +
                               "ORDER By IncidentType.Name;";

            //will hold the results
            List<IncidentType> data = new List<IncidentType>();

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

                //read the data sent back from MySQL server and create the Appliance objects
                while (myReader.Read())
                {
                    //fields that hold the results of the database query
                    string name = string.Empty;
                    string description = string.Empty;
                    string keyword = string.Empty;

                    name = myReader.GetString(0);
                    description = myReader.GetString(1);

                    if (!myReader.IsDBNull(2))
                        keyword = myReader.GetString(2);

                    //create the Incident Type object
                    IncidentType type = new IncidentType(name, description);

                    //search to see if the incident type has already been added.
                    IncidentType oldType = data.Find(x => x.Name == type.Name);

                    //if not in the list, add the first keyword to the type and add the type to the list
                    if (oldType == null)
                    {
                        type.AddKeyword(keyword);
                        data.Add(type);
                    }
                    else
                    {
                        //if already present, just add the new keyword to the existing type
                        oldType.AddKeyword(keyword);
                    }
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

            //finally, return the list
            return data;
        }


        /// <summary>
        /// For a specified incident type, finds the required appliance types and quantities.
        /// </summary>
        /// <param name="incidentTypeName">A Dictionary object when the appliance type is the key and the quantity is the value</param>
        /// <returns></returns>
        public Dictionary<ApplianceType, int> GetIncidentTypeResonse(string incidentTypeName)
        {
            //the object that will be returned
            Dictionary<ApplianceType, int> response = new Dictionary<ApplianceType, int>();

            string statement = "SELECT ApplianceTypeName, Quantity" + Environment.NewLine +
                               "FROM ApplianceType_IncidentType" + Environment.NewLine +
                               "WHERE IncidentTypeName = @typeName;";

            //object that will execute the query
            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@typeName", incidentTypeName);

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
                    string type = string.Empty;
                    if (!myReader.IsDBNull(0)) type = myReader.GetString(0);

                    int quantity = -1;
                    if (!myReader.IsDBNull(1)) quantity = myReader.GetInt32(1);

                    response.Add(new ApplianceType(type, ""), quantity);
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

            //if no response has yet to be defined for the specified incident type, add a default of 1 pump to the dictionary
            //this ensures that all incident types have a response
            if (response.Count == 0)
                response.Add(new ApplianceType("Pump", ""), 1);

            return response;
        }

        /// <summary>
        /// Returns a list of all message types
        /// </summary>
        /// <returns>An array of string, each string representing a different message type</returns>
        public string[] GetAllMessageTypes()
        {
            //the select statement
            string statement = "SELECT * FROM messagetype ORDER BY name;";

            //will hold the results
            List<string> data = new List<string>();

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

                //read the data sent back from MySQL server and create the Appliance objects
                while (myReader.Read())
                {
                    //fields that hold the results of the database query
                    string name = string.Empty;
                    if (!myReader.IsDBNull(0)) name = myReader.GetString(0);

                    data.Add(name);
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

            //finally, return the list
            return data.ToArray();
        }

        #endregion
    }

}
