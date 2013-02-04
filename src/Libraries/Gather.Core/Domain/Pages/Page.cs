using System;

namespace Gather.Core.Domain.Pages
{
    public class Page : BaseEntity
    {
        /// <summary>
        /// The content of the page 
        /// </summary>
        public virtual string Content { get; set; }

        /// <summary>
        /// Created by using user id
        /// </summary>
        public virtual int? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the user creation date
        /// </summary>
        public virtual DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the file title for the files attached
        /// </summary>
        public virtual string FileTitle { get; set; }

        /// <summary>
        /// Gets or sets if a page is a system page and therefore cant be deleted
        /// </summary>
        public virtual bool IsSystemPage { get; set; }

        /// <summary>
        /// Last modified by using user id
        /// </summary>
        public virtual int LastModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the last modified date
        /// </summary>
        public virtual DateTime LastModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the meta description of the page
        /// </summary>
        public virtual string MetaDescription { get; set; }
    
        /// <summary>
        /// Gets or sets the meta keywords of the page
        /// </summary>
        public virtual string MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta title of the page
        /// </summary>
        public virtual string MetaTitle { get; set; }

        /// <summary>
        /// Gets or sets the priority
        /// </summary>
        public virtual decimal Priority { get; set; }

        /// <summary>
        /// Gets or sets the title of the page
        /// </summary>
        public virtual string Title { get; set; }
    }
}