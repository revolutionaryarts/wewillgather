using System.Text;
using System.Web;
using System.Web.Mvc;
using Gather.Core.Infrastructure;

namespace Gather.Web.Framework.UI.Tabbing
{
    public static class TabExtensions
    {

        #region Methods

        public static HtmlString Tabs(this HtmlHelper html)
        {
            var tabHelper = EngineContext.Current.Resolve<ITabHelper>();
            var sb = new StringBuilder();

            if(tabHelper.Tabs.Count > 0)
            {
                sb.Append("<div class=tabs><ul>");

                foreach(var tab in tabHelper.Tabs)
                    sb.Append(tabHelper.GenerateTab(ref html, tab.Name, tab.Value));

                sb.Append("</ul></div>");
            }
            
            return new HtmlString(sb.ToString());
        }

        #endregion

    }
}