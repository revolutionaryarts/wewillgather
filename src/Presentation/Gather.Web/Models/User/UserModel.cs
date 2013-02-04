using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Web;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Gather.Core.Domain.Common;
using Gather.Web.Areas.Admin.Models;
using Gather.Web.Validators.User;

namespace Gather.Web.Models.User
{
    [Validator(typeof(UserValidator))]
    public class UserModel : BaseModel
    {
        public UserModel()
        {
            AvailableDisclosureLevels = new List<SelectListItem>();
            UserRoles = new List<UserRoleModel>();
        }

        public IList<UserRoleModel> AvailableUserRoles { get; set; }

        public IList<SelectListItem> AvailableDisclosureLevels { get; set; }

        public bool Active { get; set; }

        [Display(Name = "Contact Us Bio")]
        public string ContactUsBio { get; set; }

        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Please enter a display name")]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        public string Email { get; set; }

        [Display(Name = "Email Disclosure Level")]
        public int EmailDisclosureId { get; set; }

        public DisclosureLevel EmailDisclosureLevel { get; set; }

        public string FacebookDisplayName { get; set; }

        [Display(Name = "Facebook Profile Id")]
        public string FacebookProfile { get; set; }

        public DateTime LastLoginDate { get; set; }

        public string PluralDisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(DisplayName))
                    return null;
                var name = DisplayName + "'";
                if (DisplayName.Substring(DisplayName.Length - 1, 1) == "s")
                    return name;
                return name + "s";
            }
        }

        public int PrimaryAuthMethodId { get; set; }

        public AuthenticationMethod PrimaryAuthMethod { get; set; }

        private string _profilePicture;

        public string ProfilePicture
        {
            get
            {
                if (File.Exists(HttpContext.Current.Server.MapPath("~/uploads/profile/") + _profilePicture))
                    return "/uploads/profile/" + _profilePicture;
                return "/content/images/placeholder.jpg";
            }
            set { _profilePicture = value; }
        }

        [Display(Name = "Show on Contact Us")]
        public bool ShowOnContactUs { get; set; }

        public string Telephone { get; set; }

        [Display(Name = "Phone Disclosure Level")]
        public int TelephoneDisclosureId { get; set; }

        public DisclosureLevel TelephoneDisclosureLevel { get; set; }

        public string TwitterDisplayName { get; set; }

        [Display(Name = "Twitter Profile Id")]
        public string TwitterProfile { get; set; }

        [Required, MaxLength(50), Display(Name = "Username")]
        public string UserName { get; set; }

        public IList<UserRoleModel> UserRoles { get; set; }

        private string _website;

        public string Website
        {
            get
            {
                if (!string.IsNullOrEmpty(_website))
                    return new UriBuilder(_website).Uri.ToString();
                return _website;
            }
            set { _website = value; }
        }

        [Display(Name = "Website Disclosure Level")]
        public int WebsiteDisclosureId { get; set; }

        public DisclosureLevel WebsiteDisclosureLevel { get; set; }
    }
}