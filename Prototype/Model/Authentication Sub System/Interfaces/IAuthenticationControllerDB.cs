using System;

namespace Prototype.Model.Authentication_Sub_System.Interfaces
{
    /// <summary>
    /// Provides user authentication functionality
    /// </summary>
    public interface IAuthenticationControllerDB
    {
        bool Logon(int userid, string password);
        string GetName(int userId);
    }

}
