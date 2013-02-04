using Gather.Web.Areas.Admin.Models;

namespace Gather.Web.Models.Category
{
    public class CategoryFilterModel : BaseModel
    {
        public string AddLink { get; set; }

        public bool IsChecked { get; set; }

        public bool IsEnabled { get; set; }

        public string Name { get; set; }

        public int ProjectCount { get; set; }

        public string RemoveLink { get; set; }
    }
}