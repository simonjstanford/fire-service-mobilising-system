using Prototype.Model.Global_Container_Classes;
using Prototype.Model.Resource_Sub_System.Container_Classes;
using System;
using System.Collections.Generic;
using System.Windows;
namespace Prototype.Model.Resource_Sub_System
{
    /// <summary>
    /// An interface that details the methods that a database object should have to retrieve resource collection information.
    /// </summary>
	public interface IResourceControllerDB 
    {
        ApplianceDataView[] GetAppliances(bool onlyAttached);
        Dictionary<string, Point> GetAppliances(string filterType, bool onlyAvailable);
		ApplianceType[] GetAllApplianceTypes();
		ResourceStatus[] GetAllResourceStates();
        Address[] GetAllResourceAddresses();
        Base[] GetAllResourceBases();
        Appliance GetAppliance(string callSign);
	}
}
