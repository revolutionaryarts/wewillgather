using Gather.Core.Domain.Locations;

namespace Gather.Services.Geolocation
{
    public interface IGeolocationService
    {

        /// <summary>
        /// Get the lat/lng for a location search query
        /// </summary>
        /// <param name="search">Search query</param>
        /// <param name="latitude">Latitude returned from map service</param>
        /// <param name="longitude">Longitude returned from map service</param>
        /// <param name="saveLocationTree">Ensures no infinate loop </param>
        void GetLatLng(string search, out decimal latitude, out decimal longitude, bool saveLocationTree = true);

        /// <summary>
        /// Get the location associated with a lat/lng
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Latitude</param>
        Location GetLocationFromLatLng(decimal latitude, decimal longitude);

    }
}