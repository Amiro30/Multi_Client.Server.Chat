namespace Server
{
    internal class Auth
    {
        internal static bool Login(string login, string pwd)
        {
            return login == "admin" && pwd == "admin";
        }
    }
}
