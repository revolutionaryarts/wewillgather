using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Gather.Core;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Projects;
using Gather.Services.Categories;
using Gather.Services.Locations;
using Gather.Services.Projects;
using Gather.Services.SuccessStories;
using Gather.Services.Tweets;
using Gather.Web.Extensions;
using Gather.Web.Models.Home;

namespace Gather.Web.Controllers
{
    public class HomeController : Controller
    {
        
        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly ILocationService _locationService;
        private readonly IProjectService _projectService;
        private readonly SiteSettings _siteSettings;
        private readonly ISuccessStoryService _successStoryService;
        private readonly ITweetService _tweetService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Constructors

        public HomeController(ICategoryService categoryService, ILocationService locationService, IProjectService projectService, SiteSettings siteSettings, ISuccessStoryService successStoryService, ITweetService tweetService, IWebHelper webHelper)
        {
            _categoryService = categoryService;
            _locationService = locationService;
            _projectService = projectService;
            _siteSettings = siteSettings;
            _successStoryService = successStoryService;
            _tweetService = tweetService;
            _webHelper = webHelper;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            int successStoryCount = 0;

            if (_siteSettings.HomePageSuccessStoryCount != "0")
            {
                int.TryParse(_siteSettings.HomePageSuccessStoryCount, out successStoryCount);

                if (successStoryCount < 1)
                    successStoryCount = 3;

                if (successStoryCount > 10)
                    successStoryCount = 10;
            }

            var availableCategories = _categoryService.GetAllCachedCategories().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            availableCategories.Insert(0, new SelectListItem { Text = "All types", Value = "0", Selected = true });

            var projects = _projectService.GetAllCachedProjects().ToList();
            var locations = _locationService.GetRegionLocations().Select(x => x.ToModel()).ToList();

            foreach (var location in locations)
                location.ProjectCount = projects.Count(p => p.Locations.Any(l => l.LocationId == location.Id));

            var model = new HomeModel
            {
                AvailableCategories = availableCategories,
                AvailableChildFriendly = _webHelper.GetAllEnumOptions<ProjectChildFriendly>().Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() }).ToList(),
                AvailableSearchRadius = _webHelper.GetAllEnumOptions<ProjectSearchRadius>().Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() }).ToList(),
                AvailableSearchStart = _webHelper.GetAllEnumOptions<ProjectSearchStart>().Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() }).ToList(),
                Locations = locations,
                Tweets = _tweetService.GetAllTweetsAboveId(false).Select(x => x.ToModel()).ToList(),
                SuccessStories = successStoryCount != 0 ? _successStoryService.GetAllSuccessStories(1, successStoryCount).Select(x => x.ToModel()).ToList() : null
            };

            return View(model);
        }

        [HttpPost]
        public string NewTweets(long id = -1)
        {
            if (id != -1)
            {
                var serializer = new JavaScriptSerializer();
                return serializer.Serialize(_tweetService.GetAllTweetsAboveId(true, id).Select(x => x.ToModel()).ToList());
            }
            return "";
        }
    
        #endregion

    }
}