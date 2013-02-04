using System;
using System.Collections.Generic;
using System.Linq;
using Gather.Core;
using Gather.Core.Cache;
using Gather.Core.Data;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Locations;
using Gather.Core.Domain.Projects;
using Gather.Core.Domain.Users;
using Gather.Services.Geolocation;
using Gather.Services.MessageQueues;

namespace Gather.Services.Projects
{
    public class ProjectService : IProjectService
    {

        #region Constants

        private const string PROJECTS_ALL_KEY = "Gather.projects.all";

        #endregion

        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IGeolocationService _geolocationService;
        private readonly IMessageQueueService _messageQueueService;
        private readonly IRepository<ProjectUserHistory> _projectUserHistoryRepository;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<ProjectLocation> _projectLocationRepository; 
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors

        public ProjectService(ICacheManager cacheManager, IGeolocationService geolocationService, IMessageQueueService messageQueueService, IRepository<Project> projectRepository, IRepository<ProjectLocation> projectLocationRepository, IRepository<ProjectUserHistory> projectUserHistoryRepository, IWebHelper webHelper, IWorkContext workContext)
        {
            _cacheManager = cacheManager;
            _geolocationService = geolocationService;
            _messageQueueService = messageQueueService;
            _projectRepository = projectRepository;
            _projectLocationRepository = projectLocationRepository;
            _projectUserHistoryRepository = projectUserHistoryRepository;
            _webHelper = webHelper;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        #region Project

        /// <summary>
        /// Bulk updates all of the projects
        /// </summary>
        /// <param name="projects"></param>
        public void BulkUpdateProjects(IList<Project> projects)
        {
            if (projects == null)
                throw new ArgumentNullException("projects");

            foreach (var project in projects)
            {
                project.LastModifiedBy = _workContext.CurrentUser != null ? _workContext.CurrentUser.Id : 0;
                project.LastModifiedDate = DateTime.Now;
            }

            _projectRepository.BulkUpdate(projects);
        }

        /// <summary>
        /// Get all base projects for use on the front end
        /// </summary>
        /// <returns>Collection of base projects</returns>
        public IQueryable<BaseProject> GetAllCachedProjects()
        {
            return _cacheManager.Get(PROJECTS_ALL_KEY, () =>
            {
                var query = from p in _projectRepository.Table
                            where p.StatusId == (int) ProjectStatus.Open || p.StatusId == (int) ProjectStatus.InProgress
                            select new BaseProject
                            {
                                Categories = p.Categories,
                                ChildFriendly = p.ChildFriendly,
                                CreatedBy = p.CreatedBy,
                                CreatedDate = p.CreatedDate,
                                EndDate = p.EndDate,
                                Id = p.Id,
                                Latitude = p.Latitude,
                                Locations = p.Locations,
                                Longitude = p.Longitude,
                                Name = p.Name,
                                NumberOfVolunteers = p.NumberOfVolunteers,
                                Objective = p.Objective,
                                Owners = p.Owners,
                                RemainingNumberOfVolunteers = p.NumberOfVolunteers - p.Volunteers.Count,
                                StartDate = p.StartDate,
                                Volunteers = p.Volunteers
                            };
                return query;
            });
        }

        /// <summary>
        /// Get all projects
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="filter">Filter</param>        
        /// <param name="search">Search </param>
        /// <returns>Paginated project collection</returns>
        public IPaginatedList<Project> GetAllProjects(int pageIndex = 0, int pageSize = -1, int filter = 20, string search = null)
        {
            var query = _projectRepository.Table;

            if (filter == -1)
            {
                query = query.Where(u => u.Categories.Count == 0);
                query = query.Where(u => u.StatusId == (int) ProjectStatus.Open || u.StatusId == (int) ProjectStatus.InProgress);
            }
            else
            {
                query = query.Where(u => u.StatusId == filter);
            }

            if (!string.IsNullOrEmpty(search))
                query = query.Where(u => u.Name.Contains(search));

            query = query.OrderBy(u => u.Name);

            var projects = new PaginatedList<Project>(query, pageIndex, pageSize);
            return projects;
        }

        /// <summary>
        /// Get paged list of all projects within the radius of a given point
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="categories">Selected categories</param>
        /// <param name="radius">Radius</param>
        /// <param name="start">Start</param>
        /// <param name="childFriendly">Child friendly</param>
        /// <returns>paged project list</returns>
        public IPaginatedList<Project> GetAllProjectsByCoordsPaged(decimal latitude, decimal longitude, int pageIndex = 0, int pageSize = -1, IList<int> categories = null,
            ProjectSearchRadius radius = ProjectSearchRadius.FiveMiles, ProjectSearchStart start = ProjectSearchStart.Whenever,
            ProjectChildFriendly childFriendly = ProjectChildFriendly.NoPreference)
        {
            // Get the base projects
            // This returns IEnumerable so I guess searching by co-ords requires 
            var projectIds = GetAllProjectsByCoords(latitude, longitude, categories,
                radius, start, childFriendly).Select(b => b.Id);

            var query = from p in _projectRepository.Table
                        where projectIds.Contains(p.Id)
                        orderby 1 ascending // needs order to paginate
                        select p;

            var projects = new PaginatedList<Project>(query, pageIndex, pageSize);
            return projects;
        }

        /// <summary>
        /// Get all projects within the radius of a given point
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="categories">Selected categories</param>
        /// <param name="radius">Radius</param>
        /// <param name="start">Start</param>
        /// <param name="childFriendly">Child friendly</param>
        /// <returns>Base project collection</returns>
        public IList<BaseProject> GetAllProjectsByCoords(decimal latitude, decimal longitude, IList<int> categories = null, 
            ProjectSearchRadius radius = ProjectSearchRadius.FiveMiles, ProjectSearchStart start = ProjectSearchStart.Whenever, 
            ProjectChildFriendly childFriendly = ProjectChildFriendly.NoPreference)
        {
            int radiusValue;
            switch (radius)
            {
                case ProjectSearchRadius.TenMiles:
                    radiusValue = 10;
                    break;
                case ProjectSearchRadius.FifteenMiles:
                    radiusValue = 15;
                    break;
                case ProjectSearchRadius.FiftyMiles:
                    radiusValue = 50; 
                    break;
                case ProjectSearchRadius.AnyDistance:
                    radiusValue = 0;
                    break;
                default:
                    radiusValue = 5;
                    break;
            }

            var query = from p in GetAllCachedProjects().ToList()
                        where radiusValue == 0 || _webHelper.Haversine((double)latitude, (double)longitude, (double)p.Latitude, (double)p.Longitude, DistanceType.Miles) <= radiusValue
                        select p;

            if (categories != null && categories.Count > 0)
                query = query.Where(p => categories.All(x => p.Categories.Any(c => c.Id == x)));

            switch (start)
            {
                case ProjectSearchStart.TodayOrTomorrow:
                    var tomorrow = DateTime.Now.AddDays(1);
                    query = query.Where(p => p.StartDate <= new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 23, 59, 59)).ToList();
                    break;
                case ProjectSearchStart.ThisWeek:
                    var thisWeek = DateTime.Now.AddDays((int)DateTime.Now.DayOfWeek);
                    query = query.Where(p => p.StartDate <= new DateTime(thisWeek.Year, thisWeek.Month, thisWeek.Day, 23, 59, 59)).ToList();
                    break;
                case ProjectSearchStart.ThisMonth:
                    query = query.Where(p => p.StartDate <= new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, 1, 23, 59, 59).AddDays(-1)).ToList();
                    break;
            }

