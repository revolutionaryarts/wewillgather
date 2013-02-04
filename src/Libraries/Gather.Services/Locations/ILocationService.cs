using System.Collections.Generic;
using Gather.Core;
using Gather.Core.Domain.Locations;

namespace Gather.Services.Locations
{
    public interface ILocationService
    {

        /// <summary>
        /// Clear cache
        /// </summary>
        void ClearCache();

        /// <summary>
        /// Delete a location
        /// </summary>
        /// <param name="location">Location</param>
        void DeleteLocation(Location location);

        /// <summary>
        /// Try and find a location by name
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="alwaysTakeFirst">Flag to return the first location, regardless of how many are found</param>
        /// <returns>Location</returns>
        Location FindLocationByName(string name, bool alwaysTakeFirst = false);

        /// <summary>
        /// Get all cached locations
        /// </summary>
        /// <returns>Collection of locations</returns>
        IList<Location> GetAllCachedLocations();

        /// <summary>
        /// Get all child locations of a given parent
        /// </summary>
        /// <param name="parentId">Parent location id</param>
        /// <returns>Collection of locations</returns>
        IList<Location> GetAllChildLocations(int parentId = 0);
            
        /// <summary>
        /// Get all Locations
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="id">Id</param>
        /// <param name="active">Active flag</param>
        /// <param name="search">Search param</param>
        /// <returns>Paginated location collection</returns>
        IPaginatedList<Location> GetAllLocations(int pageIndex = 0, int pageSize = -1, int id = 0, bool? active = true, string search = "");

        /// <summary>
        /// Get a location by id
        /// </summary>
        /// <param name="locationId">Id of location to retrieve</param>
        /// <returns>Location</returns>
        Location GetLocationById(int locationId);

        /// <summary>
        /// Get a location by seo name
        /// </summary>
        /// <param name="seoName">Seo name</param>
        /// <param name="parentSeoName">Parent seo name</param>
        /// <returns>Location</returns>
        Location GetLocationBySeoName(string seoName, string parentSeoName = null);

        /// <summary>
        /// Get a list of child location ids
        /// </summary>
        /// <param name="parent">Parent location</param>
        /// <returns>Collection of child ids</returns>
        IList<int> GetLocationChildIds(Location parent);

        /// <summary>
        /// Get the closest location to a lat/lon point
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="radius">Radius</param>
        /// <returns>Location</returns>
        Location GetNearestLocation(decimal latitude, decimal longitude, int radius = 5);

        /// <summary>
        /// Get a list of region locations
        /// </summary>
        /// <returns>Collection of region locations</returns>
        IList<Location> GetRegionLocations();

        /// <summary>
        /// Updates the Location
        /// </summary>
        /// <param name="location"></param>
        void UpdateLocation(Location location);

    }
}