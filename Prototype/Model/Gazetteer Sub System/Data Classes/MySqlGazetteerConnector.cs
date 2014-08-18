using MySql.Data.MySqlClient;
using Prototype.Model.Global_Classes;
using Prototype.Model.Global_Container_Classes;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Prototype.Model.Gazetteer_Sub_System
{
    /// <summary>
    /// Manages the gazetteer database information
    /// </summary>
    public class MySqlGazetteerConnector : BaseMySqlConnector, IGazetteerDB
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">The name of the database</param>
        /// <param name="serverLocation">The server IP Address</param>
        /// <param name="sqlUsername">The database username</param>
        /// <param name="sqlPassword">The database password</param>
        public MySqlGazetteerConnector(string database, string serverLocation, string sqlUsername, string sqlPassword)
            : base(database, serverLocation, sqlUsername, sqlPassword)
        { }

        /// <summary>
        /// Finds any urgent messages stored in the database associated with an address
        /// </summary>
        /// <param name="address">The address to check</param>
        /// <returns>An array of objects, each representing an urgent message</returns>
        public UrgentMessage[] FindAdditionalInfo(Address address)
        {
            List<UrgentMessage> messages = new List<UrgentMessage>();

            string statement = "SELECT name, text FROM UrgentMessage" + Environment.NewLine +
                               "WHERE longitude = @long AND latitude = @lat";


            //object that will execute the query
            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@long", address.Longitude);
            command.Parameters.AddWithValue("@lat", address.Latitude);

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
                    messages.Add(new UrgentMessage(myReader.GetString(0), myReader.GetString(1)));
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

            return messages.ToArray();
        }
    }

}
