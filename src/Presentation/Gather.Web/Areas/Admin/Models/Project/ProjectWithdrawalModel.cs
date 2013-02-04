using System;
using Gather.Web.Models.ModerationQueue;
using Gather.Web.Models.Project;

namespace Gather.Web.Areas.Admin.Models.Project
{
    public class ProjectWithdrawalModel
    {
        public int Id { get; set; }

        public int ModerationQueueId { get; set; }

        public ProjectModel Project { get; set; }
        
        public ModerationQueueModel ModerationQueue { get; set; }

        public String Reason { get; set; }

        public String VolunteersMessage { get; set; }

    }
}