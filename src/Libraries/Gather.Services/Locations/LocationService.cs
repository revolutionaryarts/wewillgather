using System;
using System.Collections.Generic;
using System.Linq;
using Gather.Core;
using Gather.Core.Cache;
using Gather.Core.Data;
using Gather.Core.Domain.Locations;

namespace Gather.Services.Locations
{
    public class LocationService : ILocationService
    {

        #region Constants

        private const string LOCATIONS_ALL_KEY = "Gather.locations.all";

        #endregion

        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IRepository<Location> _locationRepository;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Constructors

        public LocationService(ICacheManager cacheManager, IRepository<Location> locationRepository, IWebHelper webHelper)
        {
            _cacheManager = cacheManager;
            _locationRepository = locationRepository;
            _webHelper = webHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clear cache
        /// </summary>
        public void ClearCache()
        {
            _cacheManager.RemoveByPattern(LOCATIONS_ALL_KEY);
        }

        /// <summary>
        /// Delete a location
        /// </summary>
        /// <param name="location">Location</param>
        public void DeleteLocation(Location location)
        {
            if (location == null)
                throw new ArgumentNullException("location");

            if (location.Deleted)
                return;

            _locationRepository.Delete(location);

            ClearCache();
        }

        /// <summary>
        /// Try and find a location by name
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="alwaysTakeFirst">Flag to return the first location, regardless of how many are found</param>
        /// <returns>Location</returns>
        public Location FindLocationByName(string name, bool alwaysTakeFirst = false)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            // Get all the locations matching the name
            var locations = GetAllCachedLocations().Where(x => x.Name.ToLower() == name.ToLower()).ToList();

            // If we only found one, return it
            if (alwaysTakeFirst || locations.Count() == 1)
                return locations.FirstOrDefault();

            // Otherwise, leave it up to geolocator
            return null;
        }

        /// <summary>
        /// Get all cached locations
        /// </summary>
        /// <returns>Collection of locations</returns>
        public IList<Location> GetAllCachedLocations()
        {
            return _cacheManager.Get(LOCATIONS_ALL_KEY, () =>
            {
                var query = from l in _locationRepository.Table
                            where l.Active && !l.Deleted
                            select l;
                return query.ToList();
            });
        }

        /// <summary>
        /// Get all child locations of a given parent
        /// </summary>
        /// <param name="parentId">Parent location id</param>
        /// <returns>Collection of locations</returns>
        public IList<Location> GetAllChildLocations(int parentId = 0)
        {
            var query = from l in GetAllCachedLocations()
                        where (l.ParentLocation == null && parentId == 0) || (l.ParentLocation != null && l.ParentLocation.Id == parentId)
                        select l;
            return query.ToList();
        }

        /// <summary>
        /// Get all Locations
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="id">Id</param>
        /// <param name="active">Active flag</param>
        /// <param name="search">Search param</param>
        /// <returns>Paginated location collection</returns>
        public IPaginatedList<Location> GetAllLocations(int pageIndex = 0, int pageSize = -1, int id = 0, bool? active = true, string search = "")
        {
            var query = _locationRepository.Table;

            query = query.Where(u => !u.Deleted);

            if (active != null)
                query = query.Where(u => u.Active == active);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(u => u.Name.Contains(search));
            else
                query = id == 0 ? query.Where(u => u.ParentLocation == null) : query.Where(u => u.ParentLocation.Id == id);

            query = query.OrderBy(u => u.Name);

            var locations = new PaginatedList<Location>(query, pageIndex, pageSize);
            return locations;
        }

        /// <summary>
        /// Get a location by id
        /// </summary>
        /// <param name="locationId">Id of location to retrieve</param>
        /// <returns>Location</returns>
        public Location GetLocationById(int locationId)
        {
            if (locationId == 0)
                return null;

            var location = _locationRepository.GetById(locationId);
            return location;
        }

        /// <summary>
        /// Get a location by seo name
        /// </summary>
        /// <param name="seoName">Seo name</param>
        /// /// <param name="parentSeoName">Parent seo name</param>
        /// <returns>Location</returns>
        public Location GetLocationBySeoName(string seoName, string parentSeoName = null)
        {
            if (string.IsNullOrEmpty(seoName))
                return null;

            // Get all the locations with the matching seo name
            var locations = GetAllCachedLocations().Where(l => l.SeoName == seoName);

            // If we have a parent name, get the first location with the same parent
            if (parentSeoName != null)
                return locations.FirstOrDefault(x => x.ParentLocation != null && x.ParentLocation.SeoName == parentSeoName);

            // If we don't have a parent name, just return the first one
            return locations.FirstOrDefault();
        }

        /// <summary>
        /// Get a list of child location ids
        /// </summary>
        /// <param name="parent">Parent location</param>
        /// <returns>Collection of child ids</returns>
        public IList<int> GetLocationChildIds(Location parent)
        {
            IList<int> childrenLocations = new List<int>();

            if (parent.ChildLocations.Count == 0)
            {
                childrenLocations.Add(parent.Id);
                return childrenLocations;
            }

            return parent.ChildLocations.Aggregate(childrenLocations, (current, child) => current.Union(GetLocationChildIds(child)).ToList());
        }

        /// <summary>
        /// Get the closest location to a lat/lon point
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="radius">Radius</param>
        /// <returns>Location</returns>
        public Location GetNearestLocation(decimal latitude, decimal longitude, int radius = 5)
        {
            if (radius <= 0)
                return null;

            if (latitude == 0 || longitude == 0)
                return null;

            var query = from p in GetAllCachedLocations().ToList()
                        let distance = _webHelper.Haversine((double)latitude, (double)longitude, (double)p.Latitude, (double)p.Longitude, DistanceType.Miles)
                        where distance <= radius
                        orderby distance
                        select p;
            return query.FirstOrDefault();
        }

        /// <summary>
        /// Get a list of region locations
        /// </summary>
        /// <returns>Collection of region locations</returns>
        public IList<Location> GetRegionLocations()
        {
            var locations = GetAllCachedLocations().Where(x => x.IsRegion && x.Name.ToLower() != "england").OrderBy(x => x.Name).Take(12).ToList();
            return locations;
        }

        /// <summary>
        /// Updates the Location
        /// </summary>
        /// <param name="location">Location</param>
        public void UpdateLocation(Location location)
        {
            if (location == null)
                throw new ArgumentNullException("location");

            location.LastModifiedDate = DateTime.Now;

            _locationRepository.Update(location);

            ClearCache();
        }

        #endregion

    }
}