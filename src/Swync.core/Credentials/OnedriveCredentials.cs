namespace Swync.core.Credentials
{
    public class OnedriveCredentials
    {
        public OnedriveCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; }
        public string Password { get; }
    }
}