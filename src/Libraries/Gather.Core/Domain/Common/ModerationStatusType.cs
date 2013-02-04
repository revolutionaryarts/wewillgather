using System.ComponentModel;

namespace Gather.Core.Domain.Common
{
    public enum ModerationStatusType
    {
        /// <summary>
        /// Open Status
        /// </summary>
        [Description("Open")]
        Open = 10,
        /// <summary>
        /// Closed Status
        /// </summary>
        [Description("Closed")]
        Closed = 15

    }
}
