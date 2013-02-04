using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Gather.Web.Areas.Admin.Models;
using Gather.Web.Models.Category;
using Gather.Web.Models.User;
using FluentValidation.Attributes;
using Gather.Web.Validators.BaseProject;

namespace Gather.Web.Models.Project
{

    [Validator(typeof(BaseProjectValidator))]
    public class BaseProjectModel : BaseModel
    {
        public BaseProjectModel()
        {
            Categories = new List<CategoryModel>();
            Locations = new List<ProjectLocationModel>();
            Owners = new List<UserModel>();
            Volunteers = new List<UserModel>();
        }

        public IList<CategoryModel> Categories { get; set; }

        public UserModel CreatedBy { get; set; }

        public int? CreatedById { get; set; }

        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Please enter an end date.")]
        [Display(Name = "end date")]
        public DateTime? EndDate { get; set; }

        public decimal Latitude { get; set; }

        public IList<ProjectLocationModel> Locations { get; set; }

        public decimal Longitude { get; set; }

        [Required(ErrorMessage = "Please enter a title.")]
        [Display(Name = "Title")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter the number of good people you need.")]
        [Display(Name = "required good people")]
        [Range(1, int.MaxValue, ErrorMessage = "The number of required good people must be greater than 0.")]
        public int NumberOfVolunteers { get; set; }

        public string Objective { get; set; }

        public IList<UserModel> Owners { get; set; }

        public int RemainingNumberOfVolunteers { get; set; }

        public string SeoName { get; set; }

        [Required(ErrorMessage = "Please enter a start date.")]
        [Display(Name = "start date")]        
        public DateTime? StartDate { get; set; }

        public IList<UserModel> Volunteers { get; set; }
    }
}