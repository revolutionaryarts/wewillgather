using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Gather.Web.Models.User;

namespace Gather.Web.Models.Project
{
    public class ProjectDetailModel
    {
        public ProjectDetailModel()
        {
            CommentComplaintTypes = new List<SelectListItem>();
            ProjectComplaintTypes = new List<SelectListItem>();
        }

        public IList<SelectListItem> CommentComplaintTypes { get; set; }

        public UserModel CurrentUser { get; set; }

        public ProjectModel Project { get; set; }

        public IList<SelectListItem> ProjectComplaintTypes { get; set; }

        public Boolean ProjectCommentTracking { get; set; }

        public string MetaDescription { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaTitle { get; set; }
    }
}