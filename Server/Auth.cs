using System.Collections.Generic;

namespace Server
{
    internal class Auth
    {
        private static readonly Dictionary<string, string> credentials = new Dictionary<string, string>
        {
            {"admin","admin"},
            {"test","test"},
            {"user","user"}
        };

        internal static bool Login(string login, string pwd)
        {
            if (credentials.ContainsKey(login) && credentials.ContainsValue(pwd))
            {
                return true;
            }

            return false;
        }
    }
}