            switch (childFriendly)
            {
                case ProjectChildFriendly.Friendly:
                    query = query.Where(p => p.ChildFriendly).ToList();
                    break;
                case ProjectChildFriendly.UnFriendly:
                    query = query.Where(p => !p.ChildFriendly).ToList();
                    break;
            }

            return query.ToList();
        }

        /// <summary>
        /// Get all projects assigned to a location
        /// </summary>
        /// <param name="location">Location</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="categories">Selected categories</param>
        /// <param name="sortType">Sort by type</param>
        /// <param name="sortDirection">Sort by direction</param>
        /// <returns>Base project collection</returns>
        public IPaginatedList<Project> GetAllProjectsByLocationPaged(Location location = null,
            int pageIndex = 0, int pageSize = -1,
            IList<int> categories = null,
            ProjectSortType sortType = ProjectSortType.StartDate,
            ProjectSortDirection sortDirection = ProjectSortDirection.Ascending)
        {
            // Get the base projects
            var baseProjects = GetAllProjectsByLocationQuery(location, categories, sortType, sortDirection);

            var query = from p in _projectRepository.Table
                        join b in baseProjects on p.Id equals b.Id
                        orderby 1 ascending // needs order to paginate
                        select p;

            var projects = new PaginatedList<Project>(query, pageIndex, pageSize);
            return projects;
        }

