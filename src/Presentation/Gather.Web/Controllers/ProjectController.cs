using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml;
using Gather.Core;
using Gather.Core.ActionResults;
using Gather.Core.Domain.Comments;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Locations;
using Gather.Core.Domain.Projects;
using Gather.Core.Seo;
using Gather.Services.Categories;
using Gather.Services.Comments;
using Gather.Services.Geolocation;
using Gather.Services.Locations;
using Gather.Services.Logging;
using Gather.Services.ModerationQueues;
using Gather.Services.Projects;
using Gather.Services.Users;
using Gather.Web.Extensions;
using Gather.Web.Framework.Controllers;
using Gather.Web.Framework.UI;
using Gather.Web.Models.Category;
using Gather.Web.Models.Location;
using Gather.Web.Models.Project;
using Gather.Web.Models.User;
using Gather.Services.Profanities;

namespace Gather.Web.Controllers
{
    public class ProjectController : BaseController
    {

        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly ICommentService _commentService;
        private readonly IGeolocationService _geolocationService;
        private readonly ILocationService _locationService;
        private readonly ILogService _logService;
        private readonly IModerationQueueService _moderationQueueService;
        private readonly IProfanityService _profanityService;
        private readonly IProjectService _projectService;
        private readonly SiteSettings _siteSettings;
        private readonly IUserService _userService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        private const string PROJECT_COMMENT_ADDED = "ProjectCommentAdded";
        private const int PROJECT_MAX_LENGTH = 80;

        private int PerPage
        {
            get
            {
                int perPage;
                if (!int.TryParse(_siteSettings.ProjectListingPageSize, out perPage))
                    perPage = 15;
                return perPage;
            }
        }

        private Url _baseLink;
        private Url _rssLink;

        private decimal _mapLatitude = (decimal)55.0632;
        private decimal _mapLongitude = (decimal)-4.7241;
        private int _mapZoomLevel = 5;
        private string _metaDescription;
        private string _metaTitle;
        private IList<BaseProject> _projects = new List<BaseProject>();
        private LocationFilterModel _locationFilters = new LocationFilterModel();
        private Location _location;

        private string _locationSeoName, _parentSeoName, _query;
        private int _childFriendly, _radius, _sortDirection, _sortType, _start;
        private List<int> _categories;

        #endregion

        #region Constructors

        public ProjectController(ICategoryService categoryService, ICommentService commentService, IGeolocationService geolocationService, ILocationService locationService, ILogService logService, IModerationQueueService moderationQueueService, IProjectService projectService, IProfanityService profanityService, SiteSettings siteSettings, IUserService userService, IWebHelper webHelper, IWorkContext workContext)
        {
            _categoryService = categoryService;
            _commentService = commentService;
            _geolocationService = geolocationService;
            _locationService = locationService;
            _logService = logService;
            _moderationQueueService = moderationQueueService;
            _profanityService = profanityService;
            _projectService = projectService;
            _siteSettings = siteSettings;
            _userService = userService;
            _webHelper = webHelper;
            _workContext = workContext;
        }

        #endregion
        
        #region Utilities

        private void SetPageLink(bool rss = false)
        {
            Url link = GetPageLink(rss: rss);
            if (rss)
                _rssLink = link;
            else
                _baseLink = link;
        }

        private Url GetPageLink(string locationSeoNameOverride = null, string parentSeoNameOverride = null, bool rss = false)
        {
            Url link;

            string locationSeoName = locationSeoNameOverride ?? _locationSeoName;
            string parentSeoName = parentSeoNameOverride ?? _parentSeoName;

            if (!string.IsNullOrEmpty(locationSeoName))
            {
                if (!string.IsNullOrEmpty(parentSeoName))
                {
                    link = new Url(Url.RouteUrl(rss ? "ProjectListingLocationWithParentRss" : "ProjectListingLocationWithParent", new { locationSeoName, parentSeoName }));
                }
                else
                {
                    link = new Url(Url.RouteUrl(rss ? "ProjectListingLocationRss" : "ProjectListingLocation", new { locationSeoName }));
                }
            }
            else
            {
                link = new Url(Url.RouteUrl(rss ? "ProjectListingRss" : "ProjectListing"));

                if (Page > 1)
                    link.AddParam("page", Page);
            }

            if (!string.IsNullOrEmpty(_query))
                link.AddParam("query", Server.UrlEncode(_query));

            if (_radius != (int)ProjectSearchRadius.FiveMiles)
                link.AddParam("radius", _radius);

            if (_categories != null && _categories.Count > 0)
                link.AddParam("categories", string.Join(",", _categories.Where(x => x != 0).Select(n => n.ToString()).ToArray()));

            if (_childFriendly != (int)ProjectChildFriendly.NoPreference)
                link.AddParam("childFriendly", _childFriendly);

            if (_start != (int)ProjectSearchStart.Whenever)
                link.AddParam("start", _start);

            if (_sortType != (int)ProjectSortType.StartDate)
                link.AddParam("sortType", _sortType);

            if (_sortDirection != (int)ProjectSortDirection.Ascending)
                link.AddParam("sortDirection", _sortDirection);

            return link;
        }

        private int CalculateNextResultCount(int page)
        {
            int count = PerPage;
            int pageCount = (int)Math.Ceiling(_projects.Count / (double)PerPage);

            if (pageCount <= page)
                count = 0;
            else
                if ((page + 1) * PerPage > _projects.Count)
                    count = PerPage - (((page + 1) * PerPage) - _projects.Count);

            return count;
        }

        private ProjectDetailModel PrepareProjectDetailModel(Project project, bool prepareComments = true)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            var model = new ProjectDetailModel
            {
                CurrentUser = _workContext.CurrentUser.ToModel(),
                Project = project.ToModel()
            };

            if (prepareComments)
            {
                var comments = project.Comments.OrderBy(x => x.CreatedDate);
                foreach (var comment in comments)
                    model.Project.Comments.Add(comment.ToModel());
            }

            foreach (var complaintType in _moderationQueueService.GetAllCommentComplaintTypes())
                model.CommentComplaintTypes.Add(new SelectListItem { Text = complaintType.Value, Value = complaintType.Key.ToString() });

            foreach (var complaintType in _moderationQueueService.GetAllProjectComplaintTypes())
                model.ProjectComplaintTypes.Add(new SelectListItem { Text = complaintType.Value, Value = complaintType.Key.ToString() });

            string descriptionDate = "";
            if (model.Project.StartDate != null)
            {
                string descriptionTime = _webHelper.DateTimeFormat("H:mmTT", model.Project.StartDate.Value);
                if (model.Project.EndDate != null && model.Project.EndDate.Value.Subtract(model.Project.StartDate.Value).Days < 1)
                    descriptionTime += " to " + _webHelper.DateTimeFormat("H:mmTT", model.Project.EndDate.Value);
                descriptionDate = string.Format(" on {0} from {1}", _webHelper.DateTimeFormat("d~ MMMM", model.Project.StartDate.Value), descriptionTime);
            }

            model.MetaTitle = model.Project.Name + " - #WeWillGather";
            model.MetaDescription = string.Format("Volunteer now and join the '{0}' project taking place{1}. Organised with #WeWillGather.", model.Project.Name, descriptionDate);

            return model;
        }

        private List<CategoryFilterModel> PrepareCategoryFilters(IList<BaseProject> projects)
        {
            var categories = _categoryService.GetAllCachedCategories();
            var categoryFilters = new List<CategoryFilterModel>();
            var metaCategories = new List<string>();

            foreach (var category in categories.Where(c => projects.Any(p => p.Categories.Any(x => x.Id == c.Id))))
            {
                bool active = _categories != null && _categories.Contains(category.Id);

                if (active)
                    metaCategories.Add(category.Name);

                var addLink = new Url(_baseLink);
                addLink.AddParam("categories", category.Id);
                addLink.RemoveParam("page");

                var removeLink = new Url(_baseLink);
                removeLink.RemoveParam("categories", category.Id);
                removeLink.RemoveParam("page");

                categoryFilters.Add(new CategoryFilterModel
                {
                    AddLink = addLink.ToString(),
                    Id = category.Id,
                    IsChecked = active,
                    IsEnabled = projects.Any(p => p.Categories.Any(x => x.Id == category.Id)),
                    Name = category.Name,
                    ProjectCount = projects.Count(p => p.Categories.Any(x => x.Id == category.Id)),
                    RemoveLink = removeLink.ToString()
                });
            }

            if (_categories != null && _categories.Count > 0)
            {
                string metaCategoryString = "";
                for(var i = 0; i < metaCategories.Count; i++)
                    metaCategoryString += (metaCategories.Count > 1 && i == metaCategories.Count - 1 ? " and " : ", ") + metaCategories[i];
                _metaTitle = string.Format(_metaTitle, metaCategoryString.TrimStart(',') + " Volunteer");
            }

            return categoryFilters;
        }

