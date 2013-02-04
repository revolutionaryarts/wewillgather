using System;
using System.Collections.Generic;
using Gather.Web.Areas.Admin.Models;
using System.ComponentModel.DataAnnotations;
using Gather.Web.Models.Media;

namespace Gather.Web.Models.Page
{
    public class PageModel : BaseModel
    {
        public bool Active { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool Deleted { get; set; }

        [Display(Name = "File Title")]
        public string FileTitle { get; set; }

        public bool IsSystemPage { get; set; }

        public int LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public List<MediaModel> Media { get; set; }

        public MediaModel MediaItem { get; set; }

        [Display(Name = "Meta Description")]
        public string MetaDescription { get; set; }

        [Display(Name = "Meta Keywords")]
        public string MetaKeywords { get; set; }

        [Display(Name = "Page Title")]
        public string MetaTitle { get; set; }

        public decimal Priority { get; set; }

        [Required]
        public string Title { get; set; }
    }
}