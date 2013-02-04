using System.Collections.Generic;
using System.Web.Mvc;
using Gather.Web.Areas.Admin.Models.Common;
using Gather.Web.Controllers;

namespace Gather.Web.Areas.Admin.Controllers
{
    public abstract class ModuleController : BaseController
    {

        #region Properties

        public string Search { get; private set; }
        public string Filter { get; set; }

        #endregion

        #region Methods

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (!string.IsNullOrEmpty(filterContext.HttpContext.Request.QueryString["search"]))
                Search = filterContext.HttpContext.Request.QueryString["search"];

            if (!string.IsNullOrEmpty(filterContext.HttpContext.Request.QueryString["filter"]))
                Filter = filterContext.HttpContext.Request.QueryString["filter"];
        }

        public ActionResult AccessDeniedView()
        {
            return RedirectToAction("accessdenied", "common");
        }

        #endregion

        #region Navigation

        public virtual IList<NavigationSectionModel> RegisterNavigation()
        {
            return new List<NavigationSectionModel>();
        }

        #endregion

    }
}