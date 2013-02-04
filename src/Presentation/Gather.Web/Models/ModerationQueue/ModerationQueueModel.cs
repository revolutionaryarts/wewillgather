using System;
using Gather.Web.Areas.Admin.Models;
using Gather.Core.Domain.Common;
using Gather.Web.Models.User;

namespace Gather.Web.Models.ModerationQueue
{
    public class ModerationQueueModel : BaseModel
    {
        public int CreatedBy { get; set; }

        public UserModel CreatedByUser { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModeratedBy { get; set; }

        public DateTime? ModeratedDate { get; set; }

        public string Notes { get; set; }

        public int RequestTypeId { get; set; }

        public ModerationRequestType RequestType { get; set; }

        public string RequestTypeDescription { get; set; }

        public int StatusId { get; set; }

        public ModerationStatusType StatusType { get; set; }

        public string RelatedProject { get; set; }

        public string RelatedLocation { get; set; }

        public string RelatedDate { get; set; }
    }
}