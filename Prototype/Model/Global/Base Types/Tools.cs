using Prototype.Model.Authentication_Sub_System.Data_Classes;
using Prototype.Model.Authentication_Sub_System.Interfaces;
using Prototype.Model.Gazetteer_Sub_System;
using Prototype.Model.Incident_Sub_System.Container_Classes;
using Prototype.Model.Incident_Sub_System.Data_Classes;
using Prototype.Model.Incident_Sub_System.Interfaces;
using Prototype.Model.Resource_Sub_System;
using Prototype.Model.Resource_Sub_System.Container_Classes;
using Prototype.Model.Resource_Sub_System.Data_Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Model.Global_Classes
{
    /// <summary>
    /// A static class providing access to common tools and data classes
    /// </summary>
    internal static class Tools
    {
        #region Database Connection Fields

        //fields that hold database access information
        private static string dbName { get { return Properties.Settings.Default.DatabaseName; } }
        private static string ip { get { return Properties.Settings.Default.ServerIP; } }
        private static string username { get { return Properties.Settings.Default.Username; } }
        private static string password { get { return Properties.Settings.Default.Password; } }

        #endregion

        internal static ObservableCollection<IncidentDataView> Incidents;
        internal static ObservableCollection<ApplianceDataView> Appliances; //backing field containing a list of all appliances

        #region Singleton Database Objects

        private static IBaseDB baseLocation;
        /// <summary>
        /// A singleton property that exposes a reference to an object that implements the IBaseDB interface.
        /// </summary>
        internal static IBaseDB BaseLoctionDB
        {
            get
            {
                if (baseLocation != null)
                    return baseLocation;
                else
                    return baseLocation = new MySqlBaseLocationConnector(dbName, ip, username, password);
            }
        }


        private static IResourceDB resourceDB;
        /// <summary>
        /// A singleton property that exposes a reference to an object that implements the IResourceDB interface.
        /// </summary>
        internal static IResourceDB ResourceDB
        {
            get
            {
                if (resourceDB != null)
                    return resourceDB;
                else
                    return resourceDB = new MySqlResourceConnector(dbName, ip, username, password);
            }
        }

        private static IApplianceDB applianceDB;
        /// <summary>
        /// A singleton property that exposes a reference to an object that implements the IApplianceDB interface.
        /// </summary>
        internal static IApplianceDB ApplianceDB
        {
            get
            {
                if (applianceDB != null)
                    return applianceDB;
                else
                    return applianceDB = new MySqlApplianceConnector(dbName, ip, username, password);
            }
        }

        private static IResourceControllerDB resourceControllerDB;
        /// <summary>
        /// A singleton property that exposes a reference to an object that implements the IResourceControllerDB interface.
        /// </summary>
        internal static IResourceControllerDB ResourceControllerDB
        {
            get
            {
                if (resourceControllerDB != null)
                    return resourceControllerDB;
                else
                    return resourceControllerDB = new MySqlResourceControllerConnector(dbName, ip, username, password);
            }
        }

        private static IGazetteerDB gazetteerDB;
        /// <summary>
        /// A singleton property that exposes a reference to an object that implements the IGazetteerDB interface.
        /// </summary>
        internal static IGazetteerDB GazetteerDB
        {
            get
            {
                if (gazetteerDB != null)
                    return gazetteerDB;
                else
                    return gazetteerDB = new MySqlGazetteerConnector(dbName, ip, username, password);
            }
        }

        private static IIncidentControllerDB incidentControllerDB;
        /// <summary>
        /// A singleton property that exposes a reference to an object that implements the IIncidentControllerDB interface.
        /// </summary>
        internal static IIncidentControllerDB IncidentControllerDB
        {
            get
            {
                if (incidentControllerDB != null)
                    return incidentControllerDB;
                else
                    return incidentControllerDB = new MySqlIncidentControllerConnector(dbName, ip, username, password);
            }
        }

        private static IIncidentDB incidentDB;
        /// <summary>
        /// A singleton property that exposes a reference to an object that implements the IIncidentDB interface.
        /// </summary>
        internal static IIncidentDB IncidentDB
        {
            get
            {
                if (incidentDB != null)
                    return incidentDB;
                else
                    return incidentDB = new MySqlIncidentDB(dbName, ip, username, password);
            }
        }

        private static IAddressSearch addressSearch;
        /// <summary>
        /// A singleton property that exposes a reference to an object that implements the IAddressSearch interface.
        /// </summary>
        internal static IAddressSearch AddressSearch
        {
            get
            {
                if (addressSearch != null)
                    return addressSearch;
                else
                    return addressSearch = new GoogleMapsConnector();
            }
        }

        private static IAuthenticationControllerDB authentication;

        public static IAuthenticationControllerDB Authentication
        {
            get
            {
                if (authentication != null)
                    return authentication;
                else
                    return authentication = new MySqlAuthenticationConnector(dbName, ip, username, password);
            }
        }
        
        #endregion

        #region Misc.
        /// <summary>
        /// Creates a custom time string in the format xh xm xs
        /// </summary>
        /// <param name="seconds">The total number of seconds to covert</param>
        /// <returns>The formatted time string</returns>
        internal static string GetTimeString(int seconds)
        {
            TimeSpan time = new TimeSpan(0, 0, seconds);
            string timeString = "";
            if (time.Hours > 0) //if the length is longer than an hour, add hours to the string
                timeString += time.Hours + "h ";
            timeString += time.Minutes + "m ";
            timeString += time.Seconds + "s ";
            return timeString;
        }

        #endregion
    }
}