        /// <summary>
        /// Get all projects assigned to a location
        /// </summary>
        /// <param name="location">Location</param>
        /// <param name="categories">Selected categories</param>
        /// <param name="sortType">Sort by type</param>
        /// <param name="sortDirection">Sort by direction</param>
        /// <returns>Base project collection</returns>
        public IList<BaseProject> GetAllProjectsByLocation(Location location = null,
            IList<int> categories = null,
            ProjectSortType sortType = ProjectSortType.StartDate, 
            ProjectSortDirection sortDirection = ProjectSortDirection.Ascending)
        {
            return GetAllProjectsByLocationQuery(location, categories, sortType, sortDirection).ToList();
        }

        /// <summary>
        /// Get all projects assigned to a location
        /// </summary>
        /// <param name="location">Location</param>
        /// <param name="categories">Selected categories</param>
        /// <param name="sortType">Sort by type</param>
        /// <param name="sortDirection">Sort by direction</param>
        /// <returns>Base project query</returns>
        private IEnumerable<BaseProject> GetAllProjectsByLocationQuery(Location location = null,
            IList<int> categories = null,
            ProjectSortType sortType = ProjectSortType.StartDate, 
            ProjectSortDirection sortDirection = ProjectSortDirection.Ascending)
        {
            var query = GetAllCachedProjects();

            if (location != null)
                query = query.Where(p => p.Locations.Any(l => l.LocationId == location.Id));

            if (categories != null && categories.Count > 0)
                query = query.Where(p => categories.All(x => p.Categories.Any(c => c.Id == x)));

            switch (sortDirection)
            {
                case ProjectSortDirection.Descending:

                    switch (sortType)
                    {
                        case ProjectSortType.CreatedDate:
                            query = query.OrderByDescending(x => x.CreatedDate);
                            break;
                        case ProjectSortType.Volunteers:
                            query = query.OrderByDescending(x => x.Volunteers.Count);
                            break;
                        default:
                            query = query.OrderByDescending(x => x.StartDate);
                            break;
                    }

                    break;
                default:

                    switch (sortType)
                    {
                        case ProjectSortType.CreatedDate:
                            query = query.OrderBy(x => x.CreatedDate);
                            break;
                        case ProjectSortType.Volunteers:
                            query = query.OrderBy(x => x.Volunteers.Count);
                            break;
                        default:
                            query = query.OrderBy(x => x.StartDate);
                            break;
                    }

                    break;
            }

            return query;
        }

        /// <summary>
        /// Gets all projects based on a category id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>A list of projects</returns>
        public IList<Project> GetAllProjectsByCategoryId(int categoryId)
        {
            var query = _projectRepository.Table;

            query = query.Where(u => u.Categories.Any(c => c.Id == categoryId));

            var projects = query.ToList();
            return projects;
        }

        /// <summary>
        /// Get a list of finished projects for a user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Collection of projects</returns>
        public IList<Project> GetFinishedProjectsByUserId(int userId)
        {
            if (userId == 0)
                return null;

            var query = _projectRepository.Table;

            query = query.Where(p => p.Owners.Any(o => o.Id == userId) || p.Volunteers.Any(v => v.Id == userId));
            query = query.Where(p => p.StatusId == (int)ProjectStatus.InProgress || p.StatusId == (int)ProjectStatus.Closed);

            query = query.OrderBy(p => p.StartDate);

            var projects = query.ToList();
            return projects;
        }

        /// <summary>
        /// Get all projects organised by a given user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Collection of projects</returns>
        public IList<Project> GetOrganisedProjectsByUserId(int userId)
        {
            if (userId == 0)
                return null;

            var query = _projectRepository.Table;
            query = query.Where(p => p.CreatedBy == userId || p.Owners.Any(o => o.Id == userId));
            query = query.Where(p => p.StatusId == (int)ProjectStatus.PendingApproval || p.StatusId == (int)ProjectStatus.Open || p.StatusId == (int)ProjectStatus.InProgress || p.StatusId == (int)ProjectStatus.Closed);

            var results = query.Select(p => new { Order = (p.StatusId == (int)ProjectStatus.PendingApproval ? 0 : 10), Data = p });

            query = results.OrderBy(p => p.Order).ThenBy(p => p.Data.StartDate).Select(p => p.Data);

            var projects = query.ToList();
            return projects;
        }

