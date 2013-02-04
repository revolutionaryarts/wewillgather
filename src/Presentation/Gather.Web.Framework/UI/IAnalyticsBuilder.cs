namespace Gather.Web.Framework.UI
{
    public interface IAnalyticsBuilder
    {
        /// <summary>
        /// Add tracking record
        /// </summary>
        /// <param name="parts"></param>
        void AddTrackingRecord(params string[] parts);

        /// <summary>
        /// Generate analytics tracking records
        /// </summary>
        /// <returns></returns>
        string GenerateAnalytics();
    }
}