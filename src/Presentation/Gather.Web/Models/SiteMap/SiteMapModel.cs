using System.Collections.Generic;
using Gather.Web.Areas.Admin.Models;
using Gather.Web.Models.Location;
using Gather.Web.Models.Page;
using Gather.Web.Models.Project;

namespace Gather.Web.Models.SiteMap
{
    public class SiteMapModel : BaseModel
    {
        public bool BaseLocation { get; set; }
        public string HashTag { get; set; }
        public IList<LocationModel> Locations { get; set; }
        public IList<PageModel> Pages { get; set; }
        public IList<BaseProjectModel> Projects { get; set; }
        public string TitleName { get; set; }
        public bool TopLocation { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaTitle { get; set; }
        public bool NoIndex { get; set; }
    }
}