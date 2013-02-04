namespace Gather.Core.Data
{
    public class DataSettingsHelper
    {

        public static bool DatabaseIsInstalled
        {
            get
            {
                var manager = new DataSettingsManager();
                var settings = manager.LoadSettings();
                return settings != null && !string.IsNullOrEmpty(settings.ConnectionString);
            }
        }

        public static bool SiteIsInstalled
        {
            get
            {
                var manager = new DataSettingsManager();
                var settings = manager.LoadSettings();
                return DatabaseIsInstalled && settings.InstallComplete;
            }
        }

    }
}