using MySql.Data.MySqlClient;
using Prototype.Model.Global_Container_Classes;
using Prototype.Model.Resource_Sub_System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Prototype.Model.Global_Classes
{
    /// <summary>
    /// A base class that provides base functionality for MySQL connections, including the creation of the connection string and execution of a statement that returns a single value.
    /// </summary>
    public abstract class BaseMySqlConnector
    {
        #region Fields

        //fields to hold database connection info
        protected string database;
        protected string serverLocation;
        protected string sqlUserName;
        protected string sqlPassword;

        //objects to connect to the database
        protected MySqlConnection connection;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">The name of the database</param>
        /// <param name="serverLocation">The server IP Address</param>
        /// <param name="sqlUsername">The database username</param>
        /// <param name="sqlPassword">The database password</param>
        public BaseMySqlConnector(string database, string serverLocation, string sqlUsername, string sqlPassword)
        {
            this.database = database;
            this.serverLocation = serverLocation;
            this.sqlUserName = sqlUsername;
            this.sqlPassword = sqlPassword;

            connection = new MySqlConnection(buildConnectionString()); //build the connection string
        }

        #region Methods

        /// <summary>
        /// Build the MySQL Connection String.
        /// </summary>
        /// <returns>A connection string that will connect to a MySQL database</returns>
        protected string buildConnectionString()
        {
            string connectionString = "server=" + serverLocation + ";User Id='" + sqlUserName + "';password='" + sqlPassword + "';Persist Security Info=True;database=" + Properties.Settings.Default.DatabaseName;
            return connectionString;
        }

        /// <summary>
        /// Executes a select statement that returns a single string value.
        /// </summary>
        /// <param name="statement">The statement to execute</param>
        /// <returns>The single value required.</returns>
        protected string executeSelectStatement(MySqlCommand command)
        {
            string data = "";

            MySqlDataReader myReader = null;
            try
            {
                // open the connection only if it is currently closed
                if (command.Connection.State == System.Data.ConnectionState.Closed)
                    command.Connection.Open();

                //close the reader if it currently exists
                if (myReader != null && !myReader.IsClosed)
                    myReader.Close();

                myReader = command.ExecuteReader(); //execute the statement

                //read the data sent back from MySQL server
                while (myReader.Read())
                {
                    data = myReader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                MessageBox.Show(command.CommandText);
            }
            finally
            {
                //close the connection
                if (myReader != null)
                    myReader.Close();
                command.Connection.Close();
            }
            return data;
        }

        /// <summary>
        /// Executes a select statement that returns a single integer value.
        /// </summary>
        /// <param name="statement">The statement to execute</param>
        /// <returns>The single value required.</returns>
        protected int executeIntSelectStatement(MySqlCommand command)
        {
            int data = -2; //assign a default value

            MySqlDataReader myReader = null;
            try
            {
                // open the connection only if it is currently closed
                if (command.Connection.State == System.Data.ConnectionState.Closed)
                    command.Connection.Open();

                //close the reader if it currently exists
                if (myReader != null && !myReader.IsClosed)
                    myReader.Close();

                myReader = command.ExecuteReader(); //execute the statement

                //read the data sent back from MySQL server
                while (myReader.Read())
                {
                    data = myReader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                MessageBox.Show(command.CommandText);
            }
            finally
            {
                //close the connection
                if (myReader != null)
                    myReader.Close();
                command.Connection.Close();
            }
            return data;
        }


        /// <summary>
        /// Executes an UPDATE or INSERT statement.
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <returns>True if at least one row was inserted or updated, false otherwise</returns>
        protected bool executeNonQuery(MySqlCommand command)
        {
            int count = -1; //counts the number of rows inserted or updated

            //execute
            try
            {
                command.Connection.Open();
                count = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                command.Connection.Close();
            }

            if (count > 0)
                return true;
            else
                return false;
        }

        #endregion
    }
}
