using System.ComponentModel;

namespace Gather.Core.Domain.Common
{
    public enum EntityType
    {
        /// <summary>
        /// Success Story
        /// </summary>
        [Description("Success Story")]
        SuccessStory = 1,
        /// <summary>
        /// Page
        /// </summary>
        [Description("Page")]
        Page = 2
    }
}