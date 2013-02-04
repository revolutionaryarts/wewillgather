using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Gather.Web.Areas.Admin.Models;
using Gather.Web.Models.Project;
using Gather.Web.Models.User;

namespace Gather.Web.Models.Comment
{
    public class CommentModel : BaseModel
    {
        public bool Active { get; set; }

        public UserModel Author { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool Deleted { get; set; }

        public CommentModel InResponseTo { get; set; }

        public int? LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public int ModerationRequestCount { get; set; }

        public ProjectModel Project { get; set; }

        public IList<CommentModel> Responses { get; set; }

        [Display(Name = "Comment")]
        public String UserComment { get; set; }
    }
}