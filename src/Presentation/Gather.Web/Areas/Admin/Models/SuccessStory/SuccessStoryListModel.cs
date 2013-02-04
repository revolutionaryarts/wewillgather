using System.Collections.Generic;
using Gather.Web.Models.SuccessStory;

namespace Gather.Web.Areas.Admin.Models.SuccessStory
{
    public class SuccessStoryListModel : BaseAdminListModel
    {
        public IList<SuccessStoryModel> SuccessStories { get; set; }
    }
}