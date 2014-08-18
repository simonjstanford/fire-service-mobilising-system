using Prototype.Model.Global_Container_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Model.Resource_Sub_System
{
    /// <summary>
    /// An interface that details the methods that a database object should have to get/set resource base information.
    /// </summary>
    public interface IBaseDB
    {
        void GetBaseInfo(int baseId, out string officeNo, out string baseName, out Address baseAddress);
        bool SetOfficePhoneNumber(int baseId, string newNumber);
        bool SetAddress(int baseId, Address newAddress);
        bool SetName(int baseId, string newName);
    }
}
