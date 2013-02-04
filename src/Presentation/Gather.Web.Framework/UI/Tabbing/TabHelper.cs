using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Gather.Web.Framework.UI.Tabbing
{
    public class TabHelper : ITabHelper
    {

        #region Variables

        private string _param = "filter";

        #endregion

        #region Properties

        public string CurrentValue { private get; set; }

        public string Param
        {
            private get { return _param; }
            set { _param = value; }
        }

        public IList<Tab> Tabs { get; set; }

        #endregion

        #region Constructors

        public TabHelper()
        {
            Tabs = new List<Tab>();
        }

        #endregion

        #region Methods

        public void Add(string name, string value)
        {
            if(!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                Tabs.Add(new Tab(name, value));
        }

        public string GenerateTab(ref HtmlHelper html, string text, string value)
        {
            var routeDataValues = html.ViewContext.RequestContext.RouteData.Values;

            RouteValueDictionary pageLinkValueDictionary = new RouteValueDictionary { { Param, value } };

            if (html.ViewContext.RequestContext.HttpContext.Request.QueryString["search"] != null)
                pageLinkValueDictionary.Add("search", html.ViewContext.RequestContext.HttpContext.Request.QueryString["search"]);

            if (!pageLinkValueDictionary.ContainsKey("id") && routeDataValues.ContainsKey("id"))
                pageLinkValueDictionary.Add("id", routeDataValues["id"]);

            // To be sure we get the right route, ensure the controller and action are specified.
            if (!pageLinkValueDictionary.ContainsKey("controller") && routeDataValues.ContainsKey("controller"))
                pageLinkValueDictionary.Add("controller", routeDataValues["controller"]);

            if (!pageLinkValueDictionary.ContainsKey("action") && routeDataValues.ContainsKey("action"))
                pageLinkValueDictionary.Add("action", routeDataValues["action"]);

            // 'Render' virtual path.
            var virtualPathForArea = RouteTable.Routes.GetVirtualPathForArea(html.ViewContext.RequestContext, pageLinkValueDictionary);

            if (virtualPathForArea == null)
                return null;

            var stringBuilder = new StringBuilder("<li");

            if (value == CurrentValue)
                stringBuilder.Append(" class=active");

            stringBuilder.AppendFormat("><a href={0}>{1}</a></li>", virtualPathForArea.VirtualPath, text);

            return stringBuilder.ToString();
        }

        #endregion

    }
}