        /// <summary>
        /// Get a project by id
        /// </summary>
        /// <param name="projectId">Id of project to retrieve</param>
        /// <returns>Project</returns>
        public Project GetProjectById(int projectId)
        {
            if (projectId == 0)
                return null;

            var project = _projectRepository.GetById(projectId);
            return project;
        }

        /// <summary>
        /// Get a list of upcoming projects for a user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Collection of projects</returns>
        public IList<Project> GetUpcomingProjectsByUserId(int userId)
        {
            if (userId == 0)
                return null;

            var query = _projectRepository.Table;

            query = query.Where(p => p.Owners.Any(o => o.Id == userId) || p.Volunteers.Any(v => v.Id == userId));
            query = query.Where(p => p.StatusId == (int)ProjectStatus.PendingApproval || p.StatusId == (int)ProjectStatus.Open);

            query = query.OrderBy(p => p.StatusId).ThenBy(p => p.StartDate);

            var projects = query.ToList();
            return projects;
        }

        /// <summary>
        /// Insert a project
        /// </summary>
        /// <param name="project">Project</param>
        public void InsertProject(Project project)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            project.CreatedDate = DateTime.Now;
            project.LastModifiedDate = DateTime.Now;

            _projectRepository.Insert(project);
        }

        /// <summary>
        /// Insert a project for twitter, recreates the object due to context issues
        /// </summary>
        /// <param name="postcode">Postcode of the project</param>
        /// <param name="name">Name of the project</param>
        /// <param name="twitterProfile">Twitter profile name</param>
        /// <returns>Id of new project</returns>
        public int InsertTwitterProject(string postcode, string name, string twitterProfile)
        {
            decimal latitude = 999;
            decimal longitude = 999;
            Location location = null;

            if (!string.IsNullOrEmpty(postcode))
            {
                _geolocationService.GetLatLng(postcode, out latitude, out longitude);
                location = _geolocationService.GetLocationFromLatLng(latitude, longitude);
            }

            var twitterProject = new Project
            {
                CreatedDate = DateTime.Now,
                LastModifiedDate = DateTime.Now,
                Name = name.Replace("  ", " ").Trim(),
                Latitude = latitude,
                Longitude = longitude,
                StatusId = (int)ProjectStatus.Draft,
                TwitterProfile = twitterProfile
            };            

            // Insert the entity
            _projectRepository.Insert(twitterProject);

            if (location != null)
            {
                // Add all the locations to the project entity
                twitterProject.Locations.Add(new ProjectLocation
                {
                    LocationId = location.Id,
                    Primary = true,
                    ProjectId = twitterProject.Id
                });

                var parent = location.ParentLocation;
                while (parent != null)
                {
                    twitterProject.Locations.Add(new ProjectLocation
                    {
                        LocationId = parent.Id,
                        Primary = false,
                        ProjectId = twitterProject.Id
                    });

                    parent = parent.ParentLocation;
                }

                // Update the entity
                _projectRepository.Update(twitterProject);
            }

            return twitterProject.Id;
        }

        /// <summary>
        /// Change ownership of projects
        /// </summary>
        /// <param name="currentUserId">Current user id</param>
        /// <param name="newUser">New user</param>
        public void MigrateProjectOwnership(int currentUserId, User newUser)
        {
            if (currentUserId == 0 || newUser == null)
                return;

            var createdProjects = _projectRepository.Table.Where(x => x.CreatedBy == currentUserId).ToList();
            foreach (var project in createdProjects)
                project.CreatedBy = newUser.Id;
            _projectRepository.BulkUpdate(createdProjects);

            var volunteeredProjects = _projectRepository.Table.Where(x => x.Owners.Any(o => o.Id == currentUserId) || x.Volunteers.Any(v => v.Id == currentUserId)).ToList();
            foreach (var project in volunteeredProjects)
            {
                var owner = project.Owners.FirstOrDefault(x => x.Id == currentUserId);
                if (owner != null)
                {
                    project.Owners.Remove(owner);
                    if (!project.Owners.Any(x => x.Id == newUser.Id))
                        project.Owners.Add(newUser);
                }

                var volunteer = project.Volunteers.FirstOrDefault(x => x.Id == currentUserId);
                if (volunteer != null)
                {
                    project.Volunteers.Remove(volunteer);
                    if (!project.Volunteers.Any(x => x.Id == newUser.Id))
                        project.Volunteers.Add(newUser);
                }
            }

            _projectRepository.BulkUpdate(volunteeredProjects);
        }

        /// <summary>
        /// Remove a given user from all projects
        /// </summary>
        /// <param name="userId">User id</param>
        public void UnassociateUserFromProjects(int userId)
        {
            if (userId == 0)
                return;

            // Update all projects created by the user
            var createdProjects = _projectRepository.Table.Where(p => p.CreatedBy == userId).ToList();

            foreach (var project in createdProjects)
                project.CreatedBy = 0;

            _projectRepository.BulkDelete(createdProjects);

            // Update all the project the user is volunteered for
            var volunteeredProjects = _projectRepository.Table.Where(p => p.Volunteers.Any(x => x.Id == userId) || p.Owners.Any(x => x.Id == userId)).ToList();

            foreach (var project in volunteeredProjects)
            {
                var owner = project.Owners.FirstOrDefault(x => x.Id == userId);
                if (owner != null)
                    project.Owners.Remove(owner);

                // TODO: Send a message to organisers saying a co-organiser has dropped out
                // TODO: Send a message to volunteers if all organisers have dropped out

                var volunteer = project.Volunteers.FirstOrDefault(x => x.Id == userId);
                if (volunteer != null)
                    project.Volunteers.Remove(volunteer);

                // TODO: Send message to organisers saying a volunteer has dropped out
            }

            _projectRepository.BulkUpdate(volunteeredProjects);
        }

        /// <summary>
        /// Updates the project
        /// </summary>
        /// <param name="project">Project</param>
        /// <param name="setLastModifiedBy"> </param>
        public void UpdateProject(Project project, bool setLastModifiedBy = true)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            if (setLastModifiedBy)
            {
                project.LastModifiedBy = _workContext.CurrentUser != null ? _workContext.CurrentUser.Id : 0;
                project.LastModifiedDate = DateTime.Now;
            }

            _projectRepository.Update(project);
        }

        #endregion

        #region Project Locations

        /// <summary>
        /// Delete a project location mapping record
        /// </summary>
        /// <param name="projectLocation">Project location</param>
        public void DeleteProjectLocation(ProjectLocation projectLocation)
        {
            if (projectLocation == null)
                return;
            _projectLocationRepository.Delete(projectLocation, true);
        }

        #endregion

        #region Project User History

        /// <summary>
        /// Get all Projects user history
        /// </summary>
        /// <param name="projectId">Project Id</param>
        /// <param name="userId"> </param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated project user history collection</returns>
        public IPaginatedList<ProjectUserHistory> GetAllProjectUserHistory(int projectId, int userId, int pageIndex = 0, int pageSize = -1)
        {
            var query = _projectUserHistoryRepository.Table;

            query = query.Where(u => u.Project.Id == projectId);
            if(userId != -1)
                query = query.Where(u => u.AffectedUser.Id == userId);
            query = query.OrderBy(u => u.CreatedDate);

            var projects = new PaginatedList<ProjectUserHistory>(query, pageIndex, pageSize);
            return projects;
        }

        /// <summary>
        /// Inserts a Project User History
        /// </summary>
        /// <param name="projectUserHistory"></param>
        public void InsertProjectUserHistory(ProjectUserHistory projectUserHistory)
        {
            if (projectUserHistory == null)
                throw new ArgumentNullException("projectUserHistory");

            projectUserHistory.CreatedDate = DateTime.Now;

            _projectUserHistoryRepository.Insert(projectUserHistory);
        }

        #endregion

        #endregion

        #region Application Maintenance

        /// <summary>
        /// Get all projects due to start in set amount of time
        /// </summary>        
        /// <param name="minutes">Number of minutes until start time (default is 24 hours)</param>
        /// <returns>list of projects</returns>
        public void MessageProjectsDueToStart(int minutes = 1440)
        {
            var startDateFuture = DateTime.Now.AddMinutes(minutes);

            // Retrieve all projects due to start in the next 24 hours
            var query = _projectRepository.Table;
            query = query.Where(p => p.ReminderMessageSent == false);
            query = query.Where(p => p.StatusId == (int)ProjectStatus.Open);
            query = query.Where(p => p.StartDate < startDateFuture);
            query = query.Where(p => p.StartDate >= DateTime.Now);
            query = query.OrderBy(u => u.StartDate);

            // Iterate each project and despatch messages about it starting and mark the project as completed.
            foreach (var project in query.ToList())
            {
                // Queue the message
                _messageQueueService.ProjectStartingMessage(project);

                // Update each project as soon as the message has been sent to avoid sending out duplicates.
                project.ReminderMessageSent = true;
                UpdateProject(project, false);
            }
        }

        /// <summary>
        /// Under subscribed projects need a tweet 1 hour before project starts to try to obtain more volunteers
        /// </summary>
        public void MessageProjectsStartingAlertForMoreVolunteers()
        {
            var startDatePreAlert = DateTime.Now.AddMinutes(+60);

            // Retrieve all projects due to start in the next hour
            var query = _projectRepository.Table;            
            query = query.Where(p => p.StatusId == (int)ProjectStatus.Open || p.StatusId == (int)ProjectStatus.InProgress);
            query = query.Where(p => p.AlertMessageSent == false);
            query = query.Where(p => p.StartDate < startDatePreAlert);
            query = query.Where(p => p.StartDate >= DateTime.Now);
            query = query.Where(p => p.EndDate > DateTime.Now);
            query = query.Where(p => p.NumberOfVolunteers > p.Volunteers.Count());
            query = query.OrderBy(u => u.StartDate);

            var projects = query.ToList();

            // Iterate each project and despatch messages about it needing more volunteers
            foreach (var project in projects)
            {
                _messageQueueService.ProjectTweet(project, MessageType.TweetProjectMoreVolunteers);
                project.AlertMessageSent = true;
            }

            // Update all the projects
            if (projects.Count > 0)
                _projectRepository.BulkUpdate(projects);
        }

        /// <summary>
        /// Get all projects which are Open and now active and set them to In Progress
        /// </summary>                
        public void UpdateOpenProjectsToInProgress()
        {
            var query = _projectRepository.Table;            
            query = query.Where(p => p.StatusId == (int)ProjectStatus.Open);            
            query = query.Where(p => p.StartDate <= DateTime.Now);            

            var projects = query.ToList();

            foreach (var project in projects)        
                project.Status = ProjectStatus.InProgress;

            if (projects.Count > 0)
                _projectRepository.BulkUpdate(projects);
        }

        /// <summary>
        /// Get all projects which are in progress and have now finished and set them to closed
        /// </summary>  
        public void UpdateInProgressProjectsToClosed()
        {
            var query = _projectRepository.Table;
            query = query.Where(p => p.StatusId == (int)ProjectStatus.InProgress);
            query = query.Where(p => p.EndDate < DateTime.Now);

            var projects = query.ToList();

            foreach (var project in projects)
            {
                // If the event is meant to recur
                if (project.IsRecurring && project.Recurrence > 0 && project.StartDate != null && project.EndDate != null)
                {
                    switch (project.RecurrenceInterval)
                    {
                        case RecurrenceInterval.Daily:
                            project.EndDate = project.EndDate.Value.AddDays(1);
                            project.StartDate = project.StartDate.Value.AddDays(1);
                            break;
                        case RecurrenceInterval.Monthly:
                            project.EndDate = project.EndDate.Value.AddMonths(1);
                            project.StartDate = project.StartDate.Value.AddMonths(1);
                            break;
                        case RecurrenceInterval.Weekly:
                            project.EndDate = project.EndDate.Value.AddDays(7);
                            project.StartDate = project.StartDate.Value.AddDays(7);
                            break;
                    }

                    project.Recurrence -= 1;
                    project.Status = ProjectStatus.Open;

                    // Queue a message to alert the people already volunteered
                    _messageQueueService.ProjectMessage(project, MessageType.ProjectRecurrenceScheduled, null, null);
                }
                else
                {
                    // If it's not a recuring event, just close it
                    project.Status = ProjectStatus.Closed;
                }
            }

            // Update the entities
            if (projects.Count > 0)
                _projectRepository.BulkUpdate(projects);
        }

        /// <summary>
        /// Get all projects which were started via twitter but not progressed. We will set them to deleted
        /// </summary>                
        public void DeleteDraftProjectsNotProgressed()
        {
            var createdDate = DateTime.Now.AddDays(-14);

            var query = _projectRepository.Table;
            query = query.Where(p => p.StatusId == (int)ProjectStatus.Draft);
            query = query.Where(p => p.CreatedDate < createdDate);

            var projectsToDelete = query.ToList();

            foreach (var project in projectsToDelete)
                project.Status = ProjectStatus.Deleted;

            if (projectsToDelete.Count > 0)
                _projectRepository.BulkUpdate(projectsToDelete);
        }

        #endregion

    }
}