using System.Collections.Generic;
using Gather.Core;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Users;
using Gather.Core.Domain.Projects;

namespace Gather.Services.Users
{
    public interface IUserService
    {

        #region Users

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="user">User</param>
        void DeleteUser(User user);

        /// <summary>
        /// Check whether an email already exists
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="id">Exclude user id</param>
        /// <returns>True/False</returns>
        bool EmailExists(string email, int id = 0);

        /// <summary>
        /// Get all users
        /// </summary>
        /// <param name="active">Active flag</param>
        /// <param name="search">Search term</param>
        /// <returns>Paginated user collection</returns>
        IPaginatedList<User> GetAllUsers(bool? active = true, string search = "");

        /// <summary>
        /// Get all users
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="active">Active flag</param>
        /// <param name="search">Search term</param>
        /// <returns>Paginated user collection</returns>
        IPaginatedList<User> GetAllUsers(int pageIndex, int pageSize, bool? active = true, string search = "");

        /// <summary>
        /// Get all users to display on the 'contact us' page
        /// </summary>
        /// <returns>Collection of users</returns>
        IList<User> GetAllUsersForContactUs();
            
        /// <summary>
        /// Get all users in a user role
        /// </summary>
        /// <param name="userRoleSystemName">UserRole system name</param>
        /// <returns>Collection of users</returns>
        IList<User> GetAllUsersInRole(string userRoleSystemName);

        /// <summary>
        /// Get the site owner
        /// </summary>
        /// <returns>User</returns>
        User GetSiteOwner();

        /// <summary>
        /// Get a user by id
        /// </summary>
        /// <param name="userId">Id of user to retrieve</param>
        /// <returns>User</returns>
        User GetUserById(int userId);

        /// <summary>
        /// Retrieve a user by their oAuth profile Id
        /// </summary>
        /// <param name="profile">Profile Id</param>
        /// <param name="method">Authentication method</param>
        /// <returns>User</returns>
        User GetUserByOAuthProfile(string profile, AuthenticationMethod method);

        /// <summary>
        /// Retrieve a user by their username
        /// </summary>
        /// <param name="userName">Username</param>
        /// <returns>User</returns>
        User GetUserByUserName(string userName);

        /// <summary>
        /// Insert a user
        /// </summary>
        /// <param name="user">User</param>
        void InsertUser(User user);

        /// <summary>
        /// Merge two users together
        /// </summary>
        /// <param name="primaryUser">Primary user entity</param>
        /// <param name="addUser">User entity to add</param>
        /// <param name="method">Authentication method</param>
        void MergeUsers(User primaryUser, User addUser, AuthenticationMethod method);

        /// <summary>
        /// Posts the information on twitter and facebook 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="project"> </param>
        /// <param name="projectAction"> </param>
        void Post(User user, Project project, ProjectAction projectAction);

        /// <summary>
        /// Updates the user
        /// </summary>
        /// <param name="user">User</param>
        void UpdateUser(User user);

        /// <summary>
        /// Checks a user exists with a given oAuth profile Id
        /// </summary>
        /// <param name="profile">Profile Id</param>
        /// <param name="method">Authentication method</param>
        /// <returns>True/False</returns>
        bool UserExists(string profile, AuthenticationMethod method);

        /// <summary>
        /// Check whether a username already exists
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="id">Exclude user id</param>
        /// <returns>True/False</returns>
        bool UserNameExists(string userName, int id = 0);

        #endregion

        #region User Roles

        /// <summary>
        /// Delete a user role
        /// </summary>
        /// <param name="userRole">User role</param>
        void DeleteUserRole(UserRole userRole);

        /// <summary>
        /// Gets a user role
        /// </summary>
        /// <param name="userRoleId">User role identifier</param>
        /// <returns>User role</returns>
        UserRole GetUserRoleById(int userRoleId);

        /// <summary>
        /// Gets a user role
        /// </summary>
        /// <param name="systemName">User role system name</param>
        /// <returns>User role</returns>
        UserRole GetUserRoleBySystemName(string systemName);

        /// <summary>
        /// Get all user roles
        /// </summary>
        /// <param name="active">Active flag</param>
        /// <param name="search">Search term</param>
        /// <returns>Paginated user collection</returns>
        IPaginatedList<UserRole> GetAllUserRoles(bool? active = true, string search = "");

        /// <summary>
        /// Get all user roles
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="active">Active flag</param>
        /// <param name="search">Search term</param>
        /// <returns>Paginated user collection</returns>
        IPaginatedList<UserRole> GetAllUserRoles(int pageIndex, int pageSize, bool? active = true, string search = "");

        /// <summary>
        /// Inserts a user role
        /// </summary>
        /// <param name="userRole">User role</param>
        void InsertUserRole(UserRole userRole);

        /// <summary>
        /// Updates the user role
        /// </summary>
        /// <param name="userRole">User role</param>
        void UpdateUserRole(UserRole userRole);

        #endregion

    }
}