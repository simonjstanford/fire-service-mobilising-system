using Prototype.Model.Resource_Sub_System.Container_Classes;
using System;
namespace Prototype.Model.Resource_Sub_System
{
    /// <summary>
    /// An interface that details the methods that a database object should have to get/set appliance information.
    /// </summary>
	public interface IApplianceDB 
    {
		bool SetOiC(string callSign, string oic);
		bool SetNumberOfCrew(string callSign, int numberOfCrew);
		bool SetNumberOfBA(string callSign, int ba);
		bool SetType(string callSign, ApplianceType type);

        ApplianceInfo GetApplianceInfo(string callSign);
	}
}
