using System.ComponentModel;

namespace Gather.Core.Domain.Common
{
    public enum ApiAuthenticationType
    {
        /// <summary>
        /// Public
        /// </summary>
        [Description("Valid")]
        Valid = 10,
        /// <summary>
        /// Volunteers Only
        /// </summary>
        [Description("Access Denied")]
        Invalid = 15,
        /// <summary>
        /// Private
        /// </summary>
        [Description("Authentication Code Expired")]
        Expired = 20
    }
}
