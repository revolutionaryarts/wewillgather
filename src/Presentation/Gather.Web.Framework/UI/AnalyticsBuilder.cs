using System;
using System.Collections.Generic;
using System.Linq;

namespace Gather.Web.Framework.UI
{
    public class AnalyticsBuilder : IAnalyticsBuilder
    {

        private readonly List<string> _eventItems;

        public AnalyticsBuilder()
        {
            _eventItems = new List<string>();
        }

        public void AddTrackingRecord(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _eventItems.Add(part);
        }

        public string GenerateAnalytics()
        {
            string result = "";
            var events = string.Join(Environment.NewLine, _eventItems.AsEnumerable().ToArray());
            if (!string.IsNullOrEmpty(events))
                result = events;

            return result;
        }

    }
}