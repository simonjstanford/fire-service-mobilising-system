using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Model.Global_Container_Classes
{
    /// <summary>
    /// A container class that stores general historical information relating to a single resource.
    /// This class is designed for efficiency - only necessary information is retrieved from the database to 
    /// be displayed in a data grid.  It is expected that processing of database data is carried out within the database
    /// so that it matches the fields exposed in this class
    /// </summary>
    public class LogDataView
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="user">The user that implemented the change that has been logged</param>
        /// <param name="dateTime">The date and time that the change took place</param>
        /// <param name="description">A description of the event</param>
        public LogDataView (string user, DateTime dateTime, string description)
        {
            User = user;
            DateTime = dateTime.ToString();
            Description = description;
        }

        /// <summary>
        /// The user that implemented the change that has been logged
        /// </summary>
        public string User { get; private set; }

        /// <summary>
        /// The date and time that the change took place
        /// </summary>
        public string DateTime { get; private set; }

        /// <summary>
        /// A description of the event
        /// </summary>
        public string Description { get; private set; }

    }
}
