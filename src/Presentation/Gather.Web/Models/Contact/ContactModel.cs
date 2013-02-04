using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Gather.Core;
using Gather.Web.Models.User;

namespace Gather.Web.Models.Contact
{
    public class ContactModel
    {
        public List<UserModel> Admins { get; set; }

        [Required(ErrorMessage = "This is a required Field")]
        [StringLength(300, MinimumLength = 4)]
        public string Comments { get; set; }

        public bool DisplayError { get; set; }

        [Required(ErrorMessage = "This is a required Field")]
        [RegularExpression(CommonHelper.EMAIL_REGEX, ErrorMessage = "Invalid Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}