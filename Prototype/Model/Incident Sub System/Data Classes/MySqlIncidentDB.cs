using MySql.Data.MySqlClient;
using Prototype.Model.Global_Classes;
using Prototype.Model.Global_Container_Classes;
using Prototype.Model.Incident_Sub_System.Container_Classes;
using Prototype.Model.Incident_Sub_System.Interfaces;
using Prototype.Model.Resource_Sub_System;
using Prototype.Model.Resource_Sub_System.Container_Classes;
using System;
using System.Collections.Generic;
using System.Windows;
namespace Prototype.Model.Incident_Sub_System.Data_Classes
{
    /// <summary>
    /// Provides functionality to retrieve information from a MySQL database for a specific incident.
    /// </summary>
    public class MySqlIncidentDB : BaseMySqlConnector, IIncidentDB
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">The name of the database</param>
        /// <param name="serverLocation">The server IP Address</param>
        /// <param name="sqlUsername">The database username</param>
        /// <param name="sqlPassword">The database password</param>
        public MySqlIncidentDB(string database, string serverLocation, string sqlUsername, string sqlPassword)
            : base(database: database, serverLocation: serverLocation, sqlUsername: sqlUsername, sqlPassword: sqlPassword)
        { }

        #region Incident Action Methods

        /// <summary>
        /// Assigns a resource to a specified incident.  If this prototype was developed further, it would be here
        /// that external systems would be contacted to physically mobilise the appliances
        /// </summary>
        /// <param name="callSign">The unique callsign of the resource to mobilise</param>
        /// <param name="incidentNumber">The unique number of the incident to mobilise to</param>
        /// <param name="user">The user that carried out the mobilisation</param>
        /// <param name="trigger">The mobilising method that was used to contact the appliance, for example station bells</param>
        /// <returns>True if the resource was assigned successfully, false otherwise</returns>
        public bool AssignResource(string callSign, int incidentNumber, int user, string trigger)
        {
            //first add the resource to assign to the incident
            Tools.ResourceDB.SetToAlerted(callSign, DateTime.Now, incidentNumber, trigger, user);

            //then change the status of the resource to 'alerted'
            //retrieve all the resource states and find one that marks the resource as alerted
            List<ResourceStatus> states = new List<ResourceStatus>(Tools.ResourceControllerDB.GetAllResourceStates());
            ResourceStatus alertedStatus = states.Find(x => x.IncidentLogAction == ResourceLogTime.Alerted);

            //Create an appliance object with the callsign and then assign it the alerted status
            Appliance appliance = Tools.ResourceControllerDB.GetAppliance(callSign);
            appliance.CurrentResourceStatus = alertedStatus;

            return true;
        }

        /// <summary>
        /// Enters a message relating to a particular incident
        /// </summary>
        /// <param name="incidentNumber">The unique incident number of the incident to enter a message for</param>
        /// <param name="user">The user ID that has entered the message</param>
        /// <param name="text">The message text</param>
        /// <param name="type">The type of message</param>
        /// <param name="callSign">The callsign of the resource that sent the message</param>
        /// <returns>True if the change was succesful, otherwise false</returns>
        public bool EnterMessage(int incidentNumber, int user, string text, string type, string callSign)
        {
            //build the SQL command
            string statement = "INSERT INTO message" + Environment.NewLine +
                               "VALUES (@datetime, @user, @incNo, @type, @text);";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@datetime", DateTime.Now);
            command.Parameters.AddWithValue("@user", user);
            command.Parameters.AddWithValue("@incNo", incidentNumber);
            command.Parameters.AddWithValue("@type", type);
            command.Parameters.AddWithValue("@text", text);

            bool result1 = executeNonQuery(command);

            bool result2 = true;
            if (type == "Stop")
            {
                //build the SQL command
                string stopStatement = "UPDATE incident SET StopRecieved = @stoptime WHERE IncidentNumber = @incNo";

                MySqlCommand stopCommand = new MySqlCommand(stopStatement, connection);
                stopCommand.Parameters.AddWithValue("@stoptime", DateTime.Now);
                stopCommand.Parameters.AddWithValue("@incNo", incidentNumber);

                result2 = executeNonQuery(stopCommand);
            }

            return result1 && result2;
        }

