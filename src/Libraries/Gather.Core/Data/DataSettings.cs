using System.Collections.Generic;
using Gather.Core.Domain.Users;

namespace Gather.Core.Data
{
    public class DataSettings
    {
        public DataSettings()
        {
            RawSettings = new Dictionary<string, object>();
        }

        public string ConnectionString { get; set; }

        public bool InstallComplete { get; set; }

        public Dictionary<string, object> RawSettings { get; set; }

        public User SiteOwner { get; set; }
    }
}