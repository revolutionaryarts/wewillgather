using System.Web.Mvc;

namespace Gather.Web.Framework.UI
{
    public static class AnalyticsExtensions
    {

        private const string EVENT_TRACKING_CLICK = "onclick=\"_gaq.push(['_trackEvent', '{0}', '{1}', '{2}']);\"";
        private const string EVENT_TRACKING = "_gaq.push(['_trackEvent', '{0}', '{1}', '{2}']);";
        private const string EVENT_TRANS = "_gaq.push(['_addTrans', '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}']);";
        private const string EVENT_TRACKING_TRANS = "_gaq.push(['_trackTrans']);";
        private const string EVENT_ITEM = "_gaq.push(['_addItem', '{0}', '{1}', '{2}', '{3}', '{4}', '{5}']);";

        public static MvcHtmlString AnalyticsEventTracking(this HtmlHelper html, string category, string subCategory, string label)
        {
            return MvcHtmlString.Create(string.Format(EVENT_TRACKING_CLICK, CleanParameters(category), CleanParameters(subCategory), CleanParameters(label)));
        }

        public static string AnalyticsBuildEventTracking(this HtmlHelper html, string category, string subCategory, string label)
        {
            return string.Format(EVENT_TRACKING, CleanParameters(category), CleanParameters(subCategory), CleanParameters(label));
        }

        public static string AnalyticsBuildEventItem(this HtmlHelper html, string category, string subCategory, string product, string tracker, decimal total, string value = "1")
        {
            return string.Format(EVENT_ITEM, CleanParameters(category), CleanParameters(subCategory), CleanParameters(product), CleanParameters(tracker), total, CleanParameters(value));
        }

        public static string AnalyticsBuildEventTrans(this HtmlHelper html, string category, string tracker, decimal total, decimal vat, decimal value = 0.00M, string value1 = "", string value2 = "", string value3 = "")
        {
            return string.Format(EVENT_TRANS, CleanParameters(category), CleanParameters(tracker), total, vat, value, CleanParameters(value1), CleanParameters(value2), CleanParameters(value3));
        }

        public static string AnalyticsBuildEventTrackingTrans(this HtmlHelper html)
        {
            return EVENT_TRACKING_TRANS;
        }

        public static string CleanParameters(string input)
        {
            return input.Replace("'", "");
        }

    }
}