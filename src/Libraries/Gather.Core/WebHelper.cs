using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Gather.Core
{
    public enum DistanceType
    {
        Kilometers,
        Miles
    }

    public class WebHelper : IWebHelper
    {

        #region Variables

        private readonly Random _random = new Random();
        private const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        #endregion

        #region Methods

        /// <summary>
        /// Replace line break with HTML break rules
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public MvcHtmlString BreakRuled(string text)
        {
            if (!string.IsNullOrEmpty(text))
                text = HttpUtility.HtmlEncode(text).Replace("\n", "<br />");
            return new MvcHtmlString(text);
        }

        /// <summary>
        /// Calculate MD5 hashed string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string CalculateMD5Hash(string s)
        {
            using (var cs = MD5.Create())
            {
                var sb = new StringBuilder();
                byte[] hash = cs.ComputeHash(Encoding.UTF8.GetBytes(s));
                foreach (byte b in hash)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        /// <summary>
        /// Custom date format
        /// </summary>
        /// <param name="dateTimeFormat">Format string</param>
        /// <param name="dateTime">Date to format</param>
        /// <returns>Formated dates</returns>
        public string DateTimeFormat(string dateTimeFormat, DateTime dateTime)
        {
            var dateTimeOutput = dateTime.ToString(dateTimeFormat);
            if (dateTimeFormat.Contains("~"))
                dateTimeOutput = dateTimeOutput.Replace("~", GenerateDaySuffix(dateTime.Day));
            if (dateTimeFormat.Contains("TT"))
                dateTimeOutput = dateTimeOutput.Replace("TT", string.Format("{0:tt}", dateTime).ToLower());
            return dateTimeOutput;
        }

        private string GenerateDaySuffix(int day)
        {
            string suffix;
            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    suffix = "st";
                    break;
                case 2:
                case 22:
                    suffix = "nd";
                    break;
                case 3:
                case 23:
                    suffix = "rd";
                    break;
                default:
                    suffix = "th";
                    break;
            }
            return suffix;
        }

        /// <summary>
        /// Get all enum options as list items
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>List of select list items</returns>
        public IList<SelectListItem> GetAllEnumListItems<T>()
        {
            return GetAllEnumOptions<T>().Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() }).ToList();
        }

        /// <summary>
        /// Get all enum options
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>Dictionary of options</returns>
        public Dictionary<int, string> GetAllEnumOptions<T>()
        {
            var enumValues = Enum.GetValues(typeof(T)).Cast<object>().Select(x => new { ID = (int)x, Name = x.GetDescription() });
            return enumValues.ToDictionary(x => x.ID, x => x.Name);
        }

        /// <summary>
        /// Get all available hours
        /// </summary>
        /// <returns>List of select list items</returns>
        public IList<SelectListItem> GetAvailableHours()
        {
            var hours = new List<SelectListItem>();

            for (int i = 0; i <= 23; i++)
            {
                hours.Add(new SelectListItem { Text = i.ToString("00"), Value = i.ToString() });
            }

            return hours;
        }

        /// <summary>
        /// Get all available minutes
        /// </summary>
        /// <returns>List of select list items</returns>
        public IList<SelectListItem> GetAvailableMinutes()
        {
            var minutes = new List<SelectListItem>
            {
                new SelectListItem { Text = "00", Value = "0" },
                new SelectListItem { Text = "15", Value = "15" },
                new SelectListItem { Text = "30", Value = "30" },
                new SelectListItem { Text = "45", Value = "45" }
            };

            return minutes;
        }

        /// <summary>
        /// Get URL referrer
        /// </summary>
        /// <returns>URL referrer</returns>
        public virtual string GetUrlReferrer()
        {
            string referrerUrl = string.Empty;

            if (HttpContext.Current != null && HttpContext.Current.Request.UrlReferrer != null)
                referrerUrl = HttpContext.Current.Request.UrlReferrer.ToString();

            return referrerUrl;
        }

        /// <summary>
        /// Get context IP address
        /// </summary>
        /// <returns>URL referrer</returns>
        public virtual string GetCurrentIpAddress()
        {
            if (HttpContext.Current != null && HttpContext.Current.Request.UserHostAddress != null)
                return HttpContext.Current.Request.UserHostAddress;
            return string.Empty;
        }

        /// <summary>
        /// Gets this page name
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings</param>
        /// <returns>Page name</returns>
        public virtual string GetThisPageUrl(bool includeQueryString)
        {
            bool useSsl = IsCurrentConnectionSecured();
            return GetThisPageUrl(includeQueryString, useSsl);
        }

        /// <summary>
        /// Gets this page name
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings</param>
        /// <param name="useSsl">Value indicating whether to get SSL protected page</param>
        /// <returns>Page name</returns>
        public virtual string GetThisPageUrl(bool includeQueryString, bool useSsl)
        {
            string url = string.Empty;
            if (HttpContext.Current == null)
                return url;

            if (includeQueryString)
            {
                string siteHost = GetSiteHost(useSsl);
                if (siteHost.EndsWith("/"))
                    siteHost = siteHost.Substring(0, siteHost.Length - 1);
                url = siteHost + HttpContext.Current.Request.RawUrl;
            }
            else
            {
                url = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
            }
            url = url.ToLowerInvariant();
            return url;
        }

        /// <summary>
        /// Return the distance between two points in miles or kilometers
        /// </summary>
        /// <param name="lat1">Latitude 1</param>
        /// <param name="lng1">Longitude 1</param>
        /// <param name="lat2">Latitude 2</param>
        /// <param name="lng2">Longitude 2</param>
        /// <param name="type">Distance unit type</param>
        /// <returns>Distance</returns>
        public double Haversine(double lat1, double lng1, double lat2, double lng2, DistanceType type)
        {
            const double r = Math.PI / 180;
            double lat1Rad = lat1 * r;
            double lng1Rad = lng1 * r;
            double lat2Rad = lat2 * r;
            double lng2Rad = lng2 * r;

            double latitude = lat2Rad - lat1Rad;
            double longitude = lng2Rad - lng1Rad;

            double a = Math.Pow(Math.Sin(latitude / 2), 2) + Math.Cos(lat1Rad) * Math.Cos(lat2Rad) * Math.Pow(Math.Sin(longitude / 2), 2);
            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            double d = (type == DistanceType.Miles ? 3960 : 6371) * c;

            return d;
        }

        /// <summary>
        /// Fully encode a string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string HtmlFullEncode(string text)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in text)
                sb.Append(String.Format("&#{0};", Convert.ToInt16(c)));
            return (sb.ToString());
        }

        /// <summary>
        /// Gets a value indicating whether current connection is secured
        /// </summary>
        /// <returns>true - secured, false - not secured</returns>
        public virtual bool IsCurrentConnectionSecured()
        {
            bool useSsl = false;
            if (HttpContext.Current != null)
            {
                useSsl = HttpContext.Current.Request.IsSecureConnection;
                // Use the below line instead if your server is using a load balancer
                //useSSL = HttpContext.Current.Request.ServerVariables["HTTP_CLUSTER_HTTPS"] == "on" ? true : false;
            }

            return useSsl;
        }

        /// <summary>
        /// Gets a value indicating whether connection should be secured
        /// </summary>
        /// <returns>Result</returns>
        public virtual bool SslEnabled()
        {
            bool useSsl = !String.IsNullOrEmpty(ConfigurationManager.AppSettings["UseSSL"]) && Convert.ToBoolean(ConfigurationManager.AppSettings["UseSSL"]);
            return useSsl;
        }

        /// <summary>
        /// Gets server variable by name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Server variable</returns>
        public virtual string ServerVariables(string name)
        {
            string tmpS = string.Empty;
            try
            {
                if (HttpContext.Current.Request.ServerVariables[name] != null)
                    tmpS = HttpContext.Current.Request.ServerVariables[name];
            }
            catch
            {
                tmpS = string.Empty;
            }
            return tmpS;
        }

        /// <summary>
        /// Gets site host location
        /// </summary>
        /// <param name="useSsl">Use SSL</param>
        /// <returns>Site host location</returns>
        public virtual string GetSiteHost(bool useSsl)
        {
            string result = "http://" + ServerVariables("HTTP_HOST");
            if (!result.EndsWith("/"))
                result += "/";
            if (useSsl)
            {
                //shared SSL certificate URL
                string sharedSslUrl = string.Empty;
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["SharedSSLUrl"]))
                    sharedSslUrl = ConfigurationManager.AppSettings["SharedSSLUrl"].Trim();

                result = !String.IsNullOrEmpty(sharedSslUrl) ? sharedSslUrl : result.Replace("http:/", "https:/");
            }
            else
            {
                if (SslEnabled())
                {
                    //SSL is enabled

                    //get shared SSL certificate URL
                    string sharedSslUrl = string.Empty;
                    if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["SharedSSLUrl"]))
                        sharedSslUrl = ConfigurationManager.AppSettings["SharedSSLUrl"].Trim();
                    if (!String.IsNullOrEmpty(sharedSslUrl))
                    {
                        //shared SSL
                        string nonSharedSslUrl = string.Empty;
                        if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["NonSharedSSLUrl"]))
                            nonSharedSslUrl = ConfigurationManager.AppSettings["NonSharedSSLUrl"].Trim();
                        if (string.IsNullOrEmpty(nonSharedSslUrl))
                            throw new Exception("NonSharedSSLUrl app config setting is not empty");
                        result = nonSharedSslUrl;
                    }
                }
            }

            if (!result.EndsWith("/"))
                result += "/";

            return result.ToLowerInvariant();
        }

        /// <summary>
        /// Gets site location
        /// </summary>
        /// <returns>Site location</returns>
        public virtual string GetSiteLocation()
        {
            bool useSsl = IsCurrentConnectionSecured();
            return GetSiteLocation(useSsl);
        }

        /// <summary>
        /// Gets site location
        /// </summary>
        /// <param name="useSsl">Use SSL</param>
        /// <returns>Site location</returns>
        public virtual string GetSiteLocation(bool useSsl)
        {
            string result = GetSiteHost(useSsl);
            if (result.EndsWith("/"))
                result = result.Substring(0, result.Length - 1);
            result = result + HttpContext.Current.Request.ApplicationPath;
            if (!result.EndsWith("/"))
                result += "/";

            return result.ToLowerInvariant();
        }

        /// <summary>
        /// Used to find a unique file name within a folder path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fullFilename"></param>
        /// <returns></returns>
        public virtual string GetUniqueFileName(string path, string fullFilename)
        {
            fullFilename = RemoveSpecialCharacters(fullFilename);

            if (!File.Exists(path + fullFilename))
                return fullFilename;

            string fileExtension = Path.GetExtension(fullFilename);
            string filename = Path.GetFileNameWithoutExtension(fullFilename);
            int i = 1;
            string returnValue = filename + "-" + i + fileExtension;
            while (File.Exists(path + returnValue))
            {
                i++;
                returnValue = filename + "-" + i + fileExtension;
            }
            return returnValue;
        }

        private string RemoveSpecialCharacters(string str)
        {
            var sb = new StringBuilder();
            foreach (char c in str)
            {
                if (c == ' ')
                {
                    sb.Append("-");
                }
                else if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns true if the requested resource is one of the typical resources that needn't be processed by the cms engine.
        /// </summary>
        /// <param name="request">HTTP Request</param>
        /// <returns>True if the request targets a static resource file.</returns>
        public virtual bool IsStaticResource(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            string path = request.Path;
            string extension = VirtualPathUtility.GetExtension(path);

            if (extension == null) return false;

            switch (extension.ToLower())
            {
                case ".axd":
                case ".ashx":
                case ".bmp":
                case ".css":
                case ".eot":
                case ".gif":
                case ".ico":
                case ".jpeg":
                case ".jpg":
                case ".js":
                case ".png":
                case ".rar":
                case ".svg":
                case ".ttf":
                case ".woff":
                case ".zip":
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public virtual string MapPath(string path)
        {
            if (HttpContext.Current != null)
                return HostingEnvironment.MapPath(path);

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            int binIndex = baseDirectory.IndexOf("\\bin\\");
            if (binIndex >= 0)
                baseDirectory = baseDirectory.Substring(0, binIndex);
            else if (baseDirectory.EndsWith("\\bin"))
                baseDirectory = baseDirectory.Substring(0, baseDirectory.Length - 4);

            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(baseDirectory, path);
        }

        /// <summary>
        /// Generate a random string of a given length
        /// </summary>
        /// <param name="size">Required string length</param>
        /// <returns>String</returns>
        public string RandomString(int size)
        {
            char[] buffer = new char[size];
            for (int i = 0; i < size; i++)
                buffer[i] = CHARS[_random.Next(CHARS.Length)];
            return new string(buffer);
        }

        /// <summary>
        /// Restart application domain
        /// </summary>
        /// <param name="redirectUrl">Redirect URL; empty string if you want to redirect to the current page URL</param>
        public virtual void RestartAppDomain(string redirectUrl = "")
        {
            if (CommonHelper.GetTrustLevel() > AspNetHostingPermissionLevel.Medium)
            {
                //full trust
                HttpRuntime.UnloadAppDomain();
                TryWriteGlobalAsax();
            }
            else
            {
                //medium trust
                bool success = TryWriteWebConfig();
                if (!success)
                {
                    throw new GatherException("#WeWillGather needs to be restarted due to a configuration change, but was unable to do so.\r\n" +
                        "To prevent this issue in the future, a change to the web server configuration is required:\r\n" +
                        "- run the application in a full trust environment, or\r\n" +
                        "- give the application write access to the 'web.config' file.");
                }

                success = TryWriteGlobalAsax();
                if (!success)
                {
                    throw new GatherException("#WeWillGather needs to be restarted due to a configuration change, but was unable to do so.\r\n" +
                        "To prevent this issue in the future, a change to the web server configuration is required:\r\n" +
                        "- run the application in a full trust environment, or\r\n" +
                        "- give the application write access to the 'Global.asax' file.");
                }
            }

            // If setting up extensions/modules requires an AppDomain restart, it's very unlikely the
            // current request can be processed correctly.  So, we redirect to the same URL, so that the
            // new request will come to the newly started AppDomain.
            var httpContext = HttpContext.Current;
            if (httpContext != null)
            {
                if (String.IsNullOrEmpty(redirectUrl))
                    redirectUrl = GetThisPageUrl(true);
                httpContext.Response.Redirect(redirectUrl, true /*endResponse*/);
            }
        }

        private bool TryWriteWebConfig()
        {
            try
            {
                // In medium trust, "UnloadAppDomain" is not supported. Touch web.config
                // to force an AppDomain restart.
                File.SetLastWriteTimeUtc(MapPath("~/web.config"), DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool TryWriteGlobalAsax()
        {
            try
            {
                File.SetLastWriteTimeUtc(MapPath("~/global.asax"), DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get a value indicating whether the request is made by search engine (web crawler)
        /// </summary>
        /// <param name="request">HTTP Request</param>
        /// <returns>Result</returns>
        public virtual bool IsSearchEngine(HttpRequestBase request)
        {
            if (request == null)
                return false;

            bool result = false;
            try
            {
                result = request.Browser.Crawler;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return result;
        }

        #endregion

    }
}