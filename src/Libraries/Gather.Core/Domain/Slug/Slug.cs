
using Gather.Core.Domain.Common;

namespace Gather.Core.Domain.Slug
{
    /// <summary>
    /// Represents a url slug
    /// </summary>
    public class Slug : BaseEntity
    {
        /// <summary>
        /// Gets or sets the slug
        /// </summary>
        public virtual string SlugUrl { get; set; }

        /// <summary>
        /// Gets or sets the Success Story Id
        /// </summary>
        public virtual int SuccessStoryId { get; set; }

        /// <summary>
        /// Gets or sets the look up type of the slug
        /// </summary>
        public virtual SlugLookupType LookupType { get; set; }


    }
}
