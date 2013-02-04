using System;
using System.Web;
using Gather.Core;
using Gather.Core.Domain.Users;
using Gather.Services.Authentication;
using Gather.Services.Users;

namespace Gather.Web.Framework
{
    public class WorkContext : IWorkContext
    {

        private readonly HttpContextBase _httpContext;
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;
        
        private User _cachedUser;

        public WorkContext(HttpContextBase httpContext, IUserService userService, IAuthenticationService authenticationService)
        {
            _httpContext = httpContext;
            _userService = userService;
            _authenticationService = authenticationService;
        }

        protected User GetCurrentUser()
        {
            if (_cachedUser != null)
                return _cachedUser;

            if (_httpContext != null)
            {
                var user = _authenticationService.GetAuthenticatedUser();
                if (user != null && user.Active)
                {
                    if (user.LastLoginDate.AddMinutes(1.0) < DateTime.Now)
                    {
                        user.LastLoginDate = DateTime.Now;
                        _userService.UpdateUser(user);
                    }

                    _cachedUser = user;
                }
            }

            return _cachedUser;
        }

        public User CurrentUser
        {
            get
            {
                return GetCurrentUser();
            }
            set
            {
                _cachedUser = value;
            }
        }

    }
}