        private LocationFilterModel PrepareLocationFilters(IList<BaseProject> projects, Location location = null)
        {
            if (location == null)
            {
                var topLocations = _locationService.GetAllChildLocations();
                LocationFilterModel allRegionFilter;

                if (topLocations.Count > 1)
                {
                    allRegionFilter = new LocationFilterModel { Name = "All Regions", ProjectCount = projects.Count };
                    foreach (var topLocation in topLocations)
                        allRegionFilter.ChildrenLocations.Add(PrepareLocationFilters(projects.Where(p => p.Locations.Any(l => l.LocationId == topLocation.Id)).ToList(), topLocation));
                }
                else
                {
                    allRegionFilter = PrepareLocationFilters(projects, topLocations.First());
                }

                return allRegionFilter;
            }

            var locationFilter = location.ToFilterModel();
            locationFilter.Link = GetPageLink(location.SeoName, location.ParentLocation != null ? location.ParentLocation.SeoName : null).ToString();
            locationFilter.ProjectCount = projects.Count(p => p.Locations.Any(l => l.LocationId == location.Id));

            foreach (var child in location.ChildLocations)
            {
                int projectCount = projects.Count(p => p.Locations.Any(l => l.LocationId == child.Id));
                if (projectCount > 0)
                {
                    var childFilter = child.ToFilterModel();
                    childFilter.Link = GetPageLink(child.SeoName, child.ParentLocation != null ? child.ParentLocation.SeoName : null).ToString();
                    childFilter.ProjectCount = projectCount;
                    locationFilter.ChildrenLocations.Add(childFilter);
                }
            }

            var parentLocation = location.ParentLocation;
            while (parentLocation != null)
            {
                var parentLocationFilter = parentLocation.ToFilterModel();
                parentLocationFilter.ChildrenLocations.Add(locationFilter);
                parentLocationFilter.Link = GetPageLink(parentLocation.SeoName, parentLocation.ParentLocation != null ? parentLocation.ParentLocation.SeoName :  null).ToString();
                parentLocationFilter.ProjectCount = projects.Count(p => p.Locations.Any(l => l.LocationId == parentLocation.Id));

                locationFilter = parentLocationFilter;
                parentLocation = parentLocation.ParentLocation;
            }

            return locationFilter;
        }

