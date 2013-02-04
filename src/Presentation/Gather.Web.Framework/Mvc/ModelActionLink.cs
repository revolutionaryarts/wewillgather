using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Gather.Web.Framework.Mvc
{
    public class ModelActionLink
    {
        public ModelActionLink()
        {
            AdditionalAttributes = new Dictionary<string, string>();
            Classes = new List<string>();
        }

        public IDictionary<string, string> AdditionalAttributes { get; set; }
        public string Alt { get; set; }
        public IList<string> Classes { get; set; }
        public string Icon { get; set; }
        public string Target { get; set; }
    }

    public class DeleteActionLink : ModelActionLink
    {
        public DeleteActionLink(int id, string search = "", int page = 0 )
        {
            HttpContextWrapper httpContextWrapper = new HttpContextWrapper(HttpContext.Current);
            UrlHelper urlHelper = new UrlHelper(new RequestContext(httpContextWrapper, RouteTable.Routes.GetRouteData(httpContextWrapper)));

            AdditionalAttributes.Add("data-id", id.ToString());
            Alt = "Delete";
            Classes.Add("delete-link");
            Icon = urlHelper.Content("~/Areas/Admin/Content/images/icon-delete.png");
            Target = urlHelper.Action("delete", new { id, search, page });
        }
    }
}