using System;
using Gather.Core.Domain.Common;
using Gather.Web.Models.ModerationQueue;
using Gather.Web.Models.Project;

namespace Gather.Web.Areas.Admin.Models.Project
{
    public class ProjectModerationModel
    {
        public int Id { get; set; }

        public int ModerationQueueId { get; set; }

        public String Reason { get; set; }

        public ProjectModel Project { get; set; }
        
        public ModerationQueueModel ModerationQueue { get; set; }
        
        public int ComplaintId { get; set; }
        
        public ProjectComplaintType ComplaintType { get; set; }

        public String VolunteersMessage { get; set; }

    }
}