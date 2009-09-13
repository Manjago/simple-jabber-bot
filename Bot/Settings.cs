using System.Configuration;

namespace Temnenkov.SJB.Bot
{
    public static class Settings
    {

        public static string Get(string key, string defaultValue)
        {
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]))
                return defaultValue;
            return ConfigurationManager.AppSettings[key];
        }

        public static int Get(string key, int defaultValue)
        {
            return int.Parse(Get(key, defaultValue.ToString()));
        }

        public static bool Get(string key, bool defaultValue)
        {
            return bool.Parse(Get(key, defaultValue.ToString()));
        }

        public static string JabberUser
        {
            get
            {
                return Get("user", "bot828");
            }
        }

        public static string JabberServer
        {
            get
            {
                return Get("server", "jabber.org");
            }
        }

        public static string JabberPassword
        {
            get
            {
                return Get("password", "");
            }
        }

        public static string RoomJid
        {
            get
            {
                return Get("roomJid", "");
            }
        }
    }
}
