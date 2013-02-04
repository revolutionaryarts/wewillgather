using System.Collections.Generic;
using Gather.Web.Models.Category;

namespace Gather.Web.Areas.Admin.Models.Category
{
    public class AdditionalDeleteModel
    {
        public IList<CategoryModel> Categories { get; set; }
        public int CategoryId { get; set; }
    }
}