using System.Collections.Generic;
using System.Linq;

namespace Gather.Web.Framework.UI
{
    public class Url
    {

        private Dictionary<string, string> _parameters; 

        public string Path { get; set; }

        public Dictionary<string, string> Parameters
        {
            get { return _parameters ?? (_parameters = new Dictionary<string, string>()); }
            set { _parameters = value; }
        }

        public Url(Url baseUrl)
        {
            Parameters = new Dictionary<string, string>(baseUrl.Parameters);
            Path = baseUrl.Path;
        }

        public Url(string path)
        {
            Path = path;
        }

        public void AddParam(string key, object value)
        {
            if (Parameters.ContainsKey(key))
            {
                var values = Parameters[key].Split(',');
                if (values.All(x => x != value.ToString()))
                    Parameters[key] = Parameters[key] + "," + value;
            }
            else
            {
                Parameters.Add(key, value.ToString());
            }
        }

        public void RemoveParam(string key, object value = null)
        {
            if (Parameters.ContainsKey(key))
            {
                if(value == null || Parameters[key] == value.ToString())
                {
                    Parameters.Remove(key);
                }
                else
                {
                    var values = Parameters[key].Split(',');
                    Parameters[key] = values.Where(x => x != value.ToString()).Aggregate((current, paramValue) => current + "," + paramValue);
                }
            }
        }

        public new string ToString()
        {
            var parameters = Parameters.Aggregate("?", (current, parameter) => current + (parameter.Key + "=" + parameter.Value + "&")).TrimEnd('&');
            if(Parameters.Count > 0)
                return Path + parameters;
            return Path;
        }

    }
}