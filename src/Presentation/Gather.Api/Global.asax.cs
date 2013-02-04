using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Gather.Api.App_Start;
using Gather.Api.Attributes;
using Gather.Core.Infrastructure;

namespace Gather.Api
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            // Load custom errors policy
            IncludeErrorDetailPolicy errorDetailPolicy;
            var config = (CustomErrorsSection)ConfigurationManager.GetSection("system.web/customErrors");
            switch (config.Mode)
            {
                case CustomErrorsMode.RemoteOnly:
                    errorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;
                    break;
                case CustomErrorsMode.On:
                    errorDetailPolicy = IncludeErrorDetailPolicy.Never;
                    break;
                case CustomErrorsMode.Off:
                    errorDetailPolicy = IncludeErrorDetailPolicy.Always;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            GlobalConfiguration.Configuration.IncludeErrorDetailPolicy = errorDetailPolicy;

            // Initialize engine
            EngineContext.Initialize(false);

            // Resolve dependency
            var resolver = new GatherDependencyResolver();
            GlobalConfiguration.Configuration.DependencyResolver = resolver;

            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            GlobalConfiguration.Configuration.Filters.Add(new TokenValidationAttribute());
            GlobalConfiguration.Configuration.Filters.Add(new CustomHttpsAttribute());

            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}