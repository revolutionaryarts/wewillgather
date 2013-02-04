using System.Collections.Generic;
using Gather.Web.Models.Page;

namespace Gather.Web.Areas.Admin.Models.Page
{
    public class PageListModel : BaseAdminListModel
    {
        public IList<PageModel> Pages { get; set; }
    }
}