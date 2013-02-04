using System.ComponentModel;

namespace Gather.Core.Domain.Common
{
    public enum RecurrenceInterval
    {
        /// <summary>
        /// Unselected
        /// </summary>
        [Description("Select")]
        Unselected = 0,
        /// <summary>
        /// Daily
        /// </summary>
        Daily = 10,
        /// <summary>
        /// Weekly
        /// </summary>
        Weekly = 15,
        /// <summary>
        /// Monthly
        /// </summary>
        Monthly = 20
    }
}