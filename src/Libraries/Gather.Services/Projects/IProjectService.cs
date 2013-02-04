using System.Collections.Generic;
using System.Linq;
using Gather.Core;
using Gather.Core.Domain.Locations;
using Gather.Core.Domain.Projects;
using Gather.Core.Domain.Users;

namespace Gather.Services.Projects
{
    public interface IProjectService
    {
       
        #region Project

        /// <summary>
        /// Bulk updates all of the projects
        /// </summary>
        /// <param name="projects"></param>
        void BulkUpdateProjects(IList<Project> projects);

        /// <summary>
        /// Get all base projects for use on the front end
        /// </summary>
        /// <returns>Collection of base projects</returns>
        IQueryable<BaseProject> GetAllCachedProjects();
            
        /// <summary>
        /// Get all Projects
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="filter">Filter</param>        
        /// <param name="search">Search term</param>
        /// <returns>Paginated project collection</returns>
        IPaginatedList<Project> GetAllProjects(int pageIndex = 0, int pageSize = -1, int filter = 20, string search = null);

        /// <summary>
        /// Get paged list of all projects within the radius of a given point
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="categories">Selected categories</param>
        /// <param name="radius">Radius</param>
        /// <param name="start">Start</param>
        /// <param name="childFriendly">Child friendly</param>
        /// <returns>Base project collection</returns>
        IPaginatedList<Project> GetAllProjectsByCoordsPaged(decimal latitude,
                                                  decimal longitude,
                                                  int pageIndex = 0,
                                                  int pageSize = -1,
                                                  IList<int> categories = null,
                                                  ProjectSearchRadius radius = ProjectSearchRadius.FiveMiles,
                                                  ProjectSearchStart start = ProjectSearchStart.Whenever,
                                                  ProjectChildFriendly childFriendly = ProjectChildFriendly.NoPreference);

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
        IList<BaseProject> GetAllProjectsByCoords(decimal latitude,
                                                  decimal longitude,
                                                  IList<int> categories = null,
                                                  ProjectSearchRadius radius = ProjectSearchRadius.FiveMiles,
                                                  ProjectSearchStart start = ProjectSearchStart.Whenever,
                                                  ProjectChildFriendly childFriendly = ProjectChildFriendly.NoPreference);

        /// <summary>
        /// Get all projects assigned to a location
        /// </summary>
        /// <param name="location">Location</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="categories">Selected categories</param>
        /// <param name="sortType">Sort by type</param>
        /// <param name="sortDirection">Sort by direction</param>
        /// <returns>Base project collection</returns>
        IPaginatedList<Project> GetAllProjectsByLocationPaged(Location location = null,
                                                              int pageIndex = 0,
                                                              int pageSize = -1,
                                                              IList<int> categories = null,
                                                              ProjectSortType sortType = ProjectSortType.StartDate,
                                                              ProjectSortDirection sortDirection = ProjectSortDirection.Ascending);

        /// <summary>
        /// Get all projects assigned to a location
        /// </summary>
        /// <param name="location">Location</param>
        /// <param name="categories">Selected categories</param>
        /// <param name="sortType">Sort by type</param>
        /// <param name="sortDirection">Sort by direction</param>
        /// <returns>Base project collection</returns>
        IList<BaseProject> GetAllProjectsByLocation(Location location = null,
                                                    IList<int> categories = null,
                                                    ProjectSortType sortType = ProjectSortType.StartDate,
                                                    ProjectSortDirection sortDirection = ProjectSortDirection.Ascending);
        
        /// <summary>
        /// This gets all of the projects by the category id
        /// </summary>
        /// <param name="categoryId">Category id</param>
        /// <returns></returns>
        IList<Project> GetAllProjectsByCategoryId(int categoryId);

        /// <summary>
        /// Get a list of finished projects for a user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Collection of projects</returns>
        IList<Project> GetFinishedProjectsByUserId(int userId);

        /// <summary>
        /// Get all projects organised by a given user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Collection of projects</returns>
        IList<Project> GetOrganisedProjectsByUserId(int userId);

        /// <summary>
        /// Get a project by id
        /// </summary>
        /// <param name="projectId">Id of project to retrieve</param>
        /// <returns>Project</returns>
        Project GetProjectById(int projectId);
            
        /// <summary>
        /// Get a list of upcoming projects for a user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Collection of projects</returns>
        IList<Project> GetUpcomingProjectsByUserId(int userId);

        /// <summary>
        /// Inserts a project
        /// </summary>
        /// <param name="project"></param>
        void InsertProject(Project project);

        /// <summary>
        /// Insert a project for twitter, recreates the object due to context issues
        /// </summary>
        /// <param name="postcode">Postcode of the project</param>
        /// <param name="name">Name of the project</param>
        /// <param name="twitterProfile">Twitter profile name</param>
        /// <returns>Id of new project</returns>
        int InsertTwitterProject(string postcode, string name, string twitterProfile);

        /// <summary>
        /// Change ownership of projects
        /// </summary>
        /// <param name="currentUserId">Current user id</param>
        /// <param name="newUser">New user</param>
        void MigrateProjectOwnership(int currentUserId, User newUser);

        /// <summary>
        /// Remove a given user from all projects
        /// </summary>
        /// <param name="userId">User id</param>
        void UnassociateUserFromProjects(int userId);

        /// <summary>
        /// Updates the project
        /// </summary>
        /// <param name="project"></param>
        /// <param name="setLastModifiedBy">Whether to user workcontext or not</param>
        void UpdateProject(Project project, bool setLastModifiedBy = true);

        #endregion

        #region Project Locations

        /// <summary>
        /// Delete a project location mapping record
        /// </summary>
        /// <param name="projectLocation">Project location</param>
        void DeleteProjectLocation(ProjectLocation projectLocation);

        #endregion

        #region Project User History

        /// <summary>
        /// Get all Projects user history
        /// </summary>
        /// <param name="projectId">Project Id</param>
        /// <param name="userId">User Id</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>      
        /// <returns>Paginated project user history collection</returns>
        IPaginatedList<ProjectUserHistory> GetAllProjectUserHistory(int projectId, int userId = -1, int pageIndex = 0, int pageSize = -1);

        /// <summary>
        /// Inserts a Project User History
        /// </summary>
        /// <param name="projectUserHistory"></param>
        void InsertProjectUserHistory(ProjectUserHistory projectUserHistory);
        #endregion
        
        #region Application Health

        /// <summary>
        /// Get all projects due to start in set amount of time
        /// </summary>        
        /// <param name="minutes">Number of minutes until start time (default is 24 hours)</param>
        void MessageProjectsDueToStart(int minutes = 1440);

        /// <summary>
        /// Under subscribed projects need a tweet 1 hour before project starts to try to obtain more volunteers
        /// </summary>
        void MessageProjectsStartingAlertForMoreVolunteers();

        /// <summary>
        /// Get all projects which are open and now active and set them to In Progress
        /// </summary>                
        void UpdateOpenProjectsToInProgress();

        /// <summary>
        /// Get all projects which are in progress which have now finished and set them to closed
        /// </summary>                
        void UpdateInProgressProjectsToClosed();

        /// <summary>
        /// Get all projects which were started via twitter but not progressed. We will set them to deleted
        /// </summary>                
        void DeleteDraftProjectsNotProgressed();

        #endregion

    }
}