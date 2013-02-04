using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Gather.Web.Models.Install
{
    public class StepOneModel
    {
        public StepOneModel()
        {
            AvailableAuthenticationMethods = new List<SelectListItem>();
        }

        public IList<SelectListItem> AvailableAuthenticationMethods { get; set; }

        [Display(Name = "Authentication Method")]
        public int DatabaseAuthenticationMethod { get; set; }

        [Required(ErrorMessage = "Please enter your database name."), Display(Name = "Database Name")]
        public string DatabaseName { get; set; }

        [Display(Name = "SQL Password")]
        public string DatabasePassword { get; set; }

        [Required(ErrorMessage = "Please enter your SQL Server name."), Display(Name = "SQL Server Name")]
        public string DatabaseServerName { get; set; }

        [Display(Name = "SQL Username")]
        public string DatabaseUsername { get; set; }
    }
}