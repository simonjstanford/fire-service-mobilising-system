using System;

namespace Prototype.Model.Incident_Sub_System
{
    /// <summary>
    /// A containter class that stores information relating to a single resource that has been assigned to an incident
    /// </summary>
    public class AssignedResource
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="callSign">The unique call sign of the resource that has been assigned</param>
        /// <param name="alerted">The time that the resource was alerted to the incident</param>
        /// <param name="mobile">The time that the resource started to proceed to the incident</param>
        /// <param name="inAttendance">The time that the resource arrived on scene at the incident</param>
        /// <param name="available">The time that the resource became available from the incident</param>
        /// <param name="closedDown">The time that the resource arrived back at a base station</param>
        /// <remarks>If there is no value yet for any date properties then DateTime.MinValue should be used.</remarks>
        public AssignedResource(string callSign, DateTime alerted, DateTime mobile, DateTime inAttendance, DateTime available, DateTime closedDown, string oic, int crew, int ba)
        {
            //assign arguments to properties
            CallSign = callSign;
            Alerted = alerted;
            Mobile = mobile;
            InAttendance = inAttendance;
            Available = available;
            ClosedDown = closedDown;
            OiC = oic;
            Crew = crew;
            BA = ba;
        }

        #region Crew Properties

        public string OiC { get; private set; }

        public int Crew { get; private set; }

        public int BA { get; private set; }

        #endregion

        #region Date Properties

        /// <summary>
        /// The unique call sign of the resource that has been assigned
        /// </summary>
        public string CallSign { get; private set; }

        /// <summary>
        /// The time that the resource was alerted to the incident.
        /// If there is no value yet for this property then DateTime.MinValue should be used.
        /// </summary>
        public DateTime Alerted { get; private set; }

        /// <summary>
        /// The time that the resource started to proceed to the incident.
        /// If there is no value yet for this property then DateTime.MinValue should be used.
        /// </summary>
        public DateTime Mobile { get; private set; }

        /// <summary>
        /// The time that the resource arrived on scene at the incident.
        /// If there is no value yet for this property then DateTime.MinValue should be used.
        /// </summary>
        public DateTime InAttendance { get; private set; }

        /// <summary>
        /// The time that the resource arrived on scene at the incident.
        /// If there is no value yet for this property then DateTime.MinValue should be used.
        /// </summary>
        public DateTime Available { get; private set; }

        /// <summary>
        /// The time that the resource arrived back at a base station.
        /// If there is no value yet for this property then DateTime.MinValue should be used.
        /// </summary>
        public DateTime ClosedDown { get; private set; }

        #endregion

        #region Formatted Date Properties

        /// <summary>
        /// Returns a UK formatted string representation of the Alerted property.
        /// If the value is DateTime.MinValue then an empty string is returned.
        /// </summary>
        public string AlertedFormatted
        {
            get
            {
                if (Alerted != DateTime.MinValue)
                    return Alerted.ToString();
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Returns a UK formatted string representation of the Mobile property.
        /// If the value is DateTime.MinValue then an empty string is returned.
        /// </summary>
        public string MobileFormatted
        {
            get
            {
                if (Mobile != DateTime.MinValue)
                    return Mobile.ToString();
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Returns a UK formatted string representation of the InAttendance property.
        /// If the value is DateTime.MinValue then an empty string is returned.
        /// </summary>
        public string InAttendanceFormatted
        {
            get
            {
                if (InAttendance != DateTime.MinValue)
                    return InAttendance.ToString();
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Returns a UK formatted string representation of the Available property.
        /// If the value is DateTime.MinValue then an empty string is returned.
        /// </summary>
        public string AvailableFormatted
        {
            get
            {
                if (Available != DateTime.MinValue)
                    return Alerted.ToString();
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Returns a UK formatted string representation of the ClosedDown property.
        /// If the value is DateTime.MinValue then an empty string is returned.
        /// </summary>
        public string ClosedDownFormatted
        {
            get
            {
                if (ClosedDown != DateTime.MinValue)
                    return ClosedDown.ToString();
                else
                    return string.Empty;
            }
        }

        #endregion
    }

}
