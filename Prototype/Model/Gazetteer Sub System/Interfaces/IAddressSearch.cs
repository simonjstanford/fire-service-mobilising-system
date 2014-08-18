using Prototype.Model.Global_Container_Classes;
using System;

namespace Prototype.Model.Gazetteer_Sub_System 
{
    /// <summary>
    /// An interface that provides the methods necessary to perform an address search.
    /// Used to assist in polymorphim - the class that implements this interface can change,
    /// but the executing class is linked through this interface to reduce the risk of error.
    /// </summary>
	public interface IAddressSearch 
    {
        Address[] FindAddress(string keyword, string county);
        int CalculateRoute(double startLat, double startLong, double endLat, double endLong);
        int CalculateRouteDummy();
    }

}
