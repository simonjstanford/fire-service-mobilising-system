using MySql.Data.MySqlClient;
using Prototype.Model.Global_Classes;
using Prototype.Model.Global_Container_Classes;
using Prototype.Model.Resource_Sub_System.Container_Classes;
using Prototype.Model.Resource_Sub_System.Data_Classes;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Prototype.Model.Resource_Sub_System
{
    /// <summary>
    /// Provides functionality to retrieve information from a MySQL database for a specific resource (either appliance or officer).
    /// </summary>
    public class MySqlResourceConnector : BaseMySqlConnector, IResourceDB
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">The name of the database</param>
        /// <param name="serverLocation">The server IP Address</param>
        /// <param name="sqlUsername">The database username</param>
        /// <param name="sqlPassword">The database password</param>
        public MySqlResourceConnector(string database, string serverLocation, string sqlUsername, string sqlPassword)
            : base(database, serverLocation, sqlUsername, sqlPassword)
        {
        }

        #region Get Methods

        /// <summary>
        /// Gets the mobilising methods for the current base station of a specified call sign
        /// </summary>
        /// <param name="callsign">The call sign to check</param>
        /// <returns>An array detailing the mobilising methods</returns>
        public string[] GetMobilisingMethods(string callsign)
        {
            List<string> methods = new List<string>();

            string statement = "SELECT MobilisationMethod_BaseLocation.MobilisationMethodTrigger, status.IsMobile" + Environment.NewLine +
                               "FROM MobilisationMethod_BaseLocation, address_resource, resource_status, status" + Environment.NewLine +
                               "WHERE MobilisationMethod_BaseLocation.BaseLocationId = address_resource.AddressId" + Environment.NewLine +
                               "AND address_resource.ResourceCallSign = resource_status.ResourceCallSign" + Environment.NewLine +
                               "AND resource_status.StatusCode = status.Code" + Environment.NewLine +
                               "AND resource_status.DateTime = (SELECT MAX(resource_status.DateTime) FROM resource_status WHERE resource_status.ResourceCallSign = @callsign)" + Environment.NewLine +
                               "AND address_resource.DateTime = (SELECT MAX(address_resource.DateTime) FROM address_resource WHERE address_resource.ResourceCallSign = @callsign)" + Environment.NewLine +
                               "AND address_resource.ResourceCallSign = @callsign" + Environment.NewLine +
                               "ORDER BY MobilisationMethod_BaseLocation.MobilisationMethodTrigger DESC;";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@callsign", callsign);

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
                    if (myReader.GetBoolean(1))
                    {
                        methods.Add("V");
                        return new string[] { "V" };
                    }
                    else
                        methods.Add(myReader.GetString(0));
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

            return methods.ToArray();
        }

        /// <summary>
        /// Retrieves general historical information relating to a single resource.
        /// This method is designed for efficiency - only necessary information is retrieved from the database to 
        /// be displayed in a data grid.  Processing of database data is carried out within the database
        /// so that it matches the fields exposed in the return class.
        /// </summary>
        /// <param name="callSign">The unique call sign to retrieve information for</param>
        /// <returns>An object that stores the subset of information</returns>
        public LogDataView[] GetHistory(string callSign)
        {
            //holds the data that will be returned
            List<LogDataView> history = new List<LogDataView>();

            //the SELECT statement to execute
            string statement = "(SELECT address_resource.DateTime AS Time," + Environment.NewLine +
                               "CONCAT('Location changed to: ', CONCAT_WS(', ', address.building, address.number, address.street, address.town)) AS Description," + Environment.NewLine +
                               "operator.Name" + Environment.NewLine +
                               "FROM address_resource, address, operator" + Environment.NewLine +
                               "WHERE address_resource.AddressId = Address.Id" + Environment.NewLine +
                               "AND address_resource.OperatorId = Operator.Id" + Environment.NewLine +
                               "AND address_resource.ResourceCallSign = @callsign)" + Environment.NewLine +
                               "UNION" + Environment.NewLine +
                               "(SELECT resource_status.DateTime AS Time, " + Environment.NewLine +
                               "CONCAT('Status changed to: ', status.Description) AS Description," + Environment.NewLine +
                               "operator.Name" + Environment.NewLine +
                               "FROM Resource_Status, Status, operator" + Environment.NewLine +
                               "WHERE Resource_Status.StatusCode = Status.Code" + Environment.NewLine +
                               "AND Resource_Status.OperatorId = Operator.Id" + Environment.NewLine +
                               "AND Resource_Status.ResourceCallSign = @callsign)" + Environment.NewLine +
                               "UNION" + Environment.NewLine +
                               "(SELECT Incident_Resource.Alerted AS Time," + Environment.NewLine +
                               "CONCAT('Alerted to incident number ', Incident_Resource.IncidentIncidentNumber, ' using trigger \"', Incident_Resource.MobTrigger, '\"') AS Description," + Environment.NewLine +
                               "operator.Name" + Environment.NewLine +
                               "FROM incident_resource, operator" + Environment.NewLine +
                               "WHERE Incident_Resource.ResourceCallSign = @callsign" + Environment.NewLine +
                               "AND Incident_Resource.OperatorId = operator.Id)" + Environment.NewLine +
                               "UNION" + Environment.NewLine +
                               "(SELECT Incident_Resource.Mobile AS Time," + Environment.NewLine +
                               "CONCAT('Mobile to incident number ', Incident_Resource.IncidentIncidentNumber) AS Description," + Environment.NewLine +
                               "operator.Name" + Environment.NewLine +
                               "FROM incident_resource, operator" + Environment.NewLine +
                               "WHERE Incident_Resource.ResourceCallSign = @callsign" + Environment.NewLine +
                               "AND Incident_Resource.OperatorId = operator.Id)" + Environment.NewLine +
                               "UNION" + Environment.NewLine +
                               "(SELECT Incident_Resource.InAttendance AS Time," + Environment.NewLine +
                               "CONCAT('On scene at incident number ', Incident_Resource.IncidentIncidentNumber) AS Description," + Environment.NewLine +
                               "operator.Name" + Environment.NewLine +
                               "FROM incident_resource, operator" + Environment.NewLine +
                               "WHERE Incident_Resource.ResourceCallSign = @callsign" + Environment.NewLine +
                               "AND Incident_Resource.OperatorId = operator.Id)" + Environment.NewLine +
                               "UNION" + Environment.NewLine +
                               "(SELECT Incident_Resource.Available AS Time," + Environment.NewLine +
                               "CONCAT('Available from incident number ', Incident_Resource.IncidentIncidentNumber) AS Description," + Environment.NewLine +
                               "operator.Name" + Environment.NewLine +
                               "FROM incident_resource, operator" + Environment.NewLine +
                               "WHERE Incident_Resource.ResourceCallSign = @callsign" + Environment.NewLine +
                               "AND Incident_Resource.OperatorId = operator.Id)" + Environment.NewLine +
                               "UNION" + Environment.NewLine +
                               "(SELECT Incident_Resource.ClosedDown AS Time," + Environment.NewLine +
                               "CONCAT('Clear from incident number ', Incident_Resource.IncidentIncidentNumber) AS Description," + Environment.NewLine +
                               "operator.Name" + Environment.NewLine +
                               "FROM incident_resource, operator" + Environment.NewLine +
                               "WHERE Incident_Resource.ResourceCallSign = @callsign" + Environment.NewLine +
                               "AND Incident_Resource.OperatorId = operator.Id)" + Environment.NewLine +
                               "ORDER BY Time DESC;";

            //the command object that will execute the query
            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@callsign", callSign);

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

        #endregion

        #region Set Methods

        /// <summary>
        /// Gives a new name to a specified resource
        /// </summary>
        /// <param name="callSign">The call sign to change</param>
        /// <param name="newName">The new name</param>
        /// <returns>True if the change was successful, false otherwise</returns>
        public bool SetName(string callSign, string newName)
        {
            //build the SQL command
            string statement = "UPDATE Resource " +
                               "SET Name = @newName " +
                               "WHERE CallSign = @callsign;";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@callsign", callSign);
            command.Parameters.AddWithValue("@newName", newName);

            return executeNonQuery(command);
        }

        /// <summary>
        /// Gets the mobile phone number for the specified call sign
        /// </summary>
        /// <param name="callSign">The callsign to change</param>
        /// <param name="newNumber">The new mobile phone number</param>
        /// <returns>True if the change was successful, false otherwise</returns>
        public bool SetMobilePhoneNumber(string callSign, string newNumber)
        {
            //build the SQL command
            string statement = "UPDATE Resource " +
                               "SET MobilePhoneNumber = @newNumber " +
                               "WHERE CallSign = @callsign;";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@callsign", callSign);
            command.Parameters.AddWithValue("@newNumber", newNumber);

            return executeNonQuery(command);
        }

        /// <summary>
        /// Sets the base information for a specified call sign.
        /// This includes address, office phone number and mobilising methods
        /// </summary>
        /// <param name="callSign">The call sign to change</param>
        /// <param name="newBase">The new base for the resource</param>
        /// <returns>True if the change was successful, false otherwise</returns>
        public bool SetBaseInfo(string callSign, Base newBase)
        {
            //build the SQL command
            string statement = "UPDATE Resource " +
                               "SET BaseLocationId = @newBase " +
                               "WHERE CallSign = @callsign;";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@callsign", callSign);
            command.Parameters.AddWithValue("@newBase", newBase.ID);

            return executeNonQuery(command);
        }

        /// <summary>
        /// Sets the current address of a resource
        /// </summary>
        /// <param name="callSign">The call sign to change</param>
        /// <param name="address">The new address</param>
        /// <param name="dateTime">The date and time the change happened</param>
        /// <param name="userId">The user that made the change</param>
        /// <returns>True if the change was successful, false otherwise</returns>
        public bool SetCurrentAddress(string callSign, Address address, DateTime dateTime, int userId)
        {
            //build the SQL command
            string statement = "INSERT INTO Address_Resource " +
                               "VALUES (@addressId, @callsign, @userId, @dateTime);";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@addressId", address.ID);
            command.Parameters.AddWithValue("@callsign", callSign);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@dateTime", dateTime);

            return executeNonQuery(command);
        }

        /// <summary>
        /// Sets the current status of a resource.
        /// </summary>
        /// <param name="callSign">The call sign to change</param>
        /// <param name="code">The new status code</param>
        /// <param name="dateTime">The date and time the change happened</param>
        /// <param name="userId">The user that made the change</param>
        /// <returns>True if the change was successful, false otherwise</returns>
        public bool SetCurrentResourceStatus(string callSign, int code, DateTime dateTime, int userId)
        {
            //build the SQL command
            string statement = "INSERT INTO resource_status" + Environment.NewLine +
                               "VALUES (@callsign, @code, @datetime, @user);";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@callsign", callSign);
            command.Parameters.AddWithValue("@code", code);
            command.Parameters.AddWithValue("@datetime", dateTime);
            command.Parameters.AddWithValue("@user", userId);

            return executeNonQuery(command);
        }

        #endregion

        #region Methods for when a resource status change affects an incident

        /// <summary>
        /// Sets a resource as 'alerted' on an incident log.  
        /// Allows a record to be kept of the movements of resources assigned to incidents.
        /// </summary>
        /// <param name="callSign">The unique callsign of the resource to mark as alerted</param>
        /// <param name="dateTime">The date and time the change was made.</param>
        /// <param name="incidentNo">The incident number of the log to change.</param>
        /// <param name="trigger">The mobilising method used to contact the resource</param>
        /// <param name="user">The user that made the change</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SetToAlerted(string callSign, DateTime dateTime, int incidentNo, string trigger, int user)
        {
            //first assign the resource to the incident
            string statement1 = "INSERT INTO Incident_Resource" + Environment.NewLine +
                              "VALUES (@incNo, @callsign, @datetime, NULL, NULL, NULL, NULL, @trigger, @user);";

            MySqlCommand command1 = new MySqlCommand(statement1, connection);
            command1.Parameters.AddWithValue("@callsign", callSign);
            command1.Parameters.AddWithValue("@incNo", incidentNo);
            command1.Parameters.AddWithValue("@datetime", dateTime);
            command1.Parameters.AddWithValue("@trigger", trigger);
            command1.Parameters.AddWithValue("@user", user);

            bool result1 = executeNonQuery(command1);

            //then record the incident number the resource is assigned to within the resource itself
            string statement2 = "UPDATE resource" + Environment.NewLine +
                                "SET AttachedIncidentNumber = @incNo" + Environment.NewLine +
                                "WHERE CallSign = @callsign;";

            MySqlCommand command2 = new MySqlCommand(statement2, connection);
            command2.Parameters.AddWithValue("@callsign", callSign);
            command2.Parameters.AddWithValue("@incNo", incidentNo);

            bool result2 = executeNonQuery(command2);


            if (result1 && result2)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Sets a resource as 'mobile' on an incident log.  
        /// Allows a record to be kept of the movements of resources assigned to incidents.
        /// </summary>
        /// <param name="callSign">The unique callsign of the resource to mark as mobile</param>
        /// <param name="dateTime">The date and time the change was made.</param>
        /// <param name="incidentNo">The incident number of the log to change.</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SetToMobile(string callSign, DateTime dateTime, int incidentNo)
        {
            string statement = "UPDATE Incident_Resource" + Environment.NewLine +
                               "SET mobile = @datetime" + Environment.NewLine +
                               "WHERE ResourceCallSign = @callsign" + Environment.NewLine +
                               "AND IncidentIncidentNumber = @incNo;";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@callsign", callSign);
            command.Parameters.AddWithValue("@incNo", incidentNo);
            command.Parameters.AddWithValue("@datetime", dateTime);

            return executeNonQuery(command);
        }

        /// <summary>
        /// Sets a resource as 'in attendance' on an incident log.
        /// Allows a record to be kept of the movements of resources assigned to incidents.
        /// </summary>
        /// <param name="callSign">The unique callsign of the resource to mark as mobile</param>
        /// <param name="dateTime">The date and time the change was made.</param>
        /// <param name="incidentNo">The incident number of the log to change.</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SetToInAttendance(string callSign, DateTime dateTime, int incidentNo)
        {
            string statement = "UPDATE Incident_Resource" + Environment.NewLine +
                               "SET InAttendance = @datetime" + Environment.NewLine +
                               "WHERE ResourceCallSign = @callsign" + Environment.NewLine +
                               "AND IncidentIncidentNumber = @incNo;";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@callsign", callSign);
            command.Parameters.AddWithValue("@incNo", incidentNo);
            command.Parameters.AddWithValue("@datetime", dateTime);

            return executeNonQuery(command);
        }

        /// <summary>
        /// Sets a resource as 'available' on an incident log.  
        /// Allows a record to be kept of the movements of resources assigned to incidents.
        /// </summary>
        /// <param name="callSign">The unique callsign of the resource to mark as mobile</param>
        /// <param name="dateTime">The date and time the change was made.</param>
        /// <param name="incidentNo">The incident number of the log to change.</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SetToAvailable(string callSign, DateTime dateTime, int incidentNo)
        {
            string statement = "UPDATE Incident_Resource" + Environment.NewLine +
                               "SET Available = @datetime" + Environment.NewLine +
                               "WHERE ResourceCallSign = @callsign" + Environment.NewLine +
                               "AND IncidentIncidentNumber = @incNo;";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@callsign", callSign);
            command.Parameters.AddWithValue("@incNo", incidentNo);
            command.Parameters.AddWithValue("@datetime", dateTime);

            return executeNonQuery(command);
        }

        /// <summary>
        /// Sets a resource as 'finished' on an incident log.    
        /// Allows a record to be kept of the movements of resources assigned to incidents.
        /// </summary>
        /// <param name="callSign">The unique callsign of the resource to mark as mobile</param>
        /// <param name="dateTime">The date and time the change was made.</param>
        /// <param name="incidentNo">The incident number of the log to change.</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SetToFinished(string callSign, DateTime dateTime, int incidentNo)
        {
            string statement = "UPDATE Incident_Resource" + Environment.NewLine +
                               "SET ClosedDown = @datetime " + Environment.NewLine +
                               "WHERE ResourceCallSign = @callsign" + Environment.NewLine +
                               "AND IncidentIncidentNumber = @incNo;" + Environment.NewLine +
                               "UPDATE resource" + Environment.NewLine +
                               "SET AttachedIncidentNumber = NULL " + Environment.NewLine +
                               "WHERE CallSign = @callsign;";


            //AttachedIncidentNumber = NULL
            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@callsign", callSign);
            command.Parameters.AddWithValue("@incNo", incidentNo);
            command.Parameters.AddWithValue("@datetime", dateTime);

            return executeNonQuery(command);
        }

        #endregion
    }

}
