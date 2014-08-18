using Prototype.Model.Global_Container_Classes;
using Prototype.Model.Incident_Sub_System.Container_Classes;
using Prototype.Model.Resource_Sub_System.Container_Classes;
using System;
using System.Collections.Generic;
using System.Windows;
namespace Prototype.Model.Incident_Sub_System.Interfaces
{
    /// <summary>
    /// An interface that details the methods that a database object should have to retrieve incident collection information.
    /// </summary>
    public interface IIncidentControllerDB
    {
        Incident CreateIncident(int user, string caller, string exchange, IncidentType incidentType, Address address, string details, int infoId);
        string[] GetAllMessageTypes();
        List<IncidentType> GetAllIncidentTypes();
        IncidentDataView[] GetAllIncidents();
        Dictionary<ApplianceType, int> GetIncidentTypeResonse(string incidentTypeName);
        Dictionary<int, Point> GetAllOpenIncidents();
    }

}
