using MySql.Data.MySqlClient;
using Prototype.Model.Authentication_Sub_System.Interfaces;
using Prototype.Model.Global_Classes;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
namespace Prototype.Model.Authentication_Sub_System.Data_Classes
{
    /// <summary>
    /// Manages the authentication of system users
    /// </summary>
    public class MySqlAuthenticationConnector : BaseMySqlConnector, IAuthenticationControllerDB
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">The name of the database</param>
        /// <param name="serverLocation">The server IP Address</param>
        /// <param name="sqlUsername">The database username</param>
        /// <param name="sqlPassword">The database password</param>
        public MySqlAuthenticationConnector(string database, string serverLocation, string sqlUsername, string sqlPassword)
            : base(database, serverLocation, sqlUsername, sqlPassword)
        { }

        public bool Logon(int userid, string password)
        {
            if (encrypt(password, getStoredSalt(userid)) == getStoredHash(userid))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Retrieves the name of a user.
        /// </summary>
        /// <param name="userId">The user ID to retrieve the name for</param>
        /// <returns>The user's name</returns>
        public string GetName(int userId)
        {
            string statement = "SELECT name FROM Operator WHERE Id = @userid";
      
            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@userid", userId);

            return executeSelectStatement(command);
        }

        /// <summary>
        /// Taken from: http://www.codeproject.com/Articles/38951/How-To-Hash-Data-Using-MD-and-SHA
        /// Encrypts a password and salt to a SHA512 hash.
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <param name="salt">The salt to add to the password</param>
        /// <returns>The SHA512 hash</returns>
        private string encrypt(string password, string salt)
        {
            //create new instance of md5
            SHA512 sha = SHA512.Create();

            //convert the input text to array of bytes
            byte[] hashData = sha.ComputeHash(Encoding.Default.GetBytes(password + salt));

            //create new instance of StringBuilder to save hashed data
            StringBuilder returnValue = new StringBuilder();

            //loop for each byte and add it to StringBuilder
            for (int i = 0; i < hashData.Length; i++)
                returnValue.Append(hashData[i].ToString());

            // return hexadecimal string
            return returnValue.ToString(); 
        }

        /// <summary>
        /// Retrieves the password hash stored in the database for a specific user
        /// </summary>
        /// <param name="userid">The unique user ID to retrieve the hash for</param>
        /// <returns>The password hash</returns>
        private string getStoredHash(int userid)
        {
            string statement = "SELECT password FROM Operator WHERE Id = @userid";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@userid", userid);

            return executeSelectStatement(command);
        }

        /// <summary>
        /// Retrieves the password salt from the database for a specific user.  
        /// Password salts are used to make decryption of the hash more difficult
        /// </summary>
        /// <param name="userid">The unique user ID to retrieve the password salt for</param>
        /// <returns>The password salt</returns>
        private string getStoredSalt(int userid)
        {
            string statement = "SELECT salt FROM Operator WHERE Id = @userid";

            MySqlCommand command = new MySqlCommand(statement, connection);
            command.Parameters.AddWithValue("@userid", userid);

            return executeSelectStatement(command);
        }
    }
}
