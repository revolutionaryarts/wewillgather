using System.Collections.Generic;
using Gather.Web.Models.ModerationQueue;

namespace Gather.Web.Areas.Admin.Models.ModerationQueue
{
    public class ModerationQueueListModel : BaseAdminListModel
    {
        public IList<ModerationQueueModel> ModerationQueue { get; set; }
    }
}