        private string PrepareMapMarkerArray(IEnumerable<BaseProject> projects)
        {
            var sb = new StringBuilder("[");
            foreach (var project in projects)
            {
                sb.Append("{");

                var location = project.Locations.First(l => l.Primary).Location;

                sb.Append("\"Latitude\":\"" + project.Latitude + "\",");
                sb.Append("\"Longitude\":\"" + project.Longitude + "\",");
                sb.Append("\"Name\":\"" + project.Name + "\",");
                sb.Append("\"StartDate\":\"" + (project.StartDate ?? DateTime.Now).ToString("dd MMM") + "\",");
                sb.Append("\"StartTime\":\"" + (project.StartDate ?? DateTime.Now).ToString("HH:mm") + "\",");
                sb.Append("\"Location\":\"" + location.Name + "\",");
                sb.Append("\"URL\":\"" + Url.RouteUrl("ProjectDetail", new { locationSeoName = location.SeoName, seoName = project.GetSeoName(), id = project.Id }) + "\",");
                sb.Append("\"Categories\":[");
                sb.Append(project.Categories.Aggregate("", (current, category) => current + " \"" + category.Name + "\",").TrimEnd(','));
                sb.Append("]");

                sb.Append("},");
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append("]");

            return sb.ToString();
        }

        private void PrepareListingResults(int? locationId = null, bool buildLocationFilters = true)
        {
            // Build non-javascript refinement links
            SetPageLink();
            SetPageLink(true);

            var sortTypeEnum = (ProjectSortType) _sortType;
            var sortDirectionEnum = (ProjectSortDirection) _sortDirection;

            // Build the category meta title
            if (_categories != null && _categories.Count > 0)
                _metaTitle = "Volunteer to do a good thing | {0} projects";

            // Start by getting all the projects and locations 
            // based on how you're entering the page
            if (!string.IsNullOrEmpty(_query))
            {
                // Work out how far we should zoom in
                switch (_radius)
                {
                    case (int)ProjectSearchRadius.FiveMiles:
                        _mapZoomLevel = 12;
                        break;
                    case (int)ProjectSearchRadius.TenMiles:
                        _mapZoomLevel = 11;
                        break;
                    case (int)ProjectSearchRadius.FifteenMiles:
                        _mapZoomLevel = 10;
                        break;
                    case (int)ProjectSearchRadius.FiftyMiles:
                        _mapZoomLevel = 8;
                        break;
                }

                // If we have a search query, the user must have come from the search form
                // Get the lat/lng of their search query
                _geolocationService.GetLatLng(_query, out _mapLatitude, out _mapLongitude);

                // Get all the projects within a given radius of the search query
                _projects = _projectService.GetAllProjectsByCoords(_mapLatitude, _mapLongitude, _categories, (ProjectSearchRadius)_radius, (ProjectSearchStart)_start, (ProjectChildFriendly)_childFriendly);
                var locationProjects = _projects;

                // If we have an additional location filter, remove projects not in the location
                if ((locationId != null && locationId > 0) || !string.IsNullOrEmpty(_locationSeoName))
                {
                    if (locationId != null && locationId > 0)
                        _location = _locationService.GetLocationById((int)locationId);
                    else
                        _location = _locationService.GetLocationBySeoName(_locationSeoName, _parentSeoName);

                    _projects = _projects.Where(x => x.Locations.Any(l => l.LocationId == _location.Id)).ToList();

                    int parentCount = 0;
                    var parentLocation = _location.ParentLocation;
                    while (parentLocation != null)
                    {
                        parentCount++;
                        parentLocation = parentLocation.ParentLocation;
                    }

                    _mapLatitude = _location.Latitude;
                    _mapLongitude = _location.Longitude;
                    _mapZoomLevel = 7 + (int)Math.Round(parentCount * 1.5);
                }

                // Build location filters
                if (buildLocationFilters && _projects.Count > 0)
                {
                    var topLocation = _projects[0].Locations.ToList().FirstOrDefault(x => _projects.All(y => y.Locations.Any(z => z.LocationId == x.LocationId)));
                    if (topLocation != null)
                    {
                        _locationFilters = PrepareLocationFilters(locationProjects, topLocation.Location);
                    }
                }
            }
            else if ((locationId != null && locationId > 0) || !string.IsNullOrEmpty(_locationSeoName))
            {
                // If we are on a location project listing page
                // Get the location entity based on the location seo name in the URL
                if (locationId != null && locationId > 0)
                    _location = _locationService.GetLocationById((int)locationId);
                else
                    _location = _locationService.GetLocationBySeoName(_locationSeoName, _parentSeoName);

                // If we couldn't find a location matching the seo name, redirect to the main project listing page
                if (_location == null)
                {
                    var httpContextWrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
                    var urlHelper = new UrlHelper(new RequestContext(httpContextWrapper, new RouteData()));
                    System.Web.HttpContext.Current.Response.Redirect(urlHelper.RouteUrl("ProjectListing"), true);
                    return;
                }

                // Build the breadcrumbs based on the location
                var parentLocation = _location.ParentLocation;
                int parentCount = 0;

                // Build the breadcrumb hierarchy
                while (parentLocation != null)
                {
                    parentCount++;
                    AddBreadcrumb(parentLocation.Name, Url.RouteUrl("ProjectListingLocation", new { locationSeoName = parentLocation.SeoName }), 0);
                    parentLocation = parentLocation.ParentLocation;
                }

                AddBreadcrumb(_location.Name, Url.RouteUrl("ProjectListingLocation", new { locationSeoName = _location.SeoName }));

                // Change the map lat/lng and zoom based on the current location and number of parents
                _mapLatitude = _location.Latitude;
                _mapLongitude = _location.Longitude;
                _mapZoomLevel = 7 + (int)Math.Round(parentCount * 1.5);

                // Check if we need to build the location filters
                if (buildLocationFilters)
                {
                    // Get all the projects, regardless of location
                    _projects = _projectService.GetAllProjectsByLocation(null, _categories, sortTypeEnum, sortDirectionEnum);

                    // Build all the location filters, passing across the project list so we can populate location counts
                    _locationFilters = PrepareLocationFilters(_projects, _location);
                }
                else
                {
                    // Get all the projects, regardless of location
                    _projects = _projectService.GetAllProjectsByLocation(_location, _categories, sortTypeEnum, sortDirectionEnum);
                }

                // Filter the projects down to the location we need for presentation
                _projects = _projects.Where(p => p.Locations.Any(l => l.LocationId == _location.Id)).ToList();

                // Build the meta title
                string projectCountTitle = _projects.Count > 0 ? " - " + _projects.Count + " Projects" : "";
                _metaTitle = string.Format("{0} Opportunities in {1}{2} - #WeWillGather", _categories != null && _categories.Count > 0 ? "{0} " : "Volunteering ", _location.Name, projectCountTitle);
            }
            else
            {
                // We must be on the main projecting listing page
                // Get all projects, regardless of location
                _projects = _projectService.GetAllProjectsByLocation(null, _categories, sortTypeEnum, sortDirectionEnum);

                // Check if we need to build the location filters
                if (buildLocationFilters)
                    _locationFilters = PrepareLocationFilters(_projects);
            }
        }

        private string ReplaceString(string str, string oldValue, string newValue, StringComparison comparison)
        {
            StringBuilder sb = new StringBuilder();
            bool frontValid = false;
            bool backValid = false;
            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                if (index > 0)
                {
                    if (char.IsLetter(str.Substring(index - 1, 1)[0]))
                    {
                        frontValid = true;
                    }
                }
                if (index + oldValue.Length < str.Length)
                {
                    if (char.IsLetter(str.Substring(index + oldValue.Length, 1)[0]))
                    {
                        backValid = true;
                    }
                }

                if (!frontValid && !backValid)
                {
                    sb.Append(str.Substring(previousIndex, index - previousIndex));
                    sb.Append(newValue);
                    index += oldValue.Length;
                }
                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }

        private string StripProfanity(string text)
        {
            string temp = _profanityService.GetAll().Aggregate(text, (current, profanity) => ReplaceString(current, profanity, new string('*', profanity.Length), StringComparison.CurrentCultureIgnoreCase));
            return temp;
        }

        #endregion

        #region Methods

        #region Add Project

        [Authorize]
        public ActionResult Add(int id)
        {
            var projectModel = new ProjectModel();

            if (id > 0)
            {
                var model = _projectService.GetProjectById(id);

                if (model == null || model.TwitterProfile != _workContext.CurrentUser.TwitterProfile || model.CreatedBy != null)
                {
                    ErrorNotification("Looks like you tried to click another Good Person's Tweet! Why not setup your own Good Thing below?");
                    return RedirectToRoute("AddProject");
                }

                projectModel = model.ToModel();
                projectModel.ReloadFromLocalStorage = false;
            }
            else
            {
                projectModel.ReloadFromLocalStorage = true;
            }

            AddHomeBreadcrumb();
            AddBreadcrumb("Add Project", Url.RouteUrl("AddProject", new { projectModel.Id }));

            projectModel.AvailableCategories = _categoryService.GetAllCategories().Select(x => x.ToModel()).ToList();
            projectModel.AvailableDisclosureLevels = _webHelper.GetAllEnumListItems<DisclosureLevel>();
            projectModel.AvailableHours = _webHelper.GetAvailableHours();
            projectModel.AvailableMinutes = _webHelper.GetAvailableMinutes();
            projectModel.AvailableRecurrenceIntervals = _webHelper.GetAllEnumListItems<RecurrenceInterval>();
            projectModel.ChildFriendly = false;
            projectModel.EmailAddress = _workContext.CurrentUser.Email;
            projectModel.EmailDisclosureId = _workContext.CurrentUser.EmailDisclosureId;
            projectModel.StartHour = 9;
            projectModel.Telephone = _workContext.CurrentUser.Telephone;
            projectModel.TelephoneDisclosureId = _workContext.CurrentUser.TelephoneDisclosureId;
            projectModel.Website = _workContext.CurrentUser.Website;
            projectModel.WebsiteDisclosureId = _workContext.CurrentUser.WebsiteDisclosureId;

            return View(projectModel);
        }

        [Authorize]
        [HttpPost, ValidateInput(false)]
        public ActionResult Add(ProjectModel model, FormCollection form, int id)
        {
            var selectedCategoryIds = form["SelectedCategories"] != null ? form["SelectedCategories"].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();

            // Make sure we have recurrence details if recurrence is enabled
            if (model.IsRecurring)
            {
                if (model.RecurrenceIntervalId == 0)
                    ModelState.AddModelError("RecurrenceIntervalId", "Please select the recurrence interval.");

                if (model.Recurrence == 0)
                    ModelState.AddModelError("Recurrence", "Please select the number of times the event will recur.");
            }

            // Make sure we have a location
            if (model.Latitude == 0 || model.Latitude == 999 || model.Longitude == 0 || model.Longitude == 999)
            {
                if (!string.IsNullOrEmpty(model.LocationInput))
                {
                    decimal latitude;
                    decimal longitude;

                    _geolocationService.GetLatLng(model.LocationInput, out latitude, out longitude);

                    if (latitude != 999 && longitude != 999)
                    {
                        model.Latitude = latitude;
                        model.Longitude = longitude;
                    }
                    else
                    {
                        // No location could be found
                        ModelState.AddModelError("LocationInput", "We couldn't find a location for '" + model.LocationInput + "'. Make sure the location is spelled correctly or try entering a postcode.");
                    }
                }
                else
                {
                    // No location has been provided
                    ModelState.AddModelError("LocationInput", "Please enter the location or postcode of where the good thing will take place.");
                }
            }

            if (ModelState.IsValid)
            {
                Project project;

                // Build the project entity
                if (id == 0)
                {
                    project = model.ToEntity();
                }
                else
                {
                    if (_workContext == null)
                        return RedirectToRoute("AddProject");

                    project = _projectService.GetProjectById(id);

                    if (project == null || project.TwitterProfile != _workContext.CurrentUser.TwitterProfile || project.CreatedBy != null)
                        return RedirectToRoute("AddProject");
                    
                    model.ToEntity(project);
                }

                // Make sure the project length doesn't exceed the max length
                // This is only to cover us incase someone tries to alter the front end validation
                if (project.Name.Length > PROJECT_MAX_LENGTH)
                    project.Name = project.Name.Substring(0, PROJECT_MAX_LENGTH);

                try
                {
                    project.StartDate = new DateTime(model.StartDate.Value.Year, model.StartDate.Value.Month, model.StartDate.Value.Day, model.StartHour, model.StartMinutes, 0);
                    project.EndDate = new DateTime(model.EndDate.Value.Year, model.EndDate.Value.Month, model.EndDate.Value.Day, model.EndHour, model.EndMinutes, 0);

                    var availableCategories = _categoryService.GetAllCategories();

                    foreach (var categoryId in selectedCategoryIds.Select(int.Parse))
                        project.Categories.Add(availableCategories.First(x => x.Id == categoryId));

                    project.CreatedBy = _workContext.CurrentUser.Id;
                    project.LastModifiedBy = _workContext.CurrentUser.Id;
                    project.Owners.Add(_workContext.CurrentUser);
                    project.Status = ProjectStatus.PendingApproval;

                    // Get the location entity based on the final resting place of the map marker
                    var location = _geolocationService.GetLocationFromLatLng(project.Latitude, project.Longitude);

                    // If we have a location, store it against the project record
                    var projectLocations = new List<ProjectLocation>();
                    if (location != null)
                    {
                        projectLocations.Add(new ProjectLocation
                        {
                            LocationId = location.Id,
                            Primary = true
                        });

                        var parent = location.ParentLocation;
                        while (parent != null)
                        {
                            projectLocations.Add(new ProjectLocation
                            {
                                LocationId = parent.Id,
                                Primary = false
                            });

                            parent = parent.ParentLocation;
                        }
                    }

                    if (id > 0)
                    {
                        // Loop through each existing project location, 
                        // deleting them from the database.
                        int locationCount = project.Locations.Count;
                        for (var i = 0; i < locationCount; i++)
                        {
                            var item = project.Locations.ElementAt(0);
                            _projectService.DeleteProjectLocation(item);
                            project.Locations.Remove(item);
                        }

                        // Loop each new project location, setting the project Id 
                        // and adding to the project location list
                        foreach (var item in projectLocations)
                        {
                            item.ProjectId = id;
                            project.Locations.Add(item);
                        }

                        // Update the existing entity
                        _projectService.UpdateProject(project);
                    }
                    else
                    {
                        // Insert the new entity
                        _projectService.InsertProject(project);

                        // Set the project location project id
                        projectLocations.ForEach(x => x.ProjectId = project.Id);
                        project.Locations = projectLocations;

                        // Update the project locations
                        _projectService.UpdateProject(project);
                    }

                    _moderationQueueService.InsertModerationQueueProjectApproval(project);

                    if (project.Id > 0)
                    {
                        TempData.Add("newProjectId", project.Id);
                        return RedirectToRoute("ConfirmProject");
                    }

                    _logService.Error("An unexpected error occurred while saving the project.");
                    ErrorNotification("An error occurred creating your project, please try again.");
                }
                catch (Exception ex)
                {
                    _logService.Error(ex.Message, ex, _workContext.CurrentUser);
                    ErrorNotification("An error occurred creating your project, please try again.");
                }
            }

            model.AvailableDisclosureLevels = _webHelper.GetAllEnumListItems<DisclosureLevel>();
            model.AvailableHours = _webHelper.GetAvailableHours();
            model.AvailableMinutes = _webHelper.GetAvailableMinutes();
            model.AvailableRecurrenceIntervals = _webHelper.GetAllEnumListItems<RecurrenceInterval>();

            model.AvailableCategories = _categoryService.GetAllCategories().Select(x => x.ToModel()).ToList();
            foreach (var categoryId in selectedCategoryIds.Select(int.Parse))
                model.AvailableCategories.First(x => x.Id == categoryId).IsChecked = true;

            model.Locations = new List<ProjectLocationModel>();

            return View(model);
        }

        public ActionResult Confirmation()
        {
            if (TempData["newProjectId"] == null)
                return RedirectToAction("index", "home");

            int id = int.Parse(TempData["newProjectId"].ToString());

            var project = _projectService.GetProjectById(id);

            if (project == null)
                return RedirectToAction("index", "home");

            AddHomeBreadcrumb();
            AddBreadcrumb("Add Project", Url.RouteUrl("AddProject"));
            AddBreadcrumb("Confirmation", Url.RouteUrl("ConfirmProject"));

            var model = project.ToModel();
            model.AvailableCategories = _categoryService.GetAllCategories().Select(x => x.ToModel()).ToList();
            return View(model);
        }

        #endregion

        #region Detail Page

        public ActionResult Detail(int id, string seoName, string locationSeoName)
        {
            var project = _projectService.GetProjectById(id);

            if (project == null || (project.Status != ProjectStatus.Open && project.Status != ProjectStatus.InProgress && project.Status != ProjectStatus.Closed))
                return RedirectToAction("index", "home");

            var primaryLocation = project.Locations.First(l => l.Primary).Location;

            AddHomeBreadcrumb();
            AddBreadcrumb("Action listing", Url.RouteUrl("ProjectListing"));
            AddBreadcrumb(project.Name, Url.RouteUrl("ProjectDetail", new { id = project.Id, seoName = project.GetSeoName(), locationSeoName = primaryLocation.SeoName }));

            if (primaryLocation.GetSeoName() != locationSeoName)
                return RedirectToRoutePermanent("ProjectDetail", new { locationSeoName = primaryLocation.GetSeoName(), seoName, id });

            if(project.GetSeoName() != seoName)
                return RedirectToRoutePermanent("ProjectDetail", new { locationSeoName = primaryLocation.GetSeoName(), seoName = project.GetSeoName(), id });

            var model = PrepareProjectDetailModel(project);

            if (TempData[PROJECT_COMMENT_ADDED] != null)
                model.ProjectCommentTracking = true;

            int commentModerationRequestLimit;
            int.TryParse(_siteSettings.CommentModerationRequestLimit, out commentModerationRequestLimit);

            // Filter out any deleted comments
            model.Project.Comments = (from c in model.Project.Comments
                                      where (c.Active && !c.Deleted && (commentModerationRequestLimit == 0 || c.ModerationRequestCount < commentModerationRequestLimit)) ||
                                      c.Responses.Any(x => x.Active && !x.Deleted && (commentModerationRequestLimit == 0 || x.ModerationRequestCount < commentModerationRequestLimit))
                                      select c).ToList();

            // From the comments we've found, filter out any deleted responses
            foreach (var comment in model.Project.Comments.Where(x => x.Responses.Count > 0))
                comment.Responses = comment.Responses.Where(x => x.Active && !x.Deleted && (commentModerationRequestLimit == 0 || x.ModerationRequestCount < commentModerationRequestLimit)).ToList();

            // Add the project owner to the list of volunteers
            if(model.Project.Volunteers.All(x => x.Id != model.Project.CreatedById) && model.Project.Owners.Any(x => x.Id == model.Project.CreatedById))
                model.Project.Volunteers.Add(model.Project.Owners.First(x => x.Id == model.Project.CreatedById));

            // Get the created by user model
            if (model.Project.CreatedById != null)
                model.Project.CreatedBy = _userService.GetUserById((int) model.Project.CreatedById).ToModel();

            return View(model);
        }

        [HttpPost, ActionName("Detail")]
        [FormValueRequired("projectSignUp")]
        public ActionResult VolunteerButton(int id, string seoName, string locationSeoName)
        {
            return RedirectToAction("Volunteer", new { id, seoName, locationSeoName });
        }

        [HttpPost, ActionName("Detail")]
        [FormValueRequired("cancelAttendance")]
        public ActionResult CancelAttendance(int id)
        {
            var project = _projectService.GetProjectById(id);

            if (project == null)
                return RedirectToAction("index", "home");

            if(project.Volunteers.Any(x => x.Id == _workContext.CurrentUser.Id))
            {
                project.Volunteers.Remove(project.Volunteers.First(x => x.Id == _workContext.CurrentUser.Id));
                _projectService.UpdateProject(project);
                AddNotification(NotifyType.Success, "Thanks, you are released from this good thing. Other good things will happen in its place.", true);
            }

            var primaryLocation = project.Locations.First(l => l.Primary).Location;
            return RedirectToRoute("ProjectDetail", new { locationSeoName = primaryLocation.GetSeoName(), seoName = project.GetSeoName(), id });
        }

        [HttpPost, ActionName("Detail")]
        [FormValueRequired("cancelProject")]
        public ActionResult CancelProject(int id)
        {
            var project = _projectService.GetProjectById(id);

            if (project == null)
                return RedirectToAction("index", "home");

            _moderationQueueService.InsertModerationQueueProjectWithdrawal(project);
            AddNotification(NotifyType.Success, "Thanks, we see you wish to cancel. A #wewillgather moderator will mull over your request. You'll get an email or direct message on the Twitter with a response.", true);

            var primaryLocation = project.Locations.First(l => l.Primary).Location;
            return RedirectToRoute("ProjectDetail", new { locationSeoName = primaryLocation.GetSeoName(), seoName = project.GetSeoName(), id });
        }

        [HttpPost, ActionName("Detail")]
        [FormValueRequired("deleteComment")]
        public ActionResult DeleteComment(int id, string deleteCommentId)
        {
            var project = _projectService.GetProjectById(id);

            if (project == null)
                return RedirectToAction("index", "home");

            var primaryLocation = project.Locations.First(l => l.Primary).Location;

            int commentId;
            if (int.TryParse(deleteCommentId, out commentId))
            {
                var comment = _commentService.GetCommentById(commentId);
                if (comment != null && comment.Author.Id == _workContext.CurrentUser.Id)
                {
                    _commentService.DeleteComment(comment);
                    AddNotification(NotifyType.Success, "Thank you, your comment has been deleted.", true);
                    return RedirectToRoute("ProjectDetail", new { locationSeoName = primaryLocation.GetSeoName(), seoName = project.GetSeoName(), id });
                }
            }

            AddNotification(NotifyType.Error, "An error occurred deleting your comment. Please try again.", true);
            return RedirectToRoute("ProjectDetail", new { locationSeoName = primaryLocation.GetSeoName(), seoName = project.GetSeoName(), id });
        }

        [HttpPost, ActionName("Detail")]
        [FormValueRequired("postComment")]
        [ValidateInput(false)]
        public ActionResult PostComment(int id, string commentBody, int inResponseTo)
        {
            var project = _projectService.GetProjectById(id);
            if (project == null)
                return RedirectToAction("index", "home");

            var primaryLocation = project.Locations.First(l => l.Primary).Location;

            if (!string.IsNullOrEmpty(commentBody))
            {
                if (commentBody.Length > 2000)
                    commentBody = commentBody.Substring(0, 2000);
                // Build the comment object
                var comment = new Comment { UserComment = StripProfanity(commentBody.StripHtml()) };

                // Set whether the comment is a response
                if (inResponseTo > 0)
                    comment.InResponseTo = _commentService.GetCommentById(inResponseTo);
                else
                    comment.Project = project;

                // Insert the comment record
                _commentService.InsertComment(comment);

                TempData.Add(PROJECT_COMMENT_ADDED, true);
                return new RedirectResult(Url.RouteUrl("ProjectDetail", new { locationSeoName = primaryLocation.GetSeoName(), seoName = project.GetSeoName(), id }) + "#comment-" + comment.Id);
            }

            AddNotification(NotifyType.Error, "You didn't enter a comment", true);
            return new RedirectResult(Url.RouteUrl("ProjectDetail", new { locationSeoName = primaryLocation.GetSeoName(), seoName = project.GetSeoName(), id }));
        }

        [HttpPost, ActionName("Detail")]
        [FormValueRequired("releaseOwnership")]
        public ActionResult ReleaseOwnership(int id)
        {
            var project = _projectService.GetProjectById(id);

            if (project == null)
                return RedirectToAction("index", "home");

            if(project.Owners.Any(x => x.Id == _workContext.CurrentUser.Id))
            {
                project.Owners.Remove(project.Owners.First(x => x.Id == _workContext.CurrentUser.Id));
                _projectService.UpdateProject(project);
                AddNotification(NotifyType.Success, "Thanks, you've pulled out of the action successfully.", true);
            }

            var primaryLocation = project.Locations.First(l => l.Primary).Location;
            return RedirectToRoute("ProjectDetail", new { locationSeoName = primaryLocation.GetSeoName(), seoName = project.GetSeoName(), id });
        }

        [HttpPost, ActionName("Detail")]
        [FormValueRequired("reportComment")]
        [ValidateInput(false)]
        public ActionResult ReportComment(int id, string flagCommentId, string flagReason, string flagMessage)
        {
            var project = _projectService.GetProjectById(id);
            if (project == null)
                return RedirectToAction("index", "home");

            var primaryLocation = project.Locations.First(l => l.Primary).Location;

            int commentId;
            if (int.TryParse(flagCommentId, out commentId))
            {
                var comment = _commentService.GetCommentById(commentId);
                if (comment != null)
                {
                    int reason;
                    if (int.TryParse(flagReason, out reason))
                    {
                        // Insert the moderation request
                        _moderationQueueService.InsertModerationQueueCommentModeration(comment, (ProjectCommentComplaintType)reason, flagMessage.StripHtml());

                        // Add notification
                        AddNotification(NotifyType.Success, "Thanks, you've told #wewillgather what upset you about this comment. We'll get to the bottom of its issues promptly.", true);

                        // Redirect out
                        return RedirectToRoute("ProjectDetail", new { locationSeoName = primaryLocation.GetSeoName(), seoName = project.GetSeoName(), id });
                    }
                }
            }

            AddNotification(NotifyType.Error, "An error occurred submitting your comment complaint. Please try again.", true);
            return RedirectToRoute("ProjectDetail", new { locationSeoName = primaryLocation.GetSeoName(), seoName = project.GetSeoName(), id });
        }

        [HttpPost, ActionName("Detail")]
        [FormValueRequired("reportProject")]
        public ActionResult ReportProject(int id, string reportReason, string reportMessage)
        {
            var project = _projectService.GetProjectById(id);
            if (project == null)
                return RedirectToAction("index", "home");

            var primaryLocation = project.Locations.First(l => l.Primary).Location;

            int reason;
            if (int.TryParse(reportReason, out reason))
            {
                _moderationQueueService.InsertModerationQueueProjectModeration(project, reportMessage, (ProjectComplaintType) reason);
                AddNotification(NotifyType.Success, "Thank you, this action has been reported to a #wewillgather moderator.", true);
                return RedirectToRoute("ProjectDetail", new { locationSeoName = primaryLocation.GetSeoName(), seoName = project.GetSeoName(), id });
            }

            AddNotification(NotifyType.Error, "An error occurred submitting your report. Please try again.", true);
            return RedirectToRoute("ProjectDetail", new { locationSeoName = primaryLocation.GetSeoName(), seoName = project.GetSeoName(), id });
        }

        public ActionResult OrganiserContact(ProjectModel project, UserModel organiser, bool isOwner = false)
        {
            bool isVolunteer = _workContext.CurrentUser != null && project.Volunteers.Any(x => x.Id == _workContext.CurrentUser.Id);

            var model = new Dictionary<string, string>();

            if (organiser.FacebookProfile != null)
                model.Add("Facebook", string.Format("http://www.facebook.com/profile.php?id={0}", organiser.FacebookProfile));

            if (organiser.TwitterProfile != null)
                model.Add("Twitter", string.Format("http://twitter.com/account/redirect_by_id?id={0}", organiser.TwitterProfile));

            if (isOwner)
            {
                if (!string.IsNullOrEmpty(project.EmailAddress) && project.EmailDisclosureLevel == DisclosureLevel.Public || (project.EmailDisclosureLevel == DisclosureLevel.VolunteersOnly && isVolunteer))
                    model.Add("Email", string.Format("mailto:{0}", project.EmailAddress.EncodeEmail()));

                if (!string.IsNullOrEmpty(project.Telephone) && project.TelephoneDisclosureLevel == DisclosureLevel.Public || (project.TelephoneDisclosureLevel == DisclosureLevel.VolunteersOnly && isVolunteer))
                    model.Add("Call", string.Format("tel:{0}", project.Telephone));

                if (!string.IsNullOrEmpty(project.Website) && project.WebsiteDisclosureLevel == DisclosureLevel.Public || (project.WebsiteDisclosureLevel == DisclosureLevel.VolunteersOnly && isVolunteer))
                    model.Add("Website", project.Website);
            }
            else
            {
                if (!string.IsNullOrEmpty(project.EmailAddress) && organiser.EmailDisclosureLevel == DisclosureLevel.Public || (organiser.EmailDisclosureLevel == DisclosureLevel.VolunteersOnly && isVolunteer))
                    model.Add("Email", string.Format("mailto:{0}", organiser.Email.EncodeEmail()));

                if (!string.IsNullOrEmpty(project.Telephone) && organiser.TelephoneDisclosureLevel == DisclosureLevel.Public || (organiser.TelephoneDisclosureLevel == DisclosureLevel.VolunteersOnly && isVolunteer))
                    model.Add("Call", string.Format("tel:{0}", organiser.Telephone));

                if (!string.IsNullOrEmpty(project.Website) && organiser.WebsiteDisclosureLevel == DisclosureLevel.Public || (organiser.WebsiteDisclosureLevel == DisclosureLevel.VolunteersOnly && isVolunteer))
                    model.Add("Website", organiser.Website);
            }

            return PartialView("_OrganiserContact", model);
        }

        [Authorize]
        public ActionResult TakeOwnership(int id)
        {
            var project = _projectService.GetProjectById(id);
            if (project == null)
                return RedirectToAction("index", "home");

            var primaryLocation = project.Locations.First(l => l.Primary).Location;

            if (project.Status != ProjectStatus.Open && project.Status != ProjectStatus.InProgress)
                return RedirectToRoute("ProjectDetail", new { id, seoName = project.GetSeoName(), locationSeoName = primaryLocation.GetSeoName() });

            if (project.Owners.All(x => x.Id != _workContext.CurrentUser.Id))
            {
                project.Owners.Add(_workContext.CurrentUser);

                if (project.Volunteers.All(x => x.Id != _workContext.CurrentUser.Id))
                    project.Volunteers.Add(_workContext.CurrentUser);

                _projectService.UpdateProject(project);

                AddNotification(NotifyType.Success, "Thank you, you have successfully volunteered to organise this action.", true);
            }

            return RedirectToRoute("ProjectDetail", new { locationSeoName = primaryLocation.GetSeoName(), seoName = project.GetSeoName(), id });
        }

        public ActionResult TinyUrl(int id)
        {
            var project = _projectService.GetProjectById(id);

            if (project == null || (project.Status != ProjectStatus.Open && project.Status != ProjectStatus.InProgress && project.Status != ProjectStatus.Closed))
                return RedirectToAction("index", "home");

            var primaryLocation = project.Locations.First(l => l.Primary).Location;
            return RedirectToRoutePermanent("ProjectDetail", new { locationSeoName = primaryLocation.GetSeoName(), seoName = project.GetSeoName(), id });
        }

        [Authorize]
        public ActionResult Volunteer(int id, string seoName, string locationSeoName)
        {
            var project = _projectService.GetProjectById(id);
            if (project == null)
                return RedirectToAction("index", "home");

            if (project.Status != ProjectStatus.Open && project.Status != ProjectStatus.InProgress)
                return RedirectToRoute("ProjectDetail", new { id, seoName, locationSeoName });

            // Make sure the user isn't already volunteered
            if (project.Volunteers.All(x => x.Id != _workContext.CurrentUser.Id))
            {
                // Add the user to the project
                project.Volunteers.Add(_workContext.CurrentUser);

                // Update the project entity
                _projectService.UpdateProject(project);

                // Post to the user's Twitter and Facebook profile to promote the action
                _userService.Post(_workContext.CurrentUser, project, ProjectAction.Join);
            }

            return new RedirectResult(Url.RouteUrl("ProjectDetail", new { id, seoName, locationSeoName }) + "#volunteerSuccess");
        }

        #endregion

        #region Edit Page

        public ActionResult Edit(int id, string seoName, string locationSeoName)
        {
            var project = _projectService.GetProjectById(id);
            if (project == null || (project.Status != ProjectStatus.Open && project.Status != ProjectStatus.InProgress))
                return RedirectToRoute("ProjectDetail", new { locationSeoName, seoName, id });

            if (_workContext.CurrentUser != null && project.Owners.Any(x => x.Id == _workContext.CurrentUser.Id))
            {
                var primaryLocation = project.Locations.First(l => l.Primary).Location;

                AddHomeBreadcrumb();
                AddBreadcrumb("Project Listing", Url.RouteUrl("ProjectListing"));
                AddBreadcrumb(project.Name, Url.RouteUrl("ProjectEdit", new { id = project.Id, seoName = project.GetSeoName(), locationSeoName = primaryLocation.SeoName }));
            }
            else
            {
                return RedirectToAction("index", "home");
            }

            var model = project.ToModel();
            model.AvailableCategories = _categoryService.GetAllCategories().Select(x => x.ToModel()).ToList();
            model.AvailableDisclosureLevels = _webHelper.GetAllEnumListItems<DisclosureLevel>();
            model.AvailableHours = _webHelper.GetAvailableHours();
            model.AvailableMinutes = _webHelper.GetAvailableMinutes();
            model.AvailableRecurrenceIntervals = _webHelper.GetAllEnumListItems<RecurrenceInterval>();
            model.CurrentUserId = _workContext.CurrentUser.Id;
            model.EndHour = model.EndDate == null ? 0 : model.EndDate.Value.Hour;
            model.EndMinutes = model.EndDate == null ? 0 : model.EndDate.Value.Minute;
            model.StartHour = model.StartDate == null ? 0 : model.StartDate.Value.Hour;
            model.StartMinutes = model.StartDate == null ? 0 : model.StartDate.Value.Minute;

            var selectCategories = model.AvailableCategories.Where(category => (model.Categories.Any(x => x.Id == category.Id)));
            selectCategories.ToList().ForEach(x => x.IsChecked = true);

            return View(model);
        }
  
        [HttpPost]
        [FormValueExists("update", "Update good thing", "projectUpdate")]
        public ActionResult Edit(ProjectModel model, FormCollection form, int id, string seoName, string locationSeoName, bool projectUpdate)
        {
            var project = _projectService.GetProjectById(id);
            if (project == null || (project.Status != ProjectStatus.Open && project.Status != ProjectStatus.InProgress))
                return RedirectToRoute("ProjectDetail", new { locationSeoName, seoName, id });

            var modelProject = project.ToModel();
            bool projectHasBeenChanged = false;

            // Make sure the user is logged in and is an organiser of the project
            if(_workContext == null || project.Owners.All(x => x.Id != _workContext.CurrentUser.Id))
                return RedirectToRoute("ProjectDetail", new { locationSeoName, seoName, id });

            // Get the list of selected categories
            var selectedCategoryIds = form["SelectedCategories"] != null ? form["SelectedCategories"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();

            // Check what type of button was clicked
            if (projectUpdate)
            {
                if (ModelState.IsValid)
                {
                    var newProject = model.ToEntity();

                    if (newProject.Name.Length > PROJECT_MAX_LENGTH)
                        newProject.Name = newProject.Name.Substring(0, PROJECT_MAX_LENGTH);

                    newProject.StartDate = new DateTime(model.StartDate.Value.Year, model.StartDate.Value.Month, model.StartDate.Value.Day, model.StartHour, model.StartMinutes, 0);
                    newProject.EndDate = new DateTime(model.EndDate.Value.Year, model.EndDate.Value.Month, model.EndDate.Value.Day, model.EndHour, model.EndMinutes, 0);

                    var availableCategories = _categoryService.GetAllCategories();
                    foreach (var categoryId in selectedCategoryIds.Select(int.Parse))
                        newProject.Categories.Add(availableCategories.First(x => x.Id == categoryId));

                    if (newProject.Name != project.Name) projectHasBeenChanged = true;
                    if (newProject.Objective != project.Objective) projectHasBeenChanged = true;
                    if (newProject.Latitude != project.Latitude) projectHasBeenChanged = true;
                    if (newProject.Longitude != project.Longitude) projectHasBeenChanged = true;
                    if (newProject.GettingThere != project.GettingThere) projectHasBeenChanged = true;
                    if (newProject.NumberOfVolunteers != project.NumberOfVolunteers) projectHasBeenChanged = true;
                    if (newProject.StartDate != project.StartDate) projectHasBeenChanged = true;
                    if (newProject.EndDate != project.EndDate) projectHasBeenChanged = true;
                    if (newProject.ChildFriendly != project.ChildFriendly) projectHasBeenChanged = true;
                    if (newProject.Skills != project.Skills) projectHasBeenChanged = true;
                    if (newProject.Equipment != project.Equipment) projectHasBeenChanged = true;
                    if (newProject.VolunteerBenefits != project.VolunteerBenefits) projectHasBeenChanged = true;
                    if (newProject.EmailAddress != project.EmailAddress) projectHasBeenChanged = true;
                    if (newProject.EmailDisclosureId != project.EmailDisclosureId) projectHasBeenChanged = true;
                    if (newProject.Telephone != project.Telephone) projectHasBeenChanged = true;
                    if (newProject.TelephoneDisclosureId != project.TelephoneDisclosureId) projectHasBeenChanged = true;
                    if (newProject.Website != project.Website) projectHasBeenChanged = true;
                    if (newProject.WebsiteDisclosureId != project.WebsiteDisclosureId) projectHasBeenChanged = true;
                        
                    if (newProject.Categories.Count(j => project.Categories.Any(c => c.Id == j.Id)) != newProject.Categories.Count)
                        projectHasBeenChanged = true; 

                    // Compare the project
                    if (!projectHasBeenChanged)
                    {
                        AddNotification(NotifyType.Error, "You didn't change the project.", true);
                    }
                    else
                    {
                        newProject.CreatedBy = _workContext.CurrentUser.Id;
                        newProject.LastModifiedBy = _workContext.CurrentUser.Id;
                        newProject.Owners.Add(_workContext.CurrentUser);
                        newProject.Status = ProjectStatus.PendingChangeApproval;

                        // Insert the temporary project
                        _projectService.InsertProject(newProject);

                        // Get the location entity based on the final resting place of the map marker
                        var location = _geolocationService.GetLocationFromLatLng(newProject.Latitude, newProject.Longitude);
                        if (location != null)
                        {
                            newProject.Locations.Add(new ProjectLocation
                            {
                                LocationId = location.Id,
                                Primary = true,
                                ProjectId = newProject.Id
                            });

                            var parent = location.ParentLocation;
                            while (parent != null)
                            {
                                newProject.Locations.Add(new ProjectLocation
                                {
                                    LocationId = parent.Id,
                                    Primary = false,
                                    ProjectId = newProject.Id
                                });

                                parent = parent.ParentLocation;
                            }
                        }

                        // Join the locations to the newly created project entity
                        _projectService.UpdateProject(newProject);

                        // Raise the moderation request
                        _moderationQueueService.InsertModerationQueueProjectChange(project, newProject);

                        // Do the update
                        AddNotification(NotifyType.Success, "Thanks, we see you wish to change the project. A #wewillgather moderator will mull over your request. You'll get an email or direct message on the Twitter with a response.", true);
                        var primaryLocation = project.Locations.First(l => l.Primary).Location;
                        return RedirectToRoute("ProjectDetail", new { id, seoName = project.GetSeoName(), locationSeoName = primaryLocation.SeoName });
                    }
                }
            }
            else
            {
                if (form["update"].ToLower() == "add as co-organisers")
                {
                    // Promote volunteers
                    var selectedVolunteerIds = form["SelectedVolunteers"] != null ? form["SelectedVolunteers"].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();
                    foreach (var selectedVolunteerId in selectedVolunteerIds)
                    {
                        _projectService.InsertProjectUserHistory(new ProjectUserHistory
                        {
                            AffectedUser = _userService.GetUserById(int.Parse(selectedVolunteerId)),
                            CommittingUser = _workContext.CurrentUser,
                            ProjectUserAction = ProjectUserAction.Added,
                            Project = project,
                            ProjectUserActionId = (int)ProjectUserAction.Added
                        });
                        project.Owners.Add(_userService.GetUserById(int.Parse(selectedVolunteerId)));
                    }
                }
                else
                {
                    // Remove organisers
                    var selectedOwnersIds = form["SelectedOwners"] != null ? form["SelectedOwners"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();
                    foreach (var selectedOwnerId in selectedOwnersIds)
                    {
                        if (int.Parse(selectedOwnerId) != modelProject.CreatedById)
                        {
                            _projectService.InsertProjectUserHistory(new ProjectUserHistory
                            {
                                AffectedUser = _userService.GetUserById(int.Parse(selectedOwnerId)),
                                CommittingUser = _workContext.CurrentUser,
                                ProjectUserAction = ProjectUserAction.Added,
                                Project = project,
                                ProjectUserActionId = (int)ProjectUserAction.Removed
                            });
                            project.Owners.Remove(_userService.GetUserById(int.Parse(selectedOwnerId)));
                        }
                    }
                }

                _projectService.UpdateProject(project);
                modelProject = project.ToModel();
            }

            model.AvailableCategories = _categoryService.GetAllCategories().Select(x => x.ToModel()).ToList();
            model.AvailableDisclosureLevels = _webHelper.GetAllEnumListItems<DisclosureLevel>();
            model.AvailableHours = _webHelper.GetAvailableHours();
            model.AvailableMinutes = _webHelper.GetAvailableMinutes();
            model.AvailableRecurrenceIntervals = _webHelper.GetAllEnumListItems<RecurrenceInterval>();
            model.CreatedById = modelProject.CreatedById;
            model.CurrentUserId = _workContext.CurrentUser.Id;
            model.Locations = modelProject.Locations;
            model.Owners = modelProject.Owners;
            model.SeoName = modelProject.SeoName;
            model.Volunteers = modelProject.Volunteers;

            foreach (var categoryId in selectedCategoryIds.Select(int.Parse))
                model.AvailableCategories.First(x => x.Id == categoryId).IsChecked = true;          

            return View(model);
        }

        #endregion

        #region Listing Page

        public ActionResult Listing(string locationSeoName = null, string parentSeoName = null, string query = null, int radius = (int)ProjectSearchRadius.FiveMiles, 
            int sortType = (int)ProjectSortType.StartDate, int sortDirection = (int)ProjectSortDirection.Ascending,
            string categories = null, int start = (int)ProjectSearchStart.Whenever, int childFriendly = (int)ProjectChildFriendly.NoPreference)
        {
            // Make sure we have valid sort values,
            // If they are invalid, redirect to the correct URL
            if (!Enum.IsDefined(typeof(ProjectSortType), sortType) || 
                !Enum.IsDefined(typeof(ProjectSortDirection), sortDirection) || 
                !Enum.IsDefined(typeof(ProjectSearchRadius), radius) || 
                !Enum.IsDefined(typeof(ProjectSearchStart), start) ||
                !Enum.IsDefined(typeof(ProjectChildFriendly), childFriendly))
            {
                var redirectValues = new RouteValueDictionary();

                if (!string.IsNullOrEmpty(locationSeoName))
                    redirectValues.Add("locationSeoName", locationSeoName);

                if (!string.IsNullOrEmpty(query))
                    redirectValues.Add("query", query);

                if (Enum.IsDefined(typeof(ProjectSearchRadius), radius) && radius != (int)ProjectSearchRadius.FiveMiles)
                    redirectValues.Add("radius", radius);

                if (Enum.IsDefined(typeof(ProjectSearchStart), start) && start != (int)ProjectSearchStart.Whenever)
                    redirectValues.Add("start", start);

                if (Enum.IsDefined(typeof(ProjectChildFriendly), childFriendly) && childFriendly != (int)ProjectChildFriendly.NoPreference)
                    redirectValues.Add("childFriendly", childFriendly);

                if (!string.IsNullOrEmpty(categories))
                    redirectValues.Add("categories", categories);

                if (Page > 1)
                    redirectValues.Add("page", Page);

                if (string.IsNullOrEmpty(locationSeoName))
                    return RedirectToRoute("ProjectListing", redirectValues);

                if (!string.IsNullOrEmpty(parentSeoName))
                {
                    redirectValues.Add("parentSeoName", parentSeoName);
                    return RedirectToRoute("ProjectListingLocationWithParent", redirectValues);
                }

                return RedirectToRoute("ProjectListingLocation", redirectValues);
            }

            // Set local values
            _locationSeoName = locationSeoName;
            _parentSeoName = parentSeoName;
            _query = query;
            _radius = radius;
            _sortType = sortType;
            _sortDirection = sortDirection;
            _start = start;
            _childFriendly = childFriendly;
            _categories = categories != null ? categories.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).Where(x => x != 0).ToList() : new List<int>();

            // Get all the projects and locations
            PrepareListingResults();

            // Make sure we are using the correct URL
            if (string.IsNullOrEmpty(_parentSeoName) && _location != null && _location.ParentLocation != null && !string.IsNullOrEmpty(_location.ParentLocation.SeoName))
            {
                return RedirectToRoute("ProjectListingLocationWithParent", new { locationSeoName, parentSeoName = _location.ParentLocation.SeoName });
            }

            // Get all the categories and convert to presentation model
            var categoryFilters = PrepareCategoryFilters(_projects);

            // Build the projects array for the front end map
            string markerArray = PrepareMapMarkerArray(_projects);

            // Build the canonical URL
            string canonicalUrl = null;
            if (HttpContext.Request.QueryString.Count > 0)
            {
                if (string.IsNullOrEmpty(locationSeoName))
                {
                    canonicalUrl = Url.RouteUrl("ProjectListing");
                }
                else if (!string.IsNullOrEmpty(_parentSeoName))
                {
                    canonicalUrl = Url.RouteUrl("ProjectListingLocationWithParent", new { locationSeoName, parentSeoName = _parentSeoName });
                }
                else
                {
                    canonicalUrl = Url.RouteUrl("ProjectListingLocation", new {locationSeoName});
                }
            }

            // Make sure we aren't trying to view an invalid page
            int totalPages = (int) Math.Ceiling(_projects.Count/(double) PerPage);
            if (totalPages > 0 && Page > totalPages)
                return RedirectToRoute("ProjectListing");

            // Build a route value dictionary to hold any additional parameters we need to send through to the pager.
            // We do this because the sort feature doesn't update the querystring, 
            // resulting in the sort parameters not being pulled through to the pager links.
            var additionalPagerValues = new RouteValueDictionary();

            if (sortType != (int)ProjectSortType.StartDate)
                additionalPagerValues.Add("sortType", sortType);

            if (sortDirection != (int)ProjectSortDirection.Ascending)
                additionalPagerValues.Add("sortDirection", sortDirection);

            // Get the result count for the mobile lazy loader
            int mobileNextResultCount = CalculateNextResultCount(Page);

            // Set the top level location filter to the main project listing URL
            _locationFilters.Link = GetPageLink("", "").ToString();

            var availableSortDirections = _webHelper.GetAllEnumListItems<ProjectSortDirection>();
            foreach (var direction in availableSortDirections)
            {
                switch ((ProjectSortDirection)int.Parse(direction.Value))
                {
                    case ProjectSortDirection.Ascending:
                        switch (sortType)
                        {
                            case (int)ProjectSortType.CreatedDate:
                                direction.Text = "Oldest";
                                break;
                            case (int)ProjectSortType.StartDate:
                                direction.Text = "Soonest";
                                break;
                            case (int)ProjectSortType.Volunteers:
                                direction.Text = "Least";
                                break;
                        }
                        break;
                    case ProjectSortDirection.Descending:
                        switch (sortType)
                        {
                            case (int)ProjectSortType.CreatedDate:
                                direction.Text = "Newest";
                                break;
                            case (int)ProjectSortType.StartDate:
                                direction.Text = "Latest";
                                break;
                            case (int)ProjectSortType.Volunteers:
                                direction.Text = "Most";
                                break;
                        }
                        break;
                }
            }

            // Bind all the data to the presentation model
            var model = new ProjectListingModel
            {
                AvailableChildFriendly = _webHelper.GetAllEnumListItems<ProjectChildFriendly>(),
                AvailableSearchRadius = _webHelper.GetAllEnumListItems<ProjectSearchRadius>(),
                AvailableSearchStart = _webHelper.GetAllEnumListItems<ProjectSearchStart>(),
                AvailableSortDirections = availableSortDirections,
                AvailableSortTypes = _webHelper.GetAllEnumListItems<ProjectSortType>(),
                CanonicalUrl = canonicalUrl,
                CategoryFilters = categoryFilters,
                IsSearch = !string.IsNullOrEmpty(query),
                Location = _location.ToModel(),
                LocationFilters = _locationFilters,
                MapLatitude = _mapLatitude,
                MapLongitude = _mapLongitude,
                MapProjects = markerArray,
                MapZoomLevel = _mapZoomLevel,
                MetaDescription = _metaDescription,
                MetaTitle = _metaTitle,
                PagedModel = new ProjectListPagedModel 
                {
                    AdditionalPagerValues = additionalPagerValues,
                    MobileNextResultCount = mobileNextResultCount,
                    PageIndex = Page,
                    PageSize = PerPage,
                    Projects = _projects.Skip((Page - 1) * PerPage).Take(PerPage).Select(x => x.ToModel()).ToList(),
                    TotalCount = _projects.Count
                },
                RssLink = _rssLink.ToString(),
                TotalCount = _projects.Count
            };

            AddHomeBreadcrumb();
            AddBreadcrumb("Good things", Url.RouteUrl("ProjectListing"), 1);

            return View(model);
        }

        public ActionResult LoadNextResults(int page = 1, int? locationId = null, string query = null, int radius = (int)ProjectSearchRadius.FiveMiles,
            int sortType = (int)ProjectSortType.StartDate, int sortDirection = (int)ProjectSortDirection.Ascending,
            string categories = null, int start = (int)ProjectSearchStart.Whenever, int childFriendly = (int)ProjectChildFriendly.NoPreference)
        {
            // Set local values
            _query = query;
            _radius = radius;
            _sortType = sortType;
            _sortDirection = sortDirection;
            _start = start;
            _childFriendly = childFriendly;
            _categories = categories != null ? categories.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).Where(x => x != 0).ToList() : new List<int>();

            // Get all the projects and locations
            PrepareListingResults(locationId, false);

            // Get the result count for the mobile lazy loader
            int mobileNextResultCount = CalculateNextResultCount(page);

            // Build the HTML output for the next listings
            var sb = new StringBuilder();
            foreach (var project in _projects.Skip((page - 1) * PerPage).Take(PerPage).Select(x => x.ToModel()))
                sb.Append(RenderRazorViewToString("_ProjectListItem", project));

            // Build the presentation model
            var model = new ProjectNextResultsModel
            {
                NextResultCount = mobileNextResultCount,
                Projects = sb.ToString()
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RssListing(string locationSeoName = null, string parentSeoName = null, string query = null, int radius = (int)ProjectSearchRadius.FiveMiles,
            int sortType = (int)ProjectSortType.StartDate, int sortDirection = (int)ProjectSortDirection.Ascending,
            string categories = null, int start = (int)ProjectSearchStart.Whenever, int childFriendly = (int)ProjectChildFriendly.NoPreference)
        {
            // Set local values
            _locationSeoName = locationSeoName;
            _parentSeoName = parentSeoName;
            _query = query;
            _radius = radius;
            _sortType = sortType;
            _sortDirection = sortDirection;
            _start = start;
            _childFriendly = childFriendly;
            _categories = categories != null ? categories.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).Where(x => x != 0).ToList() : new List<int>();

            // Get all the projects and locations
            PrepareListingResults();

            const string geoRssNameSpace = "http://www.georss.org/georss";
            var xmlQualifiedName = new XmlQualifiedName("georss", "http://www.w3.org/2000/xmlns/");
            var projectItems = new List<SyndicationItem>();

            foreach (var project in _projects)
            {
                var primaryLocation = project.Locations.First(l => l.Primary).Location;

                var description = new StringBuilder();
                description.Append("<div itemscope itemtype=\"http://data-vocabulary.org/Event\">");
                description.Append("<p itemprop=\"description\">" + _webHelper.BreakRuled(project.Objective) + "</p>");
                description.Append("<ul>");

                if (project.StartDate != null)
                {
                    description.Append("<li><strong>Start:</strong> <span itemprop=\"startDate\" datetime=\"" + string.Format("{0:yyyy-MM-ddTHH:mm}", project.StartDate.Value) + "\">" + _webHelper.DateTimeFormat("d~ MMMM yyyy", project.StartDate.Value) + "</span></li>");
                }

                description.Append("<li>");
                description.Append("<strong>Location:</strong> ");
                description.Append("<span itemprop=\"location\">" + (primaryLocation != null ? primaryLocation.Name : "Not supplied") + "</span>");
                description.Append("</li>");
                    
                if (project.StartDate != null)
                {
                    description.Append("<li>");
                    description.Append("<strong>Time:</strong> ");
                    description.Append(_webHelper.DateTimeFormat("h:mmTT", project.StartDate.Value));

                    if (project.EndDate != null && project.EndDate.Value.Subtract(project.StartDate.Value).Days < 1)
                    {
                        description.Append(" - " + _webHelper.DateTimeFormat("h:mmTT", project.EndDate.Value));
                    }

                    description.Append("</li>");
                }

                if (project.Categories.Count > 0)
                {
                    var httpContextWrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
                    var urlHelper = new UrlHelper(new RequestContext(httpContextWrapper, new RouteData()));

                    description.Append("<li>");
                    description.Append("<strong>" + (project.Categories.Count == 1 ? "Category" : "Categories") + ":</strong>");
                    description.Append(project.Categories.Aggregate("", (current, category) => current + " <a href=\"" + urlHelper.RouteUrl("ProjectListing", new { categories = category.Id }) + "\" itemprop=\"eventType\">" + category.Name + "</a>,").TrimEnd(','));
                    description.Append("</li>");
                }

                description.Append("<li><strong>Meeting at:</strong> " + (project.GettingThere ?? "Not supplied"));

                if (!string.IsNullOrEmpty(project.Skills))
                {
                    description.Append("<li><strong>Skills:</strong> " + project.Skills + "</li>");
                }

                if (!string.IsNullOrEmpty(project.Equipment))
                {
                    description.Append("<li><strong>Required tools:</strong> " + project.Equipment + "</li>");
                }

                description.Append("<li><strong>Child friendly:</strong> " + (project.ChildFriendly ? "Yes" : "No") + "</li>");

                description.Append("</ul>");
                description.Append("</div>");

                string link = Url.RouteUrl("ProjectDetail", new { locationSeoName = primaryLocation.SeoName, seoName = project.GetSeoName(), id = project.Id }, "http");
                var item = new SyndicationItem(project.Name, description.ToString(), new Uri(link))
                {                    
                    PublishDate = project.CreatedDate
                };

                if (project.CreatedBy != null && project.CreatedBy > 0)
                {
                    var owner = _userService.GetUserById(project.CreatedBy.Value);
                    if(owner != null)
                    {
                        item.Authors.Add(new SyndicationPerson("", owner.DisplayName, Url.RouteUrl("UserProfile", new {userName = owner.UserName}, "http")));
                    }
                }

                project.Owners.ToList().ForEach(organiser => item.Contributors.Add(new SyndicationPerson("", organiser.DisplayName, Url.RouteUrl("UserProfile", new { userName = organiser.UserName }, "http"))));
                project.Categories.ToList().ForEach(category => item.Categories.Add(new SyndicationCategory(category.Name)));

                item.ElementExtensions.Add("point", geoRssNameSpace, string.Format("{0} {1}", project.Latitude, project.Longitude));

                projectItems.Add(item);
            }

            var feed = new SyndicationFeed((_location != null ? _location.Name + " " : "") + "Volunteer Projects", "Volunteer projects RSS feed.", new Uri(Url.RouteUrl("ProjectListing", null, "http")), projectItems);

            feed.AttributeExtensions.Add(xmlQualifiedName, geoRssNameSpace);

            if (_mapLatitude != 0 && _mapLatitude != 999 && _mapLongitude != 0 && _mapLatitude != 999)
            {
                feed.ElementExtensions.Add("point", geoRssNameSpace, string.Format("{0} {1}", _mapLatitude, _mapLongitude));
                if (!string.IsNullOrEmpty(query))
                {
                    int radiusMiles = 0;
                    switch ((ProjectSearchRadius)radius)
                    {
                        case ProjectSearchRadius.FiveMiles:
                            radiusMiles = 5;
                            break;
                        case ProjectSearchRadius.TenMiles:
                            radiusMiles = 10;
                            break;
                        case ProjectSearchRadius.FifteenMiles:
                            radiusMiles = 15;
                            break;
                        case ProjectSearchRadius.FiftyMiles:
                            radiusMiles = 50;
                            break;
                    }

                    if (radiusMiles > 0)
                    {
                        int radiusMeters = (int)Math.Round(radiusMiles*1609.344);
                        feed.ElementExtensions.Add("radius", geoRssNameSpace, radiusMeters.ToString());
                    }
                }
            }

            return new FeedResult(new Rss20FeedFormatter(feed));
        }

        #endregion

        #endregion

    }
}