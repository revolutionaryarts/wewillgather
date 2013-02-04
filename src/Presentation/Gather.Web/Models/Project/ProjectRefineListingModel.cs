namespace Gather.Web.Models.Project
{
    public class ProjectRefineListingModel
    {
        public string CategoryFilters { get; set; }

        public string ListProjects { get; set; }

        public string LocationFilters { get; set; }

        public decimal MapLatitude { get; set; }

        public decimal MapLongitude { get; set; }

        public string MapProjects { get; set; }

        public int MapZoomLevel { get; set; }

        public string Paging { get; set; }

        public string TotalCount { get; set; }

        public int TotalResults { get; set; }
    }
}