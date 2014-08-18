using Prototype.Model.Global_Container_Classes;
using Prototype.Model.Incident_Sub_System.Container_Classes;
using System;
namespace Prototype.Model.Incident_Sub_System.Interfaces
{
    /// <summary>
    /// An interface that details the methods that a database object should have to get/set incident information.
    /// </summary>
    public interface IIncidentDB
    {
        bool SetCaller(int incidentNumber, string newCaller);
        bool SetExchange(int incidentNumber, string newExchange);
        bool SetIncidentType(int incidentNumber, IncidentType newType);
        bool SetDetails(int incidentNumber, string newDetails);     
        bool EnterMessage(int incidentNumber, int user, string text, string type, string callSign);
        bool AssignResource(string callSign, int incidentNumber, int user, string trigger);
        bool SetSummary(int incidentNumber, string text);
        bool SetOiC(int incidentNumber, string oic);
        bool Close(int incidentNumber);
        bool Reopen(int incidentNumber);
        IncidentInfo GetAllDetails(int incidentNumber);
        LogDataView[] GetHistory(int IncidentNumber);
        AssignedResource[] GetAssignedResources(int incidentNumber);
    }
}
