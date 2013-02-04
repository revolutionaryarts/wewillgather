using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Gather.Web.Framework.UI.Tabbing
{
    public class Tabs
    {

        #region Fields

        private readonly ViewContext _viewContext;
        private readonly RouteValueDictionary _linkWithoutPageValuesDictionary;
        private readonly Tabbing _tabs;

        #endregion

        #region Constructors

        public Tabs(ViewContext viewContext, Tabbing tabs, RouteValueDictionary valuesDictionary)
        {
            _viewContext = viewContext;
            _linkWithoutPageValuesDictionary = valuesDictionary;
            _tabs = tabs;
        }

        #endregion

        #region Methods

        public HtmlString RenderHtml()
        {

            var sb = new StringBuilder();

            if (_tabs.Tabs.Count > 0)
            {
                foreach (var tab in _tabs.Tabs)
                {
                    sb.Append(GeneratePageLink(tab.Name, _tabs.Param, tab.Value, _tabs.CurrentValue));
                }
            }

            return new HtmlString(sb.ToString());
        }

        private string GeneratePageLink(string linkText, string param, string value, string currentValue)
        {
            var routeDataValues = _viewContext.RequestContext.RouteData.Values;

            RouteValueDictionary pageLinkValueDictionary = new RouteValueDictionary(_linkWithoutPageValuesDictionary) { { param, value } };

            if (_viewContext.RequestContext.HttpContext.Request.QueryString["search"] != null)
            {
                pageLinkValueDictionary.Add("search", _viewContext.RequestContext.HttpContext.Request.QueryString["search"]);
            }

            if (!pageLinkValueDictionary.ContainsKey("id") && routeDataValues.ContainsKey("id"))
                pageLinkValueDictionary.Add("id", routeDataValues["id"]);

            // To be sure we get the right route, ensure the controller and action are specified.
            if (!pageLinkValueDictionary.ContainsKey("controller") && routeDataValues.ContainsKey("controller"))
                pageLinkValueDictionary.Add("controller", routeDataValues["controller"]);

            if (!pageLinkValueDictionary.ContainsKey("action") && routeDataValues.ContainsKey("action"))
                pageLinkValueDictionary.Add("action", routeDataValues["action"]);

            // 'Render' virtual path.
            var virtualPathForArea = RouteTable.Routes.GetVirtualPathForArea(_viewContext.RequestContext, pageLinkValueDictionary);

            if (virtualPathForArea == null)
                return null;

            var stringBuilder = new StringBuilder("<a");

            if (value == currentValue)
                stringBuilder.Append(" class=\"active\"");

            stringBuilder.AppendFormat(" href=\"{0}\">{1}</a>", virtualPathForArea.VirtualPath, linkText);

            return stringBuilder.ToString();
        }

        #endregion

    }

    #region Tabbing Class

    public class Tabbing
    {

        public Tabbing()
        {
            Tabs = new List<Tab>();
        }

        public string Param { get; set; }
        public List<Tab> Tabs { get; set; }
        public string CurrentValue { get; set; }
    }

    public class Tab
    {
        public Tab(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }
    }

    #endregion

}
