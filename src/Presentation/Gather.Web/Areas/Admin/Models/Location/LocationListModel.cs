using System.Collections.Generic;
using Gather.Web.Framework.Mvc;
using Gather.Web.Models.Location;

namespace Gather.Web.Areas.Admin.Models.Location
{
    public class LocationListModel : BaseAdminListModel
    {
        public IList<LocationModel> Locations { get; set; }
    }
}