using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using Gather.Core.Domain.Users;
using Newtonsoft.Json;

namespace Gather.Core.Data
{
    public class DataSettingsManager
    {

        private const string SETTINGS_FILE_NAME = "settings.txt";

        private DataSettings ParseSettings(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var rawSettings = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            var settings = new DataSettings();

            foreach (var setting in rawSettings)
            {
                switch (setting.Key)
                {
                    case "ConnectionString":
                        if (setting.Value != null)
                            settings.ConnectionString = setting.Value.ToString();
                        break;
                    case "InstallComplete":
                        if (setting.Value != null)
                            settings.InstallComplete = bool.Parse(setting.Value.ToString());
                        break;
                    case "RawSettings":
                        break;
                    case "SiteOwner":
                        if (setting.Value != null)
                            settings.SiteOwner = JsonConvert.DeserializeObject<User>(setting.Value.ToString());
                        break;
                    default:
                        settings.RawSettings.Add(setting.Key, setting.Value);
                        break;
                }
            }

            return settings;
        }

        public DataSettings LoadSettings()
        {
            string directoryPath = HostingEnvironment.MapPath("~/App_Data");
            if (!string.IsNullOrEmpty(directoryPath))
            {
                string filePath = Path.Combine(directoryPath, SETTINGS_FILE_NAME);
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    return ParseSettings(json);
                }
            }

            return new DataSettings();
        }

        public void SaveSettings(DataSettings settings)
        {
            string directoryPath = HostingEnvironment.MapPath("~/App_Data");
            if (!string.IsNullOrEmpty(directoryPath))
            {
                string filePath = Path.Combine(directoryPath, SETTINGS_FILE_NAME);
                var json = JsonConvert.SerializeObject(settings);
                File.WriteAllText(filePath, json);
            }
        }

    }
}