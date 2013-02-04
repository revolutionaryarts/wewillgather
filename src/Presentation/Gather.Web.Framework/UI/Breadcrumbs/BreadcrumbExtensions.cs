using System.Text;
using System.Web;
using System.Web.Mvc;
using Gather.Core.Infrastructure;

namespace Gather.Web.Framework.UI.Breadcrumbs
{
    public static class BreadcrumbExtensions
    {

        #region Methods

        public static HtmlString Breadcrumbs(this HtmlHelper html)
        {
            var breadcrumbHelper = EngineContext.Current.Resolve<IBreadcrumbHelper>();
            var sb = new StringBuilder();

            foreach(var breadcrumb in breadcrumbHelper.Breadcrumbs)
            {
                sb.Append("<li itemscope itemtype=\"http://data-vocabulary.org/Breadcrumb\">");
                sb.Append("<a href=\"" + breadcrumb.Target + "\" itemprop=\"url\"><span itemprop=\"title\">" + breadcrumb.Title + "</span></a>");
                sb.Append("</li>");
            }

            return new HtmlString(sb.ToString());
        }

        #endregion

    }
}