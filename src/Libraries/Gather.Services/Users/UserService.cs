using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gather.Core;
using Gather.Core.Data;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Projects;
using Gather.Core.Domain.Users;
using Gather.Core.Infrastructure;
using Gather.Core.Seo;
using Gather.Services.Comments;
using Gather.Services.ModerationQueues;
using Gather.Services.Projects;
using Gather.Services.SuccessStories;
using Facebook;
using Twitterizer;

namespace Gather.Services.Users
{
    public class UserService : IUserService
    {

        #region Fields

        private readonly CoreSettings _coreSettings;
        private readonly HttpContextBase _httpContext;
        private readonly SiteSettings _siteSettings;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserRole> _userRoleRepository;

        #endregion

        #region Constructors

        public UserService(CoreSettings coreSettings, HttpContextBase httpContext, SiteSettings siteSettings, IRepository<User> userRepository, IRepository<UserRole> userRoleRepository)
        {
            _coreSettings = coreSettings;
            _httpContext = httpContext;
            _siteSettings = siteSettings;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
        }

        #endregion

        #region Methods

        #region Users

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="user">User</param>
        public void DeleteUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var moderationQueueService = EngineContext.Current.Resolve<IModerationQueueService>();
            var projectService = EngineContext.Current.Resolve<IProjectService>();
            var successStoryService = EngineContext.Current.Resolve<ISuccessStoryService>();

            // Delete all moderation requests
            moderationQueueService.DeleteModerationQueueByCreatedBy(user.Id);

            // Remove the user from all their associated projects
            projectService.UnassociateUserFromProjects(user.Id);

            // Remove association with success stories
            successStoryService.UnassociateSuccessStoryUser(user.Id);

            // Delete the user object
            _userRepository.Delete(user, true);
        }

        /// <summary>
        /// Check whether an email already exists
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="id">Exclude user id</param>
        /// <returns>True/False</returns>
        public bool EmailExists(string email, int id = 0)
        {
            if (string.IsNullOrEmpty(email))
                return false;
            return _userRepository.Table.Any(x => x.Email == email && x.Id != id);
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <param name="active">Active flag</param>
        /// <param name="search">Search term</param>
        /// <returns>Paginated user collection</returns>
        public IPaginatedList<User> GetAllUsers(bool? active = true, string search = "")
        {
            return GetAllUsers(0, -1, active, search);
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="active">Active flag</param>
        /// <param name="search">Search</param>
        /// <returns>Paginated user collection</returns>
        public IPaginatedList<User> GetAllUsers(int pageIndex, int pageSize, bool? active = true, string search = "")
        {
            if (!string.IsNullOrEmpty(search) && search.StartsWith("@"))
                search = search.Substring(1);

            var query = _userRepository.Table;

            if (active != null)
                query = query.Where(u => u.Active == active);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u =>
                                    u.DisplayName.Contains(search) ||
                                    u.Email.Contains(search) ||
                                    u.UserName.Contains(search) ||
                                    u.FacebookDisplayName.Contains(search) ||
                                    u.TwitterDisplayName.Contains(search));
            }

            query = query.OrderBy(u => u.UserName);

            var users = new PaginatedList<User>(query, pageIndex, pageSize);
            return users;
        }

        /// <summary>
        /// Get all users to display on the 'contact us' page
        /// </summary>
        /// <returns>Collection of users</returns>
        public IList<User> GetAllUsersForContactUs()
        {
            var query = from u in _userRepository.Table
                        where u.ShowOnContactUs
                        orderby u.DisplayName
                        select u;
            return query.ToList();
        }

        /// <summary>
        /// Get all users in a user role
        /// </summary>
        /// <param name="userRoleSystemName">UserRole system name</param>
        /// <returns>Collection of users</returns>
        public IList<User> GetAllUsersInRole(string userRoleSystemName)
        {
            var query = from u in _userRepository.Table
                        where u.UserRoles.Any(r => r.SystemName == userRoleSystemName)
                        select u;
            var users = query.ToList();
            return users;
        }

        /// <summary>
        /// Get the site owner
        /// </summary>
        /// <returns>User</returns>
        public User GetSiteOwner()
        {
            return GetAllUsersInRole(SystemUserRoleNames.SiteOwner).FirstOrDefault();
        } 

        /// <summary>
        /// Get a user by id
        /// </summary>
        /// <param name="userId">Id of user to retrieve</param>
        /// <returns>User</returns>
        public User GetUserById(int userId)
        {
            if (userId == 0)
                return null;

            var user = _userRepository.GetById(userId);
            return user;
        }

        /// <summary>
        /// Retrieve a user by their oAuth profile Id
        /// </summary>
        /// <param name="profile">Profile Id</param>
        /// <param name="method">Authentication method</param>
        /// <returns>User</returns>
        public User GetUserByOAuthProfile(string profile, AuthenticationMethod method)
        {
            if (string.IsNullOrWhiteSpace(profile))
                return null;

            var query = _userRepository.Table;

            switch (method)
            {
                case AuthenticationMethod.Facebook:

                    query = query.Where(u => u.FacebookProfile == profile);

                    break;
                case AuthenticationMethod.Twitter:

                    query = query.Where(u => u.TwitterProfile == profile);

                    break;
            }

            var user = query.FirstOrDefault();
            return user;
        }

