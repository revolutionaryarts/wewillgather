using System;
using System.Web.Mvc;
using Gather.Core.Infrastructure;
using Gather.Services.Security;

namespace Gather.Web.Framework.Controllers
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AdminAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        private void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }

        private bool HasAdminAccess()
        {
            var permissionService = EngineContext.Current.Resolve<IPermissionService>();
            bool result = permissionService.Authorize(PermissionProvider.AccessAdminArea);
            return result;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            if (OutputCacheAttribute.IsChildActionCacheActive(filterContext))
                throw new InvalidOperationException("You cannot use the [AdminAuthorize] attribute when a child action cache is active");

            if (!HasAdminAccess())
                HandleUnauthorizedRequest(filterContext);
        }
    }
}