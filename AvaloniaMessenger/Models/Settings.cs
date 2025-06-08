using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace AvaloniaMessenger.Models
{
    class Settings
    {
        private static Settings _instance { get; set; }

        public string ConnectionString { get; set; } = "";
        public string ApiKey { get; set; } = "";

        private Settings()
        {
        }
        public static Settings GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Settings();

                string json = "";
                try
                {
                    json = File.ReadAllText("settings.json");
                }
                catch (FileNotFoundException e)
                {
                    File.WriteAllText("settings.json", JsonConvert.SerializeObject(_instance));
                    throw new Exception("Please fill settings.json");
                }


                var settings = JsonConvert.DeserializeObject<Settings>(json);

                if (settings == null)
                {
                    File.ReadAllText(JsonConvert.SerializeObject(_instance));
                    File.WriteAllText("settings.json", JsonConvert.SerializeObject(_instance));
                    throw new Exception("Please fill settings.json");
                }

                _instance = settings;
            }

            return _instance;
        }

        public void SaveSettings()
        {
            File.WriteAllText("settings.json", JsonConvert.SerializeObject(_instance));
        }
    }
}
