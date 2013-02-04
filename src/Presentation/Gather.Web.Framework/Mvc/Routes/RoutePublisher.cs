using System;
using System.Linq;
using System.Web.Routing;
using Gather.Core.Infrastructure;

namespace Gather.Web.Framework.Mvc.Routes
{
    public class RoutePublisher : IRoutePublisher
    {
        private readonly ITypeFinder _typeFinder;

        public RoutePublisher(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            var routeProviderTypes = _typeFinder.FindClassesOfType<IRouteProvider>();
            var routeProviders = routeProviderTypes.Select(providerType => Activator.CreateInstance(providerType) as IRouteProvider).ToList();
            routeProviders = routeProviders.OrderByDescending(rp => rp.Priority).ToList();
            routeProviders.ForEach(rp => rp.RegisterRoutes(routes));
        }
    }
}