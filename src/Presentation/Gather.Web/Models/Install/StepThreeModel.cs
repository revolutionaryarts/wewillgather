using System.ComponentModel.DataAnnotations;

namespace Gather.Web.Models.Install
{
    public class StepThreeModel
    {
        public string AuthenticationMethod { get; set; }

        [Required(ErrorMessage = "Please enter your display name."), Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Required(ErrorMessage = "Please enter your email address."), Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Email Display Name")]
        public string MailFromDisplayName { get; set; }

        [Required(ErrorMessage = "Please enter the email address you want site emails to be sent from."), Display(Name = "Email Address")]
        public string MailFromEmail { get; set; }

        [Required(ErrorMessage = "Please enter your mail server host."), Display(Name = "Host")]
        public string MailHost { get; set; }

        [Display(Name = "Password")]
        public string MailPassword { get; set; }

        [Required(ErrorMessage = "Please enter your mail server port."), Display(Name = "Port")]
        public int MailPort { get; set; }

        [Display(Name = "Username")]
        public string MailUsername { get; set; }

        [Required, Display(Name = "Access Token")]
        public string TwitterAccessToken { get; set; }

        [Required, Display(Name = "Access Token Secret")]
        public string TwitterAccessTokenSecret { get; set; }

        [Required(ErrorMessage = "Please enter a username."), Display(Name = "Username")]
        public string UserName { get; set; }
    }
}