using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.Routing;
using Gather.Core.Infrastructure;
using Gather.Web.Framework.UI;
using Gather.Web.Framework.UI.Breadcrumbs;

namespace Gather.Web.Controllers
{
    public abstract class BaseController : Controller
    {

        #region Properties

        public int Page { get; protected set; }

        #endregion

        #region Constructors

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!string.IsNullOrEmpty(filterContext.HttpContext.Request.QueryString["page"]))
                Page = int.Parse(filterContext.HttpContext.Request.QueryString["page"]);

            if (Page == 0)
                Page = 1;
        }

        #endregion

        #region Methods

        protected void AddBreadcrumb(string title, string target, int? index = null)
        {
            var breadcrumbHelper = EngineContext.Current.Resolve<IBreadcrumbHelper>();
            breadcrumbHelper.Add(title, target, index);
        }

        protected void AddBreadcrumb(string title, string targetAction, string targetController, int? index = null)
        {
            AddBreadcrumb(title, targetAction, targetController, null, null);
        }

        protected void AddBreadcrumb(string title, string targetAction, string targetController, object routeValues, int? index = null)
        {
            HttpContextWrapper httpContextWrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
            UrlHelper urlHelper = new UrlHelper(new RequestContext(httpContextWrapper, new RouteData()));

            AddBreadcrumb(title, urlHelper.Action(targetAction, targetController, routeValues), index);
        }

        protected void AddBreadcrumb(string title, string targetAction, string targetController, RouteValueDictionary routeValues, int? index = null)
        {
            HttpContextWrapper httpContextWrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
            UrlHelper urlHelper = new UrlHelper(new RequestContext(httpContextWrapper, new RouteData()));

            AddBreadcrumb(title, urlHelper.Action(targetAction, targetController, routeValues), index);
        }

        protected void AddHomeBreadcrumb()
        {
            AddBreadcrumb("Home", Url.Action("Index", "Home"), 0);
        }

        protected void AddNotification(NotifyType type, string message, bool persistForTheNextRequest)
        {
            string dataKey = string.Format("gather.notifications.{0}", type);
            if (persistForTheNextRequest)
            {
                if (TempData[dataKey] == null)
                    TempData[dataKey] = new List<string>();
                ((List<string>) TempData[dataKey]).Add(message);
            }
            else
            {
                if (ViewData[dataKey] == null)
                    ViewData[dataKey] = new List<string>();
                ((List<string>)ViewData[dataKey]).Add(message);
            }
        }

        protected void ErrorNotification(string message, bool persistForTheNextRequest = true)
        {
            AddNotification(NotifyType.Error, message, persistForTheNextRequest);
        }

        protected void ErrorNotification(Exception exception, bool persistForTheNextRequest = true)
        {
            AddNotification(NotifyType.Error, exception.Message, persistForTheNextRequest);
        }

        protected string RenderRazorViewToString(string viewName)
        {
            return RenderRazorViewToString(viewName, null);
        }

        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        protected void SuccessNotification(string message, bool persistForTheNextRequest = true)
        {
            AddNotification(NotifyType.Success, message, persistForTheNextRequest);
        }

        protected void WarningNotification(string message, bool persistForTheNextRequest = true)
        {
            AddNotification(NotifyType.Warning, message, persistForTheNextRequest);
        }

        #endregion

    }
}