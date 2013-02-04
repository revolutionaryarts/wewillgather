using System;
using Gather.Core.Domain.Users;

namespace Gather.Core.Domain.Api
{
    public class ApiAuthentication : BaseEntity
    {
        /// <summary>
        /// Gets or sets the Author of the comment
        /// </summary>
        public virtual User ApiUser { get; set; }

        /// <summary>
        /// Gets or sets the user creation date
        /// </summary>
        public virtual DateTime CreatedDate { get; set; }

        /// <summary>
        /// Last modified by using user id
        /// </summary>
        public virtual int? LastModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the last modified date
        /// </summary>
        public virtual DateTime? LastModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the name of the application using the API
        /// </summary>
        public virtual string NameOfApplication { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the web address using the API
        /// </summary>
        public virtual string WebsiteAddress { get; set; }

        /// <summary>
        /// Gets or sets the secrety key
        /// </summary>
        public virtual string SecretKey { get; set; }

        /// <summary>
        /// Gets or sets the access token
        /// </summary>
        public virtual string AccessToken { get; set; }

    }
}
