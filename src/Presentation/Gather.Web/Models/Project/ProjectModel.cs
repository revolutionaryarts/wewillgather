using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Gather.Core.Domain.Common;
using Gather.Web.Models.Comment;
using Gather.Web.Models.User;
using Gather.Web.Models.Category;

namespace Gather.Web.Models.Project
{
    public class ProjectModel : BaseProjectModel
    {
        public ProjectModel()
        {
            AvailableCategories = new List<CategoryModel>();
            AvailableDisclosureLevels = new List<SelectListItem>();
            AvailableHours = new List<SelectListItem>();
            AvailableMinutes = new List<SelectListItem>();
            AvailableRecurrenceIntervals = new List<SelectListItem>();
            AvailableStatus = new List<SelectListItem>();
            AvailableUsers = new List<UserModel>();
            Comments = new List<CommentModel>();
        }

        public IList<CategoryModel> AvailableCategories { get; set; }

        public IList<SelectListItem> AvailableDisclosureLevels { get; set; }

        public IList<SelectListItem> AvailableHours { get; set; }

        public IList<SelectListItem> AvailableMinutes { get; set; }

        public IList<SelectListItem> AvailableRecurrenceIntervals { get; set; }  

        public IList<SelectListItem> AvailableStatus { get; set; }

        public IList<UserModel> AvailableUsers { get; set; }

        private bool _childFriendly = true;
        public bool ChildFriendly
        {
            get { return _childFriendly; }
            set { _childFriendly = value; }
        }

        public IList<CommentModel> Comments { get; set; }

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Want people to have your email?")]
        public int EmailDisclosureId { get; set; }

        public DisclosureLevel EmailDisclosureLevel { get; set; }

        public int EndHour { get; set; }

        public int EndMinutes { get; set; }

        public string Equipment { get; set; }

        [Display(Name = "Meeting at"), MaxLength(80, ErrorMessage = "The meeting at text exceeds the maximum length.")]
        public string GettingThere { get; set; }

        public bool IsRecurring { get; set; }

        public int? LastModeratorApprovalBy { get; set; }

        public DateTime? LastModeratorApprovalDate { get; set; }

        public int? LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public int LocationId { get; set; }

        [Required]
        public int Recurrence { get; set; }

        [Required]
        public int RecurrenceIntervalId { get; set; }

        public RecurrenceInterval RecurrenceInterval { get; set; }

        public string Skills { get; set; }

        public int StartHour { get; set; }

        public int StartMinutes { get; set; }

        public int StatusId { get; set; }

        public ProjectStatus Status { get; set; }

        public string Telephone { get; set; }

        [Display(Name = "Telephone Disclosure Level")]
        public int TelephoneDisclosureId { get; set; }

        public DisclosureLevel TelephoneDisclosureLevel { get; set; }

        public string VolunteerBenefits { get; set; }

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

        public int ModerationId { get; set; }

        public string ModerationComment { get; set; }

        public string ModerationNotes { get; set; }

        public int CurrentUserId { get; set; }

        public string LocationInput { get; set; }

        public bool ReloadFromLocalStorage { get; set; }
    }
}