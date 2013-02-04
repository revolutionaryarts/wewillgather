
using System.ComponentModel;

namespace Gather.Core.Domain.Common
{
    public enum SlugLookupType
    {
        /// <summary>
        /// Draft
        /// </summary>
        [Description("SlugFound")]
        SlugFound = 10,
        /// <summary>
        /// Pending Approval
        /// </summary>
        [Description("Slug301Found")]
        Slug301Found = 15
    }
}
