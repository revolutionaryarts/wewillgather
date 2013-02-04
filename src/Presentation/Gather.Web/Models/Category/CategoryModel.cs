using System;
using Gather.Web.Areas.Admin.Models;
using System.ComponentModel.DataAnnotations;

namespace Gather.Web.Models.Category
{
    public class CategoryModel : BaseModel
    {
        public bool Active { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool Deleted { get; set; }

        public int LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public bool IsChecked { get; set; }

        [Required]
        [Display(Name = "Name", Description = "Maximum characters: 30")]
        public string Name { get; set; }
    }
}