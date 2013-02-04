using System;
using Gather.Core.Domain.Common;
using Gather.Web.Models.Comment;
using Gather.Web.Models.ModerationQueue;

namespace Gather.Web.Areas.Admin.Models.Project
{
    public class ProjectCommentModerationModel
    {
        public int Id { get; set; }

        public int ModerationQueueId { get; set; }

        public String Reason { get; set; }

        public CommentModel Comment { get; set; }
        
        public ModerationQueueModel ModerationQueue { get; set; }
        
        public int ComplaintId { get; set; }
        
        public ProjectCommentComplaintType ComplaintType { get; set; }

        public String UserMessage { get; set; }

        public string TypeDescription { get; set; }

    }
}