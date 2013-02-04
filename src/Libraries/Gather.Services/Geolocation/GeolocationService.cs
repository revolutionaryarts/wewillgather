using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json.Linq;
using Gather.Services.Locations;
using Gather.Core.Domain.Locations;

namespace Gather.Services.Geolocation
{
    public class GeolocationService : IGeolocationService
    {

        #region Fields

        private readonly ILocationService _locationService;

        #endregion

        #region Constructors

        public GeolocationService(ILocationService locationService)
        {
            _locationService = locationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the lat/lng for a location search query
        /// </summary>
        /// <param name="search">Search query</param>
        /// <param name="latitude">Latitude returned from map service</param>
        /// <param name="longitude">Longitude returned from map service</param>
        /// <param name="saveLocationTree">Ensures no infinate loop</param>
        public void GetLatLng(string search, out decimal latitude, out decimal longitude, bool saveLocationTree = true)
        {
            latitude = 999;
            longitude = 999;

            if (string.IsNullOrEmpty(search))
                return;

            var regex = new Regex("(([a-zA-Z]{2}[0-9]{1,2})( {0,1}[0-9][a-zA-Z]{2})?)");
            if (regex.IsMatch(search))
            {
                search = search.ToLower();
                if (!search.Contains(" ") && search.Length > 4)
                {
                    if (search.Length == 7) // e.g. BN111TH
                        search = search.Substring(0, 4) + " " + search.Substring(4, 3);
                    else // e.g. BN11TH
                        search = search.Substring(0, 3) + " " + search.Substring(3, 3);
                }
            }
            else
            {
                var location = _locationService.FindLocationByName(search);
                if (location != null)
                {
                    latitude = location.Latitude;
                    longitude = location.Longitude;
                    return;
                }
            }

            string queryUrl = string.Format("http://nominatim.openstreetmap.org/search?q={0}&format=json&addressdetails=1&limit=1&countrycodes=gb", HttpUtility.UrlEncode(search));

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(queryUrl);
                var response = (HttpWebResponse)request.GetResponse();
                var stream = response.GetResponseStream();

                if (stream != null)
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        var json = streamReader.ReadToEnd();
                        var coordinates = JArray.Parse(json);

                        if (coordinates.Count > 0)
                        {
                            var location = coordinates[0];

                            decimal tempLatitude, tempLongitude;
                            if (decimal.TryParse(location["lat"].ToString(), out tempLatitude) && decimal.TryParse(location["lon"].ToString(), out tempLongitude))
                            {
                                latitude = tempLatitude;
                                longitude = tempLongitude;
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Get the location associated with a lat/lng
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Latitude</param>
        public Location GetLocationFromLatLng(decimal latitude, decimal longitude)
        {
            Location location = null;

            try
            {
                location = _locationService.GetNearestLocation(latitude, longitude);
            }
            catch { }

            return location;
        }

        #endregion

    }
}