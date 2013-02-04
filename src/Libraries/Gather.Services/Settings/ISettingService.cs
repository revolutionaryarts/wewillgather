using System.Collections.Generic;
using Gather.Core;
using Gather.Core.Configuration;
using Gather.Core.Domain.Settings;

namespace Gather.Services.Settings
{
    public interface ISettingService
    {

        /// <summary>
        /// Clear cache
        /// </summary>
        void ClearCache();

        /// <summary>
        /// Delete a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        void DeleteSetting(Setting setting);

        /// <summary>
        /// Retrieve all settings
        /// </summary>
        /// <returns>Setting dictionary</returns>
        IDictionary<string, Setting> GetAllSettings();

        /// <summary>
        /// Get all Settings
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="active">Active flag</param>
        /// <param name="search">Search term</param>
        /// <returns>Paginated setting collection</returns>
        IPaginatedList<Setting> GetAllSettings(int pageIndex, int pageSize, bool? active = true, string search = "");

        /// <summary>
        /// Retrieve a setting by unique identifier
        /// </summary>
        /// <param name="settingId">Unique identifier</param>
        /// <returns>Setting</returns>
        Setting GetSettingById(int settingId);

        /// <summary>
        /// Get a setting by name
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="name">Setting name</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Setting value</returns>
        T GetSettingValueByName<T>(string name, T defaultValue = default(T));

        /// <summary>
        /// Get a setting by name
        /// </summary>
        /// <param name="name">Setting name</param>
        /// <returns>Setting</returns>
        Setting GetSettingByName<T>(string name);

        /// <summary>
        /// Update a setting
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="setting">Setting</param>
        void SaveSetting<T>(T setting) where T : ISettings, new();

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Setting name</param>
        /// <param name="value">Setting value</param>
        /// <param name="clearCache">An optional value indicating whether to clear the setting cache, default is true</param>
        void SetSetting<T>(string key, T value, bool clearCache = true);

        /// <summary>
        /// Updates a setting
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="clearCache"></param>
        void UpdateSetting(Setting setting, bool clearCache);

    }
}