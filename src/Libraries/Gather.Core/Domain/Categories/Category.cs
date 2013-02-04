using System;

namespace Gather.Core.Domain.Categories
{
    public class Category : BaseEntity
    {
        /// <summary>
        /// Created by using user id
        /// </summary>
        public virtual int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the user creation date
        /// </summary>
        public virtual DateTime CreatedDate { get; set; }

        /// <summary>
        /// Last modified by using user id
        /// </summary>
        public virtual int LastModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the last modified date
        /// </summary>
        public virtual DateTime LastModifiedDate { get; set; }
    
        /// <summary>
        /// Gets or sets the name of the category
        /// </summary>
        public virtual string Name { get; set; }
    }
}