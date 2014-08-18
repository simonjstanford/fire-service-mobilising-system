using Prototype.Model.Global_Container_Classes;
using System;
namespace Prototype.Model.Gazetteer_Sub_System 
{
    /// <summary>
    /// An interface used to find additional information stored in the database regarding addresses, for
    /// example urgent messages and additional locations that are attached to the property.
    /// </summary>
	public interface IGazetteerDB
    {
        UrgentMessage[] FindAdditionalInfo(Address address);
	}

}
