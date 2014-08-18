using Prototype.Model.Global_Container_Classes;
using Prototype.Model.Resource_Sub_System.Container_Classes;
using System;

namespace Prototype.Model.Resource_Sub_System
{
    /// <summary>
    /// An interface that details the methods that a database object should have to get/set information for all resource types.
    /// </summary>
	public interface IResourceDB 
    {
		bool SetName(string callSign, string newName);
        bool SetMobilePhoneNumber(string callSign, string newNumber);
		bool SetBaseInfo(string callSign, Base newBase);
        bool SetCurrentResourceStatus(string callSign, int code, DateTime dateTime, int userId);
        bool SetToAlerted(string callSign, DateTime dateTime, int incidentNo, string trigger, int user);
		bool SetToMobile(string callSign, DateTime dateTime, int incidentNo);
		bool SetToInAttendance(string callSign, DateTime dateTime, int incidentNo);
		bool SetToAvailable(string callSign, DateTime dateTime, int incidentNo);
		bool SetToFinished(string callSign, DateTime dateTime, int incidentNo);
        bool SetCurrentAddress(string callSign, Address address, DateTime dateTime, int userId);
        string[] GetMobilisingMethods(string callsign);
        LogDataView[] GetHistory(string callSign);
    }
}
