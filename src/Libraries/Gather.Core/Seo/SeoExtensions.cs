using System;
using System.Linq;
using System.Text;
using System.Web;
using Gather.Core.Domain.Locations;
using Gather.Core.Domain.Projects;

namespace Gather.Core.Seo
{
    public static class SeoExtensions
    {

        #region Locations

        public static string GetSeoName(this Location location)
        {
            if(location == null)
                throw new ArgumentNullException("location");
            return GetSeoName(location.Name);
        }

        #endregion

        #region Projects

        public static string GetSeoName(this BaseProject project)
        {
            if(project == null)
                throw new ArgumentNullException("project");
            return GetSeoName(project.Name);
        }

        public static string GetSeoName(this Project project)
        {
            if(project == null)
                throw new ArgumentNullException("project");
            return GetSeoName(project.Name);
        }

        #endregion

        #region Utilities

        public static string GetSeoName(string name, bool urlEncode = false)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            const string okChars = "abcdefghijklmnopqrstuvwxyz1234567890 _-";

            name = name.Trim().ToLower();

            var sb = new StringBuilder();

            foreach (char c in name.Where(c => char.IsLetterOrDigit(c) || okChars.Contains(c)))
                sb.Append(c);

            name = sb.ToString();
            name = name.Replace(" ", "-");

            while (name.Contains("--"))
                name = name.Replace("--", "-");

            while (name.Contains("__"))
                name = name.Replace("__", "_");

            if (urlEncode)
                name = HttpUtility.UrlEncode(name);

            return name;
        }

        #endregion

    }
}