using System;
using System.IO;
using System.Xml.Serialization;

namespace LatteGrabCore
{
    public class Settings
    {
        String server;
        String username;
        String apiKey;

        public String Server
        {
            get
            {
                return server;
            }

            set
            {
                server = value;
            }
        }

        public String Username
        {
            get
            {
                return username;
            }

            set
            {
                username = value;
            }
        }

        public String APIKey
        {
            get
            {
                return apiKey;
            }

            set
            {
                apiKey = value;
            }
        }

        public Settings()
        {
            server = "https://grabpaw.com";
            username = null;
            apiKey = null;
        }

        public static String DefaultLocation()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LatteGrabCore.xml");
        }

        public static void Serialize(string file, Settings c)
        {
            XmlSerializer xs = new XmlSerializer(c.GetType());
            StreamWriter writer = File.CreateText(file);

            xs.Serialize(writer, c);
            writer.Flush();
            writer.Close();
        }

        public static Settings Deserialize(string file)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Settings));
            StreamReader reader = File.OpenText(file);
            Settings c = (Settings)xs.Deserialize(reader);

            reader.Close();

            return c;
        }
    }
}
