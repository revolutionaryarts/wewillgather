using System;
using System.Web;
using System.Web.Security;
using Gather.Core.Domain.Users;
using Gather.Services.Logging;
using Gather.Services.Users;

namespace Gather.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {

        #region Fields

        private readonly HttpContextBase _httpContext;
        private readonly ILogService _logService;
        private readonly IUserService _userService;

        private User _cachedUser;

        #endregion

        #region Constructors

        public AuthenticationService(HttpContextBase httpContext, ILogService logService, IUserService userService)
        {
            _httpContext = httpContext;
            _logService = logService;
            _userService = userService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Signs in a new user
        /// </summary>
        /// <param name="user">User entity</param>
        /// <param name="createPersistentCookie">True/false</param>
        public void SignIn(User user, bool createPersistentCookie)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var ticket = new FormsAuthenticationTicket(
                1,
                user.DisplayName, 
                now, 
                now.Add(FormsAuthentication.Timeout), 
                createPersistentCookie, 
                user.Id.ToString(), 
                FormsAuthentication.FormsCookiePath
            );

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Expires = now.Add(FormsAuthentication.Timeout),
                Secure = FormsAuthentication.RequireSSL,
                Path = FormsAuthentication.FormsCookiePath
            };

            if (FormsAuthentication.CookieDomain != null)
                cookie.Domain = FormsAuthentication.CookieDomain;

            _httpContext.Response.Cookies.Add(cookie);
            _cachedUser = user;
        }

        /// <summary>
        /// Signs out the current authenticated user
        /// </summary>
        public void SignOut()
        {
            _cachedUser = null;
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// Return the current authenticated user
        /// </summary>
        /// <returns>User</returns>
        public User GetAuthenticatedUser()
        {
            if (_cachedUser != null)
                return _cachedUser;

            if (_httpContext == null)
                return null;

            if (_httpContext.Request == null)
                return null;

            if (!_httpContext.Request.IsAuthenticated)
                return null;

            if (!(_httpContext.User.Identity is FormsIdentity))
                return null;

            var formsIdentity = (FormsIdentity)_httpContext.User.Identity;
            var user = GetAuthenticatedUserFromTicket(formsIdentity.Ticket);
            if (user != null && user.Active)
                _cachedUser = user;
            return _cachedUser;
        }

        /// <summary>
        /// Return the current authenticated user from the forms ticket
        /// </summary>
        /// <param name="ticket">Forms authentication ticket</param>
        /// <returns>User</returns>
        public User GetAuthenticatedUserFromTicket(FormsAuthenticationTicket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            if (string.IsNullOrEmpty(ticket.UserData))
                return null;

            int userId;
            if (!int.TryParse(ticket.UserData, out userId))
                return null;

            var user = _userService.GetUserById(userId);
            return user;
        }

        #endregion

    }
}