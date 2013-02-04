using System.Web.Mvc;
using System.Web.Mvc.Html;
using Gather.Web.Areas.Admin.Models.Common;

namespace Gather.Web.Areas.Admin.Extensions
{
    public static class HtmlExtensions
    {

        public static MvcHtmlString DeleteScript(this HtmlHelper html, string actionName = null, bool ajax = false, object routeValues = null)
        {
            return DeleteScript(html, null, null, actionName, ajax, routeValues);
        }

        public static MvcHtmlString DeleteScript(this HtmlHelper html, string additionalControllerName, string additionalActionName, string actionName = null, bool ajax = false, object routeValues = null)
        {
            if (actionName == null)
                actionName = "Delete";

            var model = new DeleteScriptModel
            {
                ActionName = actionName,
                AdditionalActionName = additionalActionName ?? "",
                AdditionalControllerName = additionalControllerName ?? "",
                Ajax = ajax,
                ControllerName = html.ViewContext.RouteData.GetRequiredString("controller"),
                Search = html.ViewContext.HttpContext.Request.QueryString["search"] ?? "",
                Page = html.ViewContext.HttpContext.Request.QueryString["page"] ?? "",
                RouteValues = routeValues
            };

            html.RenderPartial("_DeleteScript", model);

            return null;
        }

    }
}