        /// <summary>
        /// Retrieve a user by their username
        /// </summary>
        /// <param name="userName">Username</param>
        /// <returns>User</returns>
        public User GetUserByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return null;

            var user = _userRepository.Table.FirstOrDefault(x => x.UserName == userName);
            return user;
        }

        /// <summary>
        /// Insert a user
        /// </summary>
        /// <param name="user">User</param>
        public void InsertUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.Active = true;
            user.CreatedDate = DateTime.Now;
            user.LastLoginDate = DateTime.Now;

            _userRepository.Insert(user);
        }

        /// <summary>
        /// Merge two users together
        /// </summary>
        /// <param name="primaryUser">Primary user entity</param>
        /// <param name="addUser">User entity to add</param>
        /// /// <param name="method">Authentication method</param>
        public void MergeUsers(User primaryUser, User addUser, AuthenticationMethod method)
        {
            if (primaryUser == null || addUser == null)
                return;

            var commentService = EngineContext.Current.Resolve<ICommentService>();
            var moderationQueueService = EngineContext.Current.Resolve<IModerationQueueService>();
            var projectService = EngineContext.Current.Resolve<IProjectService>();
            var successStoryService = EngineContext.Current.Resolve<ISuccessStoryService>();

            // Change ownership of any comments
            commentService.MigrateCommentOwnership(addUser.Id, primaryUser);

            // Change ownership of any moderation requests
            moderationQueueService.MigrateModerationQueueOwnership(addUser.Id, primaryUser.Id);

            // Change ownership of any projects
            projectService.MigrateProjectOwnership(addUser.Id, primaryUser);

            // Change ownership and author of any success stories
            successStoryService.MigrateSuccessStoryAuthor(addUser.Id, primaryUser);

            switch (method)
            {
                case AuthenticationMethod.Facebook:

                    primaryUser.FacebookDisplayName = addUser.FacebookDisplayName;
                    primaryUser.FacebookProfile = addUser.FacebookProfile;

                    break;
                case AuthenticationMethod.Twitter:

                    primaryUser.TwitterDisplayName = addUser.TwitterDisplayName;
                    primaryUser.TwitterProfile = addUser.TwitterProfile;

                    break;
            }

            // Update primary user entity
            _userRepository.Update(primaryUser);

            // Delete the old user
            _userRepository.Delete(addUser, true);
        }

        /// <summary>
        /// Posts the information on facebook and twitter
        /// </summary>
        /// <param name="user">User to send from</param>
        /// <param name="project">Related project</param>
        /// <param name="action">Action that occurred</param>
        public void Post(User user, Project project, ProjectAction action)
        {
            // Make sure we have a user and a project
            if (user == null || project == null)
                return;

            // Build the URL of the project
            var urlHelper = new UrlHelper(_httpContext.Request.RequestContext);
            string projectUrl = _coreSettings.Domain.TrimEnd('/') + urlHelper.RouteUrl("ProjectDetailTiny", new { project.Id });

            // Try and send a Facebook status update
            if (!string.IsNullOrEmpty(user.FacebookAccessToken))
            {
                try
                {
                    string description, message;

                    // Build the message depending on the action that occurred
                    switch (action)
                    {
                        case ProjectAction.Approved:
                            description = "I've just setup a good thing on #WeWillGather, can anyone help?";
                            message = "I've just setup a good thing on #WeWillGather, can anyone help?";
                            break;
                        default:
                            description = "I've just signed up to a good thing on #WeWillGather!";
                            message = "I've just signed up to a good thing on #WeWillGather!";             
                            break;
                    }

                    // Build the parameters
                    var parameters = new Dictionary<string, object>();
                    parameters["description"] = description;
                    parameters["message"] = message;
                    parameters["name"] = project.Name;
                    parameters["link"] = projectUrl;

                    // Post to the user's wall
                    var fb = new FacebookClient(user.FacebookAccessToken);
                    fb.Post(user.FacebookProfile + "/feed", parameters);
                }
                catch { }
            }

            // Try and send a Twitter status update
            if (!string.IsNullOrEmpty(user.TwitterAccessToken) && !string.IsNullOrEmpty(user.TwitterAccessSecret))
            {
                try
                {
                    // Build the oAuth tokens object
                    var tokens = new OAuthTokens
                    {
                        AccessToken = user.TwitterAccessToken,
                        AccessTokenSecret = user.TwitterAccessSecret,
                        ConsumerKey = _siteSettings.TwitterConsumerKey,
                        ConsumerSecret = _siteSettings.TwitterConsumerSecret
                    };

                    var siteSettings = EngineContext.Current.Resolve<SiteSettings>();
                    string message;

                    // Build the message depending on the action that occurred
                    switch (action)
                    {
                        case ProjectAction.Approved:
                            message = string.Format("I've just setup a good thing {0} {1}", projectUrl, siteSettings.TwitterHashTag);
                            break;
                        default:
                            message = string.Format("I've just signed up to a good thing {0} {1}", projectUrl, siteSettings.TwitterHashTag);
                            break;
                    }

                    // Update the Twitter status
                    TwitterStatus.Update(tokens, message);
                }
                catch { }
            }
        }

        /// <summary>
        /// Updates the user
        /// </summary>
        /// <param name="user">User</param>
        public void UpdateUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            // Just incase, truncate the username
            if (user.UserName.Length > 50)
                user.UserName = user.UserName.Substring(0, 50);

            // Convert the username to a URL friendly version
            user.UserName = SeoExtensions.GetSeoName(user.UserName);

            _userRepository.Update(user);
        }

        /// <summary>
        /// Checks a user exists with a given oAuth profile Id
        /// </summary>
        /// <param name="profile">Profile Id</param>
        /// <param name="method">Authentication method</param>
        /// <returns>True/False</returns>
        public bool UserExists(string profile, AuthenticationMethod method)
        {
            if (string.IsNullOrWhiteSpace(profile))
                return false;

            var query = _userRepository.Table;

            switch (method)
            {
                case AuthenticationMethod.Facebook:
                    return query.Any(u => u.FacebookProfile == profile);
                case AuthenticationMethod.Twitter:
                    return query.Any(u => u.TwitterProfile == profile);
            }

            return false;
        }

        /// <summary>
        /// Check whether a username already exists
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="id">Exclude user id</param>
        /// <returns>True/False</returns>
        public bool UserNameExists(string userName, int id = 0)
        {
            if (string.IsNullOrEmpty(userName))
                return true;
            return _userRepository.Table.Any(x => x.UserName == userName && x.Id != id);
        }

        #endregion

        #region User Roles

        /// <summary>
        /// Delete a user role
        /// </summary>
        /// <param name="userRole">User role</param>
        public void DeleteUserRole(UserRole userRole)
        {
            if (userRole == null)
                throw new ArgumentNullException("userRole");

            if (userRole.IsSystemRole)
                throw new GatherException("System role could not be deleted");

            _userRoleRepository.Delete(userRole);
            UpdateUserRole(userRole);
        }

        /// <summary>
        /// Gets a user role
        /// </summary>
        /// <param name="userRoleId">User role identifier</param>
        /// <returns>User role</returns>
        public UserRole GetUserRoleById(int userRoleId)
        {
            if (userRoleId == 0)
                return null;

            var userRole = _userRoleRepository.GetById(userRoleId);
            return userRole;
        }

        /// <summary>
        /// Gets a user role
        /// </summary>
        /// <param name="systemName">User role system name</param>
        /// <returns>User role</returns>
        public UserRole GetUserRoleBySystemName(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var query = from ur in _userRoleRepository.Table
                        orderby ur.Id
                        where ur.SystemName == systemName
                        select ur;
            var userRole = query.FirstOrDefault();
            return userRole;
        }

        /// <summary>
        /// Get all user roles
        /// </summary>
        /// <param name="active">Active flag</param>
        /// <param name="search">Search term</param>
        /// <returns>Paginated user collection</returns>
        public IPaginatedList<UserRole> GetAllUserRoles(bool? active = true, string search = "")
        {
            return GetAllUserRoles(0, -1, active, search);
        }

        /// <summary>
        /// Get all user roles
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="active">Active flag</param>
        /// <param name="search">Search term</param>
        /// <returns>Paginated user collection</returns>
        public IPaginatedList<UserRole> GetAllUserRoles(int pageIndex, int pageSize, bool? active = true, string search = "")
        {
            var query = _userRoleRepository.Table;

            query = query.Where(u => !u.Deleted);

            if (active != null)
                query = query.Where(u => u.Active == active);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(u => u.Name.Contains(search) || u.SystemName.Contains(search));

            query = query.OrderBy(u => u.Name);

            var userRoles = new PaginatedList<UserRole>(query, pageIndex, pageSize);
            return userRoles;
        }

        /// <summary>
        /// Inserts a user role
        /// </summary>
        /// <param name="userRole">User role</param>
        public void InsertUserRole(UserRole userRole)
        {
            if (userRole == null)
                throw new ArgumentNullException("userRole");

            userRole.Active = true;
            userRole.CreatedDate = DateTime.Now;

            _userRoleRepository.Insert(userRole);
        }

        /// <summary>
        /// Updates the user role
        /// </summary>
        /// <param name="userRole">User role</param>
        public void UpdateUserRole(UserRole userRole)
        {
            if (userRole == null)
                throw new ArgumentNullException("userRole");

            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            userRole.LastModifiedBy = workContext.CurrentUser.Id;
            userRole.LastModifiedDate = DateTime.Now;

            _userRoleRepository.Update(userRole);
        }

        #endregion

        #endregion

    }
}