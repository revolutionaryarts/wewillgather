using System;
using System.Collections.Generic;
using Gather.Core.Domain.Common;

namespace Gather.Core.Domain.Users
{
    public class User : BaseEntity
    {
        private ICollection<UserRole> _userRoles;

        public User()
        {
            UserGuid = Guid.NewGuid();
        }

        /// <summary>
        /// Gets or sets the bio used on the 'contact us' page
        /// </summary>
        public virtual string ContactUsBio { get; set; }

        /// <summary>
        /// Gets or sets the user creation date
        /// </summary>
        public virtual DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the user display name
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the user email
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets or sets the email disclosure identifier
        /// </summary>
        public virtual int EmailDisclosureId { get; set; }

        /// <summary>
        /// Gets or sets the email disclosure level
        /// </summary>
        public virtual DisclosureLevel EmailDisclosureLevel
        {
            get { return (DisclosureLevel)EmailDisclosureId; }
            set { EmailDisclosureId = (int)value; }
        }

        /// <summary>
        /// Gets or sets the facebook access token
        /// </summary>
        public virtual string FacebookAccessToken { get; set; }
    
        /// <summary>
        /// Gets or sets the user Facebook profile display name
        /// </summary>
        public virtual string FacebookDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the user Facebook profile Id
        /// </summary>
        public virtual string FacebookProfile { get; set; }

        /// <summary>
        /// Gets or sets the last login date
        /// </summary>
        public virtual DateTime LastLoginDate { get; set; }

        /// <summary>
        /// Gets or sets the primary authentication method identifier
        /// </summary>
        public virtual int PrimaryAuthMethodId { get; set; }

        /// <summary>
        /// Gets or sets the primary authentication method
        /// </summary>
        public virtual AuthenticationMethod PrimaryAuthMethod
        {
            get { return (AuthenticationMethod) PrimaryAuthMethodId; }
            set { PrimaryAuthMethodId = (int) value; }
        }

        /// <summary>
        /// Gets or sets the profile picture image name
        /// </summary>
        public string ProfilePicture { get; set; }

        /// <summary>
        /// Gets or sets the 'show on contact us' flag
        /// </summary>
        public virtual bool ShowOnContactUs { get; set; }

        /// <summary>
        /// Gets or sets the user's telephone
        /// </summary>
        public virtual string Telephone { get; set; }

        /// <summary>
        /// Gets or sets the telephone disclosure identifier
        /// </summary>
        public virtual int TelephoneDisclosureId { get; set; }

        /// <summary>
        /// Gets or sets the telephone disclosure level
        /// </summary>
        public virtual DisclosureLevel TelephoneDisclosureLevel
        {
            get { return (DisclosureLevel)TelephoneDisclosureId; }
            set { TelephoneDisclosureId = (int)value; }
        }

        /// <summary>
        /// Gets or sets the Twitter access token secret
        /// </summary>
        public virtual string TwitterAccessSecret { get; set; }
    
        /// <summary>
        /// Gets or sets the Twitter access token
        /// </summary>
        public virtual string TwitterAccessToken { get; set; }
    
        /// <summary>
        /// Gets or sets the user Twitter profile display name
        /// </summary>
        public virtual string TwitterDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the user Twitter profile Id
        /// </summary>
        public virtual string TwitterProfile { get; set; }

        /// <summary>
        /// Gets or sets the user Guid
        /// </summary>
        public virtual Guid UserGuid { get; set; }

        /// <summary>
        /// Gets or sets the username
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user roles
        /// </summary>
        public virtual ICollection<UserRole> UserRoles
        {
            get { return _userRoles ?? (_userRoles = new List<UserRole>()); }
            set { _userRoles = value; }
        }

        /// <summary>
        /// Gets or sets the user's website url
        /// </summary>
        public virtual string Website { get; set; }

        /// <summary>
        /// Gets or sets the website disclosure identifier
        /// </summary>
        public virtual int WebsiteDisclosureId { get; set; }

        /// <summary>
        /// Gets or sets the website disclosure level
        /// </summary>
        public virtual DisclosureLevel WebsiteDisclosureLevel
        {
            get { return (DisclosureLevel)WebsiteDisclosureId; }
            set { WebsiteDisclosureId = (int)value; }
        }
    }
}