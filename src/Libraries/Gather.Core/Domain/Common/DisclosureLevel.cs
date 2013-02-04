using System.ComponentModel;

namespace Gather.Core.Domain.Common
{
    public enum DisclosureLevel
    {
        /// <summary>
        /// Public
        /// </summary>
        [Description("Everyone")]
        Public = 10,
        /// <summary>
        /// Volunteers Only
        /// </summary>
        [Description("Good people only")]
        VolunteersOnly = 15,
        /// <summary>
        /// Private
        /// </summary>
        [Description("No one - keep it private")]
        Private = 20
    }
}