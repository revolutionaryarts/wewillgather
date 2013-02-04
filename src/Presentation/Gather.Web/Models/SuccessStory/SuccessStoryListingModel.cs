using System.Collections.Generic;

namespace Gather.Web.Models.SuccessStory
{
    public class SuccessStoryListingModel : BasePageModel
    {        
        public IList<SuccessStoryModel> SuccessStories { get; set; }
    }
}