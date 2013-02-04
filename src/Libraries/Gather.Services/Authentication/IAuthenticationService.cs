using System.Web.Security;
using Gather.Core.Domain.Users;

namespace Gather.Services.Authentication
{
    public interface IAuthenticationService
    {

        /// <summary>
        /// Signs in a new user
        /// </summary>
        /// <param name="user">User entity</param>
        /// <param name="createPersistentCookie">True/false</param>
        void SignIn(User user, bool createPersistentCookie);

        /// <summary>
        /// Signs out the current authenticated user
        /// </summary>
        void SignOut();

        /// <summary>
        /// Return the current authenticated user
        /// </summary>
        /// <returns>User</returns>
        User GetAuthenticatedUser();

        /// <summary>
        /// Return the current authenticated user from the forms ticket
        /// </summary>
        /// <param name="ticket">Forms authentication ticket</param>
        /// <returns>User</returns>
        User GetAuthenticatedUserFromTicket(FormsAuthenticationTicket ticket);

    }
}