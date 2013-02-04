using System.Linq;
using System.Web.Mvc;
using Gather.Core.Domain.Locations;
using Gather.Services.Locations;
using Gather.Services.Pages;
using Gather.Services.Projects;
using Gather.Web.Extensions;
using Gather.Web.Models.SiteMap;

namespace Gather.Web.Controllers
{
    public class SiteMapController : BaseController
    {

        #region Fields

        private readonly ILocationService _locationService;
        private readonly IPageService _pageService;
        private readonly IProjectService _projectService;

        #endregion

        #region Constructor

        public SiteMapController(ILocationService locationService, IPageService pageService, IProjectService projectService)
        {
            _locationService = locationService;
            _pageService = pageService;
            _projectService = projectService;
        }

        #endregion

        #region Methods

        public ActionResult SiteMap()
        {
            return View(PopulateSiteMap(null));
        }

        public ActionResult SiteMapLocation(string locationSeoName)
        {
            return View("SiteMap", PopulateSiteMap(_locationService.GetLocationBySeoName(locationSeoName)));
        }

        private SiteMapModel PopulateSiteMap(Location location)
        {
            var model = new SiteMapModel
            {
                Locations = _locationService.GetAllChildLocations(location == null ? 0 : location.Id).OrderBy(x => x.Name).Select(x => x.ToModel()).ToList(),
                Pages = _pageService.GetAllPages(1, -1).Select(x => x.ToModel()).ToList(),
                TitleName = location == null ? "Site Map" : location.Name + " Site Map",
                MetaTitle = location == null ? "Sitemap - #WeWillGather" : "Sitemap for " + location.Name + " | Voluntary projects in " + location.Name,
                MetaDescription = location == null ? "Use the #WeWillGather sitemap to quickly track down the community projects that are taking place in your area." : "",
                NoIndex = location != null
            };

            AddHomeBreadcrumb();
            Location parentLocation = null;
            if (location != null)
            {
                parentLocation = location.ParentLocation;
                AddBreadcrumb(model.TitleName, Url.RouteUrl("SiteMapLocation", new { locationSeoName = location.SeoName }));
            }

            while (parentLocation != null)
            {
                AddBreadcrumb(parentLocation.Name + " Site Map", Url.RouteUrl("SiteMapLocation", new { locationSeoName = parentLocation.SeoName }), 1);
                parentLocation = parentLocation.ParentLocation;
            }

            AddBreadcrumb("Site Map", Url.RouteUrl("SiteMap"), 1);

            var projects = _projectService.GetAllProjectsByLocation(location);

            model.BaseLocation = model.Locations.Count == 0;

            if (!model.BaseLocation)
            {
                foreach (var loc in model.Locations)
                    loc.ProjectCount = projects.Count(p => p.Locations.Any(l => l.LocationId == loc.Id));
            }
            else
            {
                 model.Projects = projects.Select(x => x.ToModel()).ToList();
            }

            return model;
        }

        #endregion

    }
}