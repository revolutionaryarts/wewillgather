using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Gather.Core
{
    /// <summary>
    /// Represents a common helper
    /// </summary>
    public interface IWebHelper
    {
        /// <summary>
        /// Replace line break with HTML break rules
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        MvcHtmlString BreakRuled(string text);

        /// <summary>
        /// Calculate MD5 hashed string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        string CalculateMD5Hash(string s);

        /// <summary>
        /// Custom date format
        /// </summary>
        /// <param name="dateTimeFormat">Format string</param>
        /// <param name="dateTime">Date to format</param>
        /// <returns>Formated dates</returns>
        string DateTimeFormat(string dateTimeFormat, DateTime dateTime);

        /// <summary>
        /// Get all enum options as list items
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>List of select list items</returns>
        IList<SelectListItem> GetAllEnumListItems<T>();

        /// <summary>
        /// Get all enum options
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>Dictionary of options</returns>
        Dictionary<int, string> GetAllEnumOptions<T>();

        /// <summary>
        /// Get all available hours
        /// </summary>
        /// <returns>List of select list items</returns>
        IList<SelectListItem> GetAvailableHours();

        /// <summary>
        /// Get all available minutes
        /// </summary>
        /// <returns>List of select list items</returns>
        IList<SelectListItem> GetAvailableMinutes();

        /// <summary>
        /// Get URL referrer
        /// </summary>
        /// <returns>URL referrer</returns>
        string GetUrlReferrer();

        /// <summary>
        /// Get context IP address
        /// </summary>
        /// <returns>URL referrer</returns>
        string GetCurrentIpAddress();

        /// <summary>
        /// Gets this page name
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings</param>
        /// <returns>Page name</returns>
        string GetThisPageUrl(bool includeQueryString);

        /// <summary>
        /// Gets this page name
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings</param>
        /// <param name="useSsl">Value indicating whether to get SSL protected page</param>
        /// <returns>Page name</returns>
        string GetThisPageUrl(bool includeQueryString, bool useSsl);

        /// <summary>
        /// Gets a unique file name within a given path
        /// </summary>
        /// <param name="path">The path for the file</param>
        /// <param name="fullFilename">The full file name</param>
        /// <returns>Full file path</returns>
        string GetUniqueFileName(string path, string fullFilename);

        /// <summary>
        /// Return the distance between two points in miles or kilometers
        /// </summary>
        /// <param name="lat1">Latitude 1</param>
        /// <param name="lng1">Longitude 1</param>
        /// <param name="lat2">Latitude 2</param>
        /// <param name="lng2">Longitude 2</param>
        /// <param name="type">Distance unit type</param>
        /// <returns>Distance</returns>
        double Haversine(double lat1, double lng1, double lat2, double lng2, DistanceType type);

        /// <summary>
        /// Fully encode a string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        string HtmlFullEncode(string text);

        /// <summary>
        /// Gets a value indicating whether current connection is secured
        /// </summary>
        /// <returns>true - secured, false - not secured</returns>
        bool IsCurrentConnectionSecured();

        /// <summary>
        /// Gets a value indicating whether connection should be secured
        /// </summary>
        /// <returns>Result</returns>
        bool SslEnabled();

        /// <summary>
        /// Gets server variable by name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Server variable</returns>
        string ServerVariables(string name);

        /// <summary>
        /// Gets site host location
        /// </summary>
        /// <param name="useSsl">Use SSL</param>
        /// <returns>Site host location</returns>
        string GetSiteHost(bool useSsl);

        /// <summary>
        /// Gets site location
        /// </summary>
        /// <returns>Site location</returns>
        string GetSiteLocation();

        /// <summary>
        /// Gets site location
        /// </summary>
        /// <param name="useSsl">Use SSL</param>
        /// <returns>Site location</returns>
        string GetSiteLocation(bool useSsl);

        /// <summary>
        /// Returns true if the requested resource is one of the typical resources that needn't be processed by the cms engine.
        /// </summary>
        /// <param name="request">HTTP Request</param>
        /// <returns>True if the request targets a static resource file.</returns>
        bool IsStaticResource(HttpRequest request);

        /// <summary>
        /// Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        string MapPath(string path);

        /// <summary>
        /// Generate a random string of a given length
        /// </summary>
        /// <param name="size">Required string length</param>
        /// <returns>String</returns>
        string RandomString(int size);

        /// <summary>
        /// Restart application domain
        /// </summary>
        /// <param name="redirectUrl">Redirect URL; empty string if you want to redirect to the current page URL</param>
        void RestartAppDomain(string redirectUrl = "");

        /// <summary>
        /// Get a value indicating whether the request is made by search engine (web crawler)
        /// </summary>
        /// <param name="request">HTTP Request</param>
        /// <returns>Result</returns>
        bool IsSearchEngine(HttpRequestBase request);
    }
}