        /// <summary>
        /// Marks an incident as closed
        /// </summary>
        /// <param name="incidentNumber">The unique incident number of the incident to mark as closed</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool Close(int incidentNumber)
        {
            //first check to see if all resource have returned from the incident
            IncidentInfo info = GetAllDetails(incidentNumber);

            bool canClose = true;
            foreach (AssignedResource resource in info.AssignedResources)
            {
                if (resource.Alerted != DateTime.MinValue && resource.ClosedDown == DateTime.MinValue)
                {
                    canClose = false;
                    break;
                }
            }

            //if they have then close the incident
            if (canClose)
            {
                string statement = "UPDATE incident" + Environment.NewLine +
                                   "SET IncidentClosed = @datetime" + Environment.NewLine +
                                   "WHERE IncidentNumber = @incNo;";

                MySqlCommand command = new MySqlCommand(statement, connection);
                command.Parameters.AddWithValue("@datetime", DateTime.Now);
                command.Parameters.AddWithValue("@incNo", incidentNumber);

                return executeNonQuery(command);
            }
            else
            {
                //if not, display an error message
                MessageBox.Show("All resources must be unassigned from incident before it can be closed.");
                return false;
            }
        }

        /// <summary>
        /// Un-marks an incident as closed
        /// </summary>
        /// <param name="incidentNumber">The unique incident number of the incident to mark as open</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool Reopen(int incidentNumber)
        {
            string statement = "UPDATE incident" + Environment.NewLine +
                               "SET IncidentClosed = NULL" + Environment.NewLine +
                               "WHERE IncidentNumber = @incNo;";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@incNo", incidentNumber);

            return executeNonQuery(command);
        }

        #endregion

        #region Get Methods

