using System;
using System.Linq;
using Gather.Core;
using Gather.Core.Configuration;
using Gather.Services.Settings;

namespace Gather.Services.Configuration
{
    public class ConfigurationProvider<T> : IConfigurationProvider<T> where T : ISettings, new()
    {

        #region Fields

        private readonly ISettingService _settingService;

        public T Settings { get; set; }

        #endregion

        #region Constructors

        public ConfigurationProvider(ISettingService settingService)
        {
            _settingService = settingService;
            BuildConfiguration();
        }

        #endregion

        #region Methods

        private void BuildConfiguration()
        {
            Settings = Activator.CreateInstance<T>();

            var properties = from prop in typeof(T).GetProperties()
                             where prop.CanWrite && prop.CanRead
                             let setting = _settingService.GetSettingValueByName<string>(typeof(T).Name + "." + prop.Name)
                             where setting != null
                             where CommonHelper.GetCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string))
                             let value = CommonHelper.GetCustomTypeConverter(prop.PropertyType).ConvertFromInvariantString(setting)
                             select new { prop, value };

            properties.ToList().ForEach(p => p.prop.SetValue(Settings, p.value, null));
        }

        public void SaveSettings(T settings)
        {
            var properties = from prop in typeof(T).GetProperties()
                             where prop.CanRead && prop.CanWrite
                             where CommonHelper.GetCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string))
                             select prop;

            foreach (var prop in properties)
            {
                string key = typeof(T).Name + "." + prop.Name;
                dynamic value = prop.GetValue(settings, null);
                _settingService.SetSetting(key, value ?? "", false);
            }

            _settingService.ClearCache();

            Settings = settings;
        }

        #endregion

    }
}