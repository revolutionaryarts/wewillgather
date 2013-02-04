using System;
using System.Collections.Generic;
using System.Web;
using FluentValidation.Attributes;
using Gather.Web.Areas.Admin.Models;
using Gather.Web.Models.Media;
using Gather.Web.Models.User;
using System.ComponentModel.DataAnnotations;
using Gather.Web.Validators.SuccessStory;

namespace Gather.Web.Models.SuccessStory
{
    [Validator(typeof(SuccessStoryValidator))]
    public class SuccessStoryModel : BaseModel
    {

        public bool Active { get; set; }

        [Required]
        public string Article { get; set; }

        public UserModel Author { get; set; }

        public int AuthorId { get; set; }

        public List<UserModel> Authors { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool Deleted { get; set; }

        public int LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public MediaModel Media { get; set; }

        [Display(Name = "Meta Description")]
        public string MetaDescription { get; set; }

        [Display(Name = "Meta Keywords")]
        public string MetaKeywords { get; set; }

        [Display(Name = "Page Title")]
        public string MetaTitle { get; set; }

        public int ProjectId { get; set; }

        [Required]
        [Display(Name = "Short Summary")]
        public string ShortSummary { get; set; }

        [Required]
        public string Title { get; set; }
        
        public string SeName { get; set; }

        [Display(Name = "Image (Minimum 1000x640px)")]
        public HttpPostedFileBase UploadedFile { get; set; }
    }
}