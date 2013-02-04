using System.ComponentModel.DataAnnotations;

namespace Gather.Web.Areas.Admin.Models.Setting
{
    public class SiteOwnerModel
    {
        [Required, Display(Name = "Enable SSL")]
        public bool MailEnableSSL { get; set; }

        [Display(Name = "Email Display Name")]
        public string MailFromDisplayName { get; set; }

        [Required, Display(Name = "Email Address")]
        public string MailFromEmail { get; set; }

        [Required, Display(Name = "Host")]
        public string MailHost { get; set; }

        [Display(Name = "Password")]
        public string MailPassword { get; set; }

        [Required, Display(Name = "Port")]
        public int MailPort { get; set; }

        [Required, Display(Name = "Use Default Credentials")]
        public bool MailUseDefaultCredentials { get; set; }

        [Display(Name = "Username")]
        public string MailUsername { get; set; }

        [Required, Display(Name = "Access Token")]
        public string TwitterAccessToken { get; set; }

        [Required, Display(Name = "Access Token Secret")]
        public string TwitterAccessTokenSecret { get; set; }
    }
}