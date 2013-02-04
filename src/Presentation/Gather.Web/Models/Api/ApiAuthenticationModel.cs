using System;
using System.ComponentModel.DataAnnotations;
using Gather.Web.Areas.Admin.Models;
using Gather.Web.Models.User;

namespace Gather.Web.Models.Api
{
    public class ApiAuthenticationModel : BaseModel
    {
        /// <summary>
        /// Gets or Sets the active status
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the deleted status
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the Author of the comment
        /// </summary>
        public UserModel ApiUser { get; set; }

        /// <summary>
        /// Gets or sets the user creation date
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Last modified by using user id
        /// </summary>
        public int? LastModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the last modified date
        /// </summary>
        public DateTime? LastModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the name of the application using the API
        /// </summary>
        [Required]
        [Display(Name = "Website Name")]
        public string NameOfApplication { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [Required]
        [Display(Name = "Reason for request")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the web address using the API
        /// </summary>
        [Required]
        [Display(Name = "Website URL")]
        public string WebsiteAddress { get; set; }

        /// <summary>
        /// Gets or sets the secrety key
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Gets or sets the access token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the secrety key
        /// </summary>
        public bool ShowToken { get; set; }

    }
}