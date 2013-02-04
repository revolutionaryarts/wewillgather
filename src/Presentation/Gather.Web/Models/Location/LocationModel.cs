using System;
using System.ComponentModel.DataAnnotations;
using Gather.Web.Areas.Admin.Models;

namespace Gather.Web.Models.Location
{
    public class LocationModel : BaseModel
    {
        public bool Active { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool Deleted { get; set; }

        [Display(Name = "Hash Tag")]
        public string HashTag { get; set; }

        public int? LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string Name { get; set; }

        public LocationModel ParentLocation { get; set; }

        public int ProjectCount { get; set; }

        public string SeoName { get; set; }
    }
}
