using System;
using System.Collections.Generic;
using System.Linq;
using Gather.Core;
using Gather.Core.Cache;
using Gather.Core.Configuration;
using Gather.Core.Data;
using Gather.Core.Domain.Settings;
using Gather.Core.Infrastructure;
using Gather.Core.Domain.Common;

namespace Gather.Services.Settings
{
    public class SettingService : ISettingService
    {

        #region Constants

        private const string SETTINGS_ALL_KEY = "Gather.setting.all";

        #endregion

        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IRepository<Setting> _settingRepository;

        #endregion

        #region Constructors

        public SettingService(ICacheManager cacheManager, IRepository<Setting> settingRepository)
        {
            _cacheManager = cacheManager;
            _settingRepository = settingRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clear cache
        /// </summary>
        public void ClearCache()
        {
            _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);
        }

        /// <summary>
        /// Delete a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        public void DeleteSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingRepository.Delete(setting);

            ClearCache();
        }

        /// <summary>
        /// Retrieve all settings
        /// </summary>
        /// <returns>Setting dictionary</returns>
        public IDictionary<string, Setting> GetAllSettings()
        {
            return _cacheManager.Get(SETTINGS_ALL_KEY, () =>
            {
                var query = from s in _settingRepository.Table
                            orderby s.Name
                            select s;
                return query.ToDictionary(s => s.Name.ToLowerInvariant());
            });
        }


        /// <summary>
        /// Get all settings
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="active">Active flag</param>
        /// <param name="search">search </param>
        /// <returns>Paginated settings collection</returns>
        public IPaginatedList<Setting> GetAllSettings(int pageIndex, int pageSize, bool? active = true, string search = "")
        {
            var query = _settingRepository.Table;

            // Search through the setting name
            if (search != null)
                query = query.Where(s => s.Name.Contains(search));

            // Hide the core and owner settings from appearing in the main list
            query = query.Where(s => !s.Name.Contains(typeof(CoreSettings).Name));
            query = query.Where(s => !s.Name.Contains(typeof(OwnerSettings).Name));

            // Order by name
            query = query.OrderBy(s => s.Name);

            var settings = new PaginatedList<Setting>(query, pageIndex, pageSize);
            return settings;
        }

        /// <summary>
        /// Retrieve a setting by unique identifier
        /// </summary>
        /// <param name="settingId">Unique identifier</param>
        /// <returns>Setting</returns>
        public Setting GetSettingById(int settingId)
        {
            if (settingId == 0)
                return null;
            return _settingRepository.GetById(settingId);
        }

        /// <summary>
        /// Get a setting value by name
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="name">Setting name</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Setting value</returns>
        public T GetSettingValueByName<T>(string name, T defaultValue = default(T))
        {
            if (string.IsNullOrEmpty(name))
                return defaultValue;

            name = name.Trim().ToLowerInvariant();

            var settings = GetAllSettings();
            if (settings.ContainsKey(name))
                return settings[name].As<T>();
            return defaultValue;
        }

        /// <summary>
        /// Get a setting by name
        /// </summary>
        /// <param name="name">Setting name</param>
        /// <returns>Setting</returns>
        public Setting GetSettingByName<T>(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            string settingName = typeof(T).Name + "." + name.ToLower();
            var setting = _settingRepository.Table.FirstOrDefault(x => x.Name == settingName);
            return setting;
        }

        /// <summary>
        /// Adds a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear the setting cache</param>
        private void InsertSetting(Setting setting, bool clearCache)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingRepository.Insert(setting);

            if (clearCache)
                ClearCache();
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Setting name</param>
        /// <param name="value">Setting value</param>
        /// <param name="clearCache">An optional value indicating whether to clear the setting cache, default is true</param>
        public void SetSetting<T>(string key, T value, bool clearCache = true)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            key = key.Trim().ToLower();

            var settings = GetAllSettings();

            string valueStr = CommonHelper.GetCustomTypeConverter(typeof(T)).ConvertToInvariantString(value);
            if (settings.ContainsKey(key))
            {
                Setting setting = settings[key];
                setting = GetSettingById(setting.Id);
                setting.Value = valueStr;
                UpdateSetting(setting, clearCache);
            }
            else
            {
                InsertSetting(new Setting
                {
                    Name = key,
                    Value = valueStr,
                }, clearCache);
            }
        }

        /// <summary>
        /// Update a setting
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="setting">Setting</param>
        public void SaveSetting<T>(T setting) where T : ISettings, new()
        {
            EngineContext.Current.Resolve<IConfigurationProvider<T>>().SaveSettings(setting);
        }

        /// <summary>
        /// Update a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear the setting cache</param>
        public void UpdateSetting(Setting setting, bool clearCache)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            setting.LastModifiedDate = DateTime.Now;
            _settingRepository.Update(setting);

            if (clearCache)
                ClearCache();
        }

        #endregion

    }
}