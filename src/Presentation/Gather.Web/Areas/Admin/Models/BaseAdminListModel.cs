using Gather.Web.Framework.UI.Tabbing;

namespace Gather.Web.Areas.Admin.Models
{
    public class BaseAdminListModel
    {
        public string Filter { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public string Search { get; set; }
        public Tabbing Tabs { get; set; }
    }
}