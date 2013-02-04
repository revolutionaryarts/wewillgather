using System.Collections.Generic;
using Gather.Web.Areas.Admin.Models;

namespace Gather.Web.Models.Location
{
    public class LocationFilterModel : BaseModel
    {
        public LocationFilterModel()
        {
            ChildrenLocations = new List<LocationFilterModel>();
        }

        public IList<LocationFilterModel> ChildrenLocations { get; set; }

        public string Link { get; set; }

        public string Name { get; set; }

        //public string ParentSeoName { get; set; }

        public int ProjectCount { get; set; }
        
        //public string SeoName { get; set; }

    }
}