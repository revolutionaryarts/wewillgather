using System.Collections.Generic;
using System.Web.Mvc;
using Gather.Web.Models.Category;
using Gather.Web.Models.Location;

namespace Gather.Web.Models.Project
{
    public class ProjectListingModel
    {
        public ProjectListingModel()
        {
            AvailableChildFriendly = new List<SelectListItem>();
            AvailableSearchRadius = new List<SelectListItem>();
            AvailableSearchStart = new List<SelectListItem>();
            AvailableSortDirections = new List<SelectListItem>();
            AvailableSortTypes = new List<SelectListItem>();
            CategoryFilters = new List<CategoryFilterModel>();
        }

        public IList<SelectListItem> AvailableChildFriendly { get; set; }

        public IList<SelectListItem> AvailableSearchRadius { get; set; }

        public IList<SelectListItem> AvailableSearchStart { get; set; }

        public IList<SelectListItem> AvailableSortDirections { get; set; }

        public IList<SelectListItem> AvailableSortTypes { get; set; }

        public string CanonicalUrl { get; set; }

        public IList<CategoryFilterModel> CategoryFilters { get; set; }

        public bool IsSearch { get; set; }

        public LocationModel Location { get; set; }

        public LocationFilterModel LocationFilters { get; set; }

        public decimal MapLatitude { get; set; }

        public decimal MapLongitude { get; set; }

        public string MapProjects { get; set; }

        public int MapZoomLevel { get; set; }

        public string MetaDescription { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaTitle { get; set; }

        public ProjectListPagedModel PagedModel { get; set; }

        public string RssLink { get; set; }

        public int TotalCount { get; set; }
    }
}