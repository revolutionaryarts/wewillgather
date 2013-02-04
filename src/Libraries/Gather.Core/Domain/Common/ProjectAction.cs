using System.ComponentModel;

namespace Gather.Core.Domain.Common
{
    public enum ProjectAction
    {
        /// <summary>
        /// Public
        /// </summary>
        [Description("Join")]
        Join = 10,
        /// <summary>
        /// Approved
        /// </summary>
        [Description("Approved")]
        Approved = 15
    }
}