        /// <summary>
        /// Retrieves a subset of incident information to display in the all incidents tab DataSet.
        /// This method was designed for efficiency - all incident is retrieved in a single SELECT statement and passed to an Incident object
        /// which then exposes the data
        /// </summary>
        /// <param name="incidentNumber">The unique incident number to retrieve information for</param>
        /// <returns>An object representing the collection of information for a single incident</returns>
        public IncidentInfo GetAllDetails(int incidentNumber)
        {
            //will hold the results
            DateTime dateTime = DateTime.MinValue;
            string caller = string.Empty;
            string exchange = string.Empty;
            IncidentType type = null;
            string details = string.Empty;
            Address address = null;
            AdditionalAddressInfo additionalAddressInfo = null;
            string summary = string.Empty;
            string oic = string.Empty;
            List<AssignedResource> assignedResources = new List<AssignedResource>();
            string operatorName = string.Empty;
            DateTime stopTime = DateTime.MinValue;
            DateTime incidentClosedTime = DateTime.MinValue;

            //the statement to execute
            string statement = "SELECT * FROM incident" + Environment.NewLine +
                               "INNER JOIN IncidentType" + Environment.NewLine +
                               "ON incident.IncidentTypeName = incidenttype.Name" + Environment.NewLine +
                               "LEFT JOIN Incident_Resource " + Environment.NewLine +
                               "ON incident.IncidentNumber = Incident_Resource.IncidentIncidentNumber" + Environment.NewLine +
                               "INNER JOIN operator" + Environment.NewLine +
                               "ON operator.Id = incident.OperatorId" + Environment.NewLine +
                               "LEFT JOIN appliance" + Environment.NewLine +
                               "ON Incident_Resource.ResourceCallSign = appliance.CallSign" + Environment.NewLine +
                               "WHERE incident.IncidentNumber = @incidentNo;";

            //object that will execute the query
            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@incidentNo", incidentNumber);

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

                //read the data sent back from MySQL server and create the IncidentInfo object
                while (myReader.Read())
                {
                    //retrieve all the data - each area is separated into different regions below

                    #region Incident Details

                    if (!myReader.IsDBNull(12)) dateTime = myReader.GetDateTime(12);
                    if (!myReader.IsDBNull(13)) caller = myReader.GetString(13);
                    if (!myReader.IsDBNull(14)) exchange = myReader.GetString(14);
                    if (!myReader.IsDBNull(20) && !myReader.IsDBNull(21)) type = new IncidentType(myReader.GetString(20), myReader.GetString(21));
                    if (!myReader.IsDBNull(15)) details = myReader.GetString(15);
                    if (!myReader.IsDBNull(18)) summary = myReader.GetString(18);
                    if (!myReader.IsDBNull(19)) oic = myReader.GetString(19);
                    if (!myReader.IsDBNull(32)) operatorName = myReader.GetString(32);
                    if (!myReader.IsDBNull(16)) stopTime = myReader.GetDateTime(16);
                    if (!myReader.IsDBNull(17)) incidentClosedTime = myReader.GetDateTime(17);

                    #endregion

                    #region Address Data

                    string building = string.Empty;
                    if (!myReader.IsDBNull(2)) building = myReader.GetString(2);

                    string number = string.Empty;
                    if (!myReader.IsDBNull(3)) number = myReader.GetString(3);

                    string street = string.Empty;
                    if (!myReader.IsDBNull(4)) street = myReader.GetString(4);

                    string town = string.Empty;
                    if (!myReader.IsDBNull(5)) town = myReader.GetString(5);

                    string postcode = string.Empty;
                    if (!myReader.IsDBNull(6)) postcode = myReader.GetString(6);

                    string county = string.Empty;
                    if (!myReader.IsDBNull(7)) county = myReader.GetString(7);

                    double longitude = myReader.GetDouble(8);
                    double latitude = myReader.GetDouble(9);

                    address = new Address(-1, building, number, street, town, postcode, county, longitude, latitude);

                    #endregion

                    #region Assigned Resource Data

                    string callSign = string.Empty;
                    if (!myReader.IsDBNull(23)) callSign = myReader.GetString(23);

                    DateTime alerted = DateTime.MinValue;
                    if (!myReader.IsDBNull(24)) alerted = myReader.GetDateTime(24);

                    DateTime mobile = DateTime.MinValue;
                    if (!myReader.IsDBNull(25)) mobile = myReader.GetDateTime(25);

                    DateTime inAttendance = DateTime.MinValue;
                    if (!myReader.IsDBNull(26)) inAttendance = myReader.GetDateTime(26);

                    DateTime available = DateTime.MinValue;
                    if (!myReader.IsDBNull(27)) available = myReader.GetDateTime(27);

                    DateTime closedDown = DateTime.MinValue;
                    if (!myReader.IsDBNull(28)) closedDown = myReader.GetDateTime(28);

                    
                    string appOic = string.Empty;
                    if (!myReader.IsDBNull(37)) appOic = myReader.GetString(37);

                    int crew = -1;
                    if (!myReader.IsDBNull(38)) crew = myReader.GetInt32(38);

                    int ba = -1;
                    if (!myReader.IsDBNull(39)) ba = myReader.GetInt32(39);


                    assignedResources.Add(new AssignedResource(callSign, alerted, mobile, inAttendance, available, closedDown, appOic, crew, ba));

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

            //return the data
            return new IncidentInfo(incidentNumber, dateTime, caller, exchange, type, details, address, additionalAddressInfo, summary, oic, assignedResources.ToArray(), operatorName, stopTime, incidentClosedTime);
        }

        /// <summary>
        /// Retrieves a formatted history log of all important actions that have taken place for an incident.
        /// This includes all messages and resource movements.
        /// </summary>
        /// <param name="IncidentNumber"></param>
        /// <returns>An array of objects, each representing a single log entry</returns>
        public LogDataView[] GetHistory(int IncidentNumber)
        {
            //holds the data that will be returned
            List<LogDataView> history = new List<LogDataView>();

            string statement = "(SELECT message.DateTime AS Time, CONCAT(message.MessageTypeName, \" message: \", message.Text) AS Description, operator.Name" + Environment.NewLine +
                               "FROM message, operator" + Environment.NewLine +
                               "WHERE message.OperatorId = operator.Id" + Environment.NewLine +
                               "AND message.IncidentIncidentNumber = @incNo)" + Environment.NewLine +
                               "UNION" + Environment.NewLine +
                               "(SELECT Incident_Resource.alerted AS Time, CONCAT(Incident_Resource.ResourceCallSign, \" has been alerted to incident using trigger '\", Incident_Resource.MobTrigger, \"'\") AS Description, operator.Name" + Environment.NewLine +
                               "FROM Incident_Resource, operator" + Environment.NewLine +
                               "WHERE Incident_Resource.OperatorId = operator.Id" + Environment.NewLine +
                               "AND IncidentIncidentNumber = @incNo)" + Environment.NewLine +
                               "UNION" + Environment.NewLine +
                               "(SELECT Incident_Resource.Mobile AS Time, CONCAT(Incident_Resource.ResourceCallSign, \" is mobile to incident\") AS Description, operator.Name" + Environment.NewLine +
                               "FROM Incident_Resource, operator" + Environment.NewLine +
                               "WHERE Incident_Resource.OperatorId = operator.Id" + Environment.NewLine +
                               "AND IncidentIncidentNumber = @incNo)" + Environment.NewLine +
                               "UNION" + Environment.NewLine +
                               "(SELECT Incident_Resource.InAttendance AS Time, CONCAT(Incident_Resource.ResourceCallSign, \" is on scene\") AS Description, operator.Name" + Environment.NewLine +
                               "FROM Incident_Resource, operator" + Environment.NewLine +
                               "WHERE Incident_Resource.OperatorId = operator.Id" + Environment.NewLine +
                               "AND IncidentIncidentNumber = @incNo)" + Environment.NewLine +
                               "UNION" + Environment.NewLine +
                               "(SELECT Incident_Resource.Available AS Time, CONCAT(Incident_Resource.ResourceCallSign, \" is available from incident\") AS Description, operator.Name" + Environment.NewLine +
                               "FROM Incident_Resource, operator" + Environment.NewLine +
                               "WHERE Incident_Resource.OperatorId = operator.Id" + Environment.NewLine +
                               "AND IncidentIncidentNumber = @incNo)" + Environment.NewLine +
                               "UNION" + Environment.NewLine +
                               "(SELECT Incident_Resource.ClosedDown AS Time, CONCAT(Incident_Resource.ResourceCallSign, \" is clear from incident\") AS Description, operator.Name" + Environment.NewLine +
                               "FROM Incident_Resource, operator" + Environment.NewLine +
                               "WHERE Incident_Resource.OperatorId = operator.Id" + Environment.NewLine +
                               "AND IncidentIncidentNumber = @incNo)" + Environment.NewLine +
                               "ORDER BY Time;";

            //the command object that will execute the query
            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@incNo", IncidentNumber);

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
                    if (!myReader.IsDBNull(0))
                    {
                        DateTime dateTime = myReader.GetDateTime(0);

                        string description = myReader.GetString(1);
                        string user = myReader.GetString(2);
                        history.Add(new LogDataView(user, dateTime, description));
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
            return history.ToArray();
        }

        /// <summary>
        /// Retrieves an array of all resources assigned to an incident, together with the mobilisation times.
        /// </summary>
        /// <param name="incidentNumber">The unique incident number to check</param>
        /// <returns>An array of objects, each respresenting a separate resource mobilisation</returns>
        public AssignedResource[] GetAssignedResources(int incidentNumber)
        {
            //will hold the results
            List<AssignedResource> assignedResources = new List<AssignedResource>();

            //the statement to execute
            string statement = "SELECT * FROM Incident_Resource" + Environment.NewLine +
                               "INNER JOIN appliance" + Environment.NewLine +
                               "ON Incident_Resource.ResourceCallSign = appliance.CallSign" + Environment.NewLine +
                               "WHERE Incident_Resource.IncidentIncidentNumber = @incidentNo" + Environment.NewLine + 
                               "ORDER BY alerted;";

            //object that will execute the query
            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@incidentNo", incidentNumber);

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

                //read the data sent back from MySQL server and create the IncidentInfo object
                while (myReader.Read())
                {
                    string callSign = string.Empty;
                    if (!myReader.IsDBNull(1)) callSign = myReader.GetString(1);

                    DateTime alerted = DateTime.MinValue;
                    if (!myReader.IsDBNull(2)) alerted = myReader.GetDateTime(2);

                    DateTime mobile = DateTime.MinValue;
                    if (!myReader.IsDBNull(3)) mobile = myReader.GetDateTime(3);

                    DateTime inAttendance = DateTime.MinValue;
                    if (!myReader.IsDBNull(4)) inAttendance = myReader.GetDateTime(4);

                    DateTime available = DateTime.MinValue;
                    if (!myReader.IsDBNull(5)) available = myReader.GetDateTime(5);

                    DateTime closedDown = DateTime.MinValue;
                    if (!myReader.IsDBNull(6)) closedDown = myReader.GetDateTime(6);


                    string appOic = string.Empty;
                    if (!myReader.IsDBNull(11)) appOic = myReader.GetString(11);

                    int crew = -1;
                    if (!myReader.IsDBNull(12)) crew = myReader.GetInt32(12);

                    int ba = -1;
                    if (!myReader.IsDBNull(13)) ba = myReader.GetInt32(13);

                    assignedResources.Add(new AssignedResource(callSign, alerted, mobile, inAttendance, available, closedDown, appOic, crew, ba));
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

            //return the data
            return assignedResources.ToArray();
        }

        #endregion

        #region Set Methods

        /// <summary>
        /// Sets the original caller for a specified incident
        /// </summary>
        /// <param name="incidentNumber">The unqiue incident number for the incident to change</param>
        /// <param name="newCaller">The new caller details</param>
        /// <returns>True if the change was successful, false otherwise</returns>
        public bool SetCaller(int incidentNumber, string newCaller)
        {
            //build the SQL command
            string statement = "UPDATE incident" + Environment.NewLine +
                               "SET Caller = @caller" + Environment.NewLine +
                               "WHERE IncidentNumber = @incNo;";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@caller", newCaller);
            command.Parameters.AddWithValue("@incNo", incidentNumber);

            return executeNonQuery(command);
        }

        /// <summary>
        /// Set the exchange that passed the original callers details
        /// </summary>
        /// <param name="incidentNumber">The unqiue incident number for the incident to change</param>
        /// <param name="newExchange">The new exchange details</param>
        /// <returns>True if the change was successful, false otherwise</returns>
        public bool SetExchange(int incidentNumber, string newExchange)
        {
            //build the SQL command
            string statement = "UPDATE incident" + Environment.NewLine +
                               "SET Exchange = @exchange" + Environment.NewLine +
                               "WHERE IncidentNumber = @incNo;";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@exchange", newExchange);
            command.Parameters.AddWithValue("@incNo", incidentNumber);

            return executeNonQuery(command);
        }

        /// <summary>
        /// Sets the incident type for a specified incident
        /// </summary>
        /// <param name="incidentNumber">The unqiue incident number for the incident to change</param>
        /// <param name="newType">The new incident type</param>
        /// <returns>True if the change was successful, false otherwise</returns>
        public bool SetIncidentType(int incidentNumber, IncidentType newType)
        {
            //build the SQL command
            string statement = "UPDATE incident" + Environment.NewLine +
                               "SET IncidentTypeName = @type" + Environment.NewLine +
                               "WHERE IncidentNumber = @incNo;";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@type", newType);
            command.Parameters.AddWithValue("@incNo", incidentNumber);

            return executeNonQuery(command);
        }

        /// <summary>
        /// Sets the details for a specified incident
        /// </summary>
        /// <param name="incidentNumber">The unqiue incident number for the incident to change</param>
        /// <param name="newDetails">The new details</param>
        /// <returns>True if the change was successful, false otherwise</returns>
        public bool SetDetails(int incidentNumber, string newDetails)
        {
            //build the SQL command
            string statement = "UPDATE incident" + Environment.NewLine +
                               "SET Details = @details" + Environment.NewLine +
                               "WHERE IncidentNumber = @incNo;";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@details", newDetails);
            command.Parameters.AddWithValue("@incNo", incidentNumber);

            return executeNonQuery(command);
        }

        /// <summary>
        /// Sets the summary for an incident
        /// </summary>
        /// <param name="incidentNumber">The unqiue incident number for the incident to change</param>
        /// <param name="text">The summary text</param>
        /// <returns>True if the change was successful, false otherwise</returns>
        public bool SetSummary(int incidentNumber, string text)
        {
            //build the SQL command
            string statement = "UPDATE incident" + Environment.NewLine +
                               "SET Summary = @summary" + Environment.NewLine +
                               "WHERE IncidentNumber = @incNo;";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@summary", text);
            command.Parameters.AddWithValue("@incNo", incidentNumber);

            return executeNonQuery(command);
        }

        /// <summary>
        /// Sets the officer in charge (OiC) of the incident
        /// </summary>
        /// <param name="incidentNumber">The unqiue incident number for the incident to change</param>
        /// <param name="oic">The new OiC</param>
        /// <returns>True if the change was successful, false otherwise</returns>
        public bool SetOiC(int incidentNumber, string oic)
        {
            //build the SQL command
            string statement = "UPDATE incident" + Environment.NewLine +
                               "SET OiC = @oic" + Environment.NewLine +
                               "WHERE IncidentNumber = @incNo;";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@oic", oic);
            command.Parameters.AddWithValue("@incNo", incidentNumber);

            return executeNonQuery(command);
        }

        #endregion
    }

}
