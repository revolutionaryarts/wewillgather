using System.Collections.Generic;
using System.Web.Mvc;
using Gather.Web.Areas.Admin.Models;
using Gather.Web.Models.Location;
using Gather.Web.Models.SuccessStory;
using Gather.Web.Models.Tweet;

namespace Gather.Web.Models.Home
{
    public class HomeModel : BaseModel
    {
        public HomeModel()
        {
            AvailableCategories = new List<SelectListItem>();
            AvailableChildFriendly = new List<SelectListItem>();
            AvailableSearchRadius = new List<SelectListItem>();
            AvailableSearchStart = new List<SelectListItem>();
            Locations = new List<LocationModel>();
        }

        public IList<SelectListItem> AvailableCategories { get; set; }

        public IList<SelectListItem> AvailableChildFriendly { get; set; }

        public IList<SelectListItem> AvailableSearchRadius { get; set; }

        public IList<SelectListItem> AvailableSearchStart { get; set; }

        public IList<LocationModel> Locations { get; set; }

        public IList<TweetModel> Tweets { get; set; }

        public IList<SuccessStoryModel> SuccessStories { get; set; }
    }
}