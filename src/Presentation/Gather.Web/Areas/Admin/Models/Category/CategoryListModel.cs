using System.Collections.Generic;
using Gather.Web.Framework.Mvc;
using Gather.Web.Models.Category;

namespace Gather.Web.Areas.Admin.Models.Category
{
    public class CategoryListModel : BaseAdminListModel
    {
        public IList<CategoryModel> Categories { get; set; }
    }
}