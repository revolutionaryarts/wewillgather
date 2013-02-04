using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Gather.Web.Framework.UI.Paging
{
    public class Pager
    {

        #region Fields

        private readonly ViewContext _viewContext;
        private readonly int _pageSize;
        private readonly int _currentPage;
        private readonly int _totalItemCount;
        private readonly RouteValueDictionary _linkWithoutPageValuesDictionary;

        #endregion

        #region Constructors

        public Pager(ViewContext viewContext, int pageSize, int currentPage, int totalItemCount, RouteValueDictionary valuesDictionary)
        {
            _viewContext = viewContext;
            _pageSize = pageSize;
            _currentPage = currentPage;
            _totalItemCount = totalItemCount;
            _linkWithoutPageValuesDictionary = valuesDictionary;
        }

        #endregion

        #region Methods

        public HtmlString RenderHtml()
        {
            var pageCount = (int)Math.Ceiling(_totalItemCount / (double)_pageSize);
            const int nrOfPagesToDisplay = 10;

            var sb = new StringBuilder();

            sb.Append("<span>" + _totalItemCount + " result" + (_totalItemCount != 1 ? "s" : "") + "</span>");

            if (pageCount > 1)
            {
                sb.Append("<span class=\"page-label\">Pages: </span>");

                // Previous
                sb.Append(_currentPage > 1 ? GeneratePageLink("<span class=\"prev\">Previous</span>", _currentPage - 1) : "<span class=\"prev disabled\">Previous</span>");

                var start = 1;
                var end = pageCount;

                if (pageCount > nrOfPagesToDisplay)
                {
                    var middle = (int)Math.Ceiling(nrOfPagesToDisplay / 2d) - 1;
                    var below = (_currentPage - middle);
                    var above = (_currentPage + middle);

                    if (below < 4)
                    {
                        above = nrOfPagesToDisplay;
                        below = 1;
                    }
                    else if (above > (pageCount - 4))
                    {
                        above = pageCount;
                        below = (pageCount - nrOfPagesToDisplay + 1);
                    }

                    start = below;
                    end = above;
                }

                if (start > 1)
                {
                    sb.Append(GeneratePageLink("1", 1));

                    if (start > 3)
                        sb.Append(GeneratePageLink("2", 2));

                    if (start > 2)
                        sb.Append("<span class=\"dots\">...</span>");
                }

                for (var i = start; i <= end; i++)
                {
                    if (i == _currentPage || (_currentPage <= 0 && i == 0))
                        sb.AppendFormat("<span class=\"current\">{0}</span>", i);
                    else
                        sb.Append(GeneratePageLink(i.ToString(CultureInfo.InvariantCulture), i));
                }

                if (end < pageCount)
                {
                    if (end < pageCount - 1)
                        sb.Append("<span class=\"dots\">...</span>");

                    if (pageCount - 2 > end)
                        sb.Append(GeneratePageLink((pageCount - 1).ToString(CultureInfo.InvariantCulture), pageCount - 1));

                    sb.Append(GeneratePageLink(pageCount.ToString(CultureInfo.InvariantCulture), pageCount));
                }

                // Next
                sb.Append(_currentPage < pageCount ? GeneratePageLink("<span class=\"next\">Next</span>", (_currentPage + 1)) : "<span class=\"next disabled\">Next</span>");
            }

            return new HtmlString(sb.ToString());
        }

        private string GeneratePageLink(string linkText, int pageNumber)
        {
            var routeDataValues = _viewContext.RequestContext.RouteData.Values;
            RouteValueDictionary pageLinkValueDictionary = new RouteValueDictionary(_linkWithoutPageValuesDictionary);

            // Get all the route values e.g. controller, action, id
            foreach (var routeValue in routeDataValues.Where(x => !pageLinkValueDictionary.ContainsKey(x.Key)))
                pageLinkValueDictionary.Add(routeValue.Key, routeValue.Value);

            // Get all the querystring parameters
            foreach (var parameter in _viewContext.RequestContext.HttpContext.Request.QueryString.AllKeys.Where(x => !string.IsNullOrEmpty(x)).Select(x => x.ToString()).Where(x => !pageLinkValueDictionary.ContainsKey(x)))
                pageLinkValueDictionary.Add(parameter, _viewContext.RequestContext.HttpContext.Request.QueryString[parameter]);

            // Avoid canonical errors when page count is equal to 1.
            if (pageNumber == 1 && pageLinkValueDictionary.ContainsKey("page"))
                pageLinkValueDictionary.Remove("page");
            else
                pageLinkValueDictionary["page"] = pageNumber;

            // 'Render' virtual path.
            var virtualPathForArea = RouteTable.Routes.GetVirtualPathForArea(_viewContext.RequestContext, pageLinkValueDictionary);
            if (virtualPathForArea == null)
                return null;

            return string.Format("<a href=\"{0}\" data-page=\"{1}\">{2}</a>", virtualPathForArea.VirtualPath, pageNumber, linkText);
        }

        #endregion

    }
}