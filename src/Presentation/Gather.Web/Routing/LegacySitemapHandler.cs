using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Gather.Core.Infrastructure;
using Gather.Services.Locations;

namespace Gather.Web.Routing
{
    public class LegacySitemapHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var locationService = EngineContext.Current.Resolve<ILocationService>();

            // Grab the response
            var response = requestContext.HttpContext.Response;
            
            // Grab the location name
            var locationSeoName = requestContext.RouteData.GetRequiredString("locationSeoName");

            // Find a location with the name canonical name
            var location = locationService.GetLocationBySeoName(locationSeoName);

            // If no location was found, send to 404
            if (location == null)
                return NotFound();

            string routeName;
            RouteValueDictionary routeValues = new RouteValueDictionary();

            if (location.ParentLocation != null && !string.IsNullOrEmpty(location.ParentLocation.SeoName))
            {
                routeName = "SiteMapParentLocation";
                routeValues.Add("parentSeoName", location.ParentLocation.SeoName);
                routeValues.Add("locationSeoName", location.SeoName);
            }
            else
            {
                routeName = "SiteMapLocation";
                routeValues.Add("locationSeoName", location.SeoName);
            }

            // Redirect to the new location page
            var urlHelper = new UrlHelper(requestContext);
            return Redirect(response, urlHelper.RouteUrl(routeName, routeValues));
        }

        private IHttpHandler NotFound()
        {
            throw new HttpException(404, "Page not found!");
        }

        private IHttpHandler Redirect(HttpResponseBase response, string url)
        {
            response.AddHeader("Location", url);
            response.StatusCode = 301;
            response.End();
            return null;
        }
    }
}