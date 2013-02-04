using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Gather.Web.Areas.Admin.Models.Common;
using Gather.Web.Framework.Controllers;

namespace Gather.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class HomeController : ModuleController
    {

        #region Methods

        public ActionResult Index()
        {
            return RedirectToAction("index", "moderation");
            //AddBreadcrumb("Dashboard", Url.Action("index", new { area = "admin" }));
            //return View();
        }
        
        #endregion

        //#region Navigation

        //public override IList<NavigationSectionModel> RegisterNavigation()
        //{
        //    var sections = new List<NavigationSectionModel>();

        //    HttpContextWrapper httpContextWrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
        //    UrlHelper urlHelper = new UrlHelper(new RequestContext(httpContextWrapper, new RouteData()));

        //    var section = new NavigationSectionModel
        //    {
        //        Icon = urlHelper.Content("~/Areas/Admin/Content/Images/menu-dashboard.png"),
        //        Name = "dashboard",
        //        Target = urlHelper.Action("index", "home", new { area = "admin" }),
        //        Title = "Dashboard"
        //    };

        //    sections.Add(section);

        //    return sections;
        //}

        //#endregion

    }
}