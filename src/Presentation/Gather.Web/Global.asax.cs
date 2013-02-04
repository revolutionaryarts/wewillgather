using System;
using System.Globalization;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FluentValidation.Mvc;
using Gather.Core;
using Gather.Core.Data;
using Gather.Core.Infrastructure;
using Gather.Services.Logging;
using Gather.Services.Tasks;
using Gather.Web.Framework;
using Gather.Web.Framework.Mvc;
using Gather.Web.Framework.Mvc.Routes;

namespace Gather.Web
{
    public class MvcApplication : HttpApplication
    {

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var routePublisher = EngineContext.Current.Resolve<IRoutePublisher>();
            routePublisher.RegisterRoutes(routes);

            routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "Gather.Web.Controllers" }
            );
        }

        protected void Application_Start()
        {
            // Initialize engine
            EngineContext.Initialize(false);

            // Check if the database is installed
            bool siteIsInstalled = DataSettingsHelper.SiteIsInstalled;

            // Resolve dependency
            var dependencyResolver = new GatherDependencyResolver();
            DependencyResolver.SetResolver(dependencyResolver);

            // Register site areas
            AreaRegistration.RegisterAllAreas();

            // Register filters and routes
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            // Register fluentvalidation
            var fluentValidationModelValidatorProvider = new FluentValidationModelValidatorProvider(new ValidatorFactory());
            ModelValidatorProviders.Providers.Add(fluentValidationModelValidatorProvider);
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            fluentValidationModelValidatorProvider.AddImplicitRequiredValidator = false;

            // Register script bundles
            var adminCssBundle = new Bundle("~/areas/admin/content/core.css", new CssMinify());
            adminCssBundle.AddDirectory("~/areas/admin/content/", "*.css", true);
            BundleTable.Bundles.Add(adminCssBundle);

            var adminJsBundle = new Bundle("~/areas/admin/scripts/core.js", new JsMinify());
            adminJsBundle.AddDirectory("~/areas/admin/scripts/jquery/", "*.js", true);
            BundleTable.Bundles.Add(adminJsBundle);

            var coreCssBundle = new Bundle("~/content/css/core.css", new CssMinify());
            coreCssBundle.AddFile("~/Content/Css/normalize.css");
            coreCssBundle.AddFile("~/Content/Css/styles.css");
            coreCssBundle.AddFile("~/Content/Css/jquery-ui-1.8.21.css");
            coreCssBundle.AddFile("~/Content/Css/leaflet.css");
            coreCssBundle.AddFile("~/Content/Css/leaflet.zoomfs.css");
            BundleTable.Bundles.Add(coreCssBundle);

            var jqueryJsBundle = new Bundle("~/content/jquery.js", new NoTransform());
            jqueryJsBundle.AddDirectory("~/Scripts/jQuery/", "*.js");
            BundleTable.Bundles.Add(jqueryJsBundle);

            var coreJsBundle = new Bundle("~/content/core.js", new JsMinify());
            coreJsBundle.AddDirectory("~/Scripts/", "*.js");
            BundleTable.Bundles.Add(coreJsBundle);

            var mapJsBundle = new Bundle("~/content/coremap.js", new JsMinify());
            mapJsBundle.AddFile("~/Scripts/PerPage/core-maps.js");
            BundleTable.Bundles.Add(mapJsBundle);

            var homeJsBundle = new Bundle("~/content/home.js", new JsMinify());
            homeJsBundle.AddFile("~/Scripts/PerPage/home.js");
            BundleTable.Bundles.Add(homeJsBundle);

            var projectDetailJsBundle = new Bundle("~/content/projectdetail.js", new JsMinify());
            projectDetailJsBundle.AddFile("~/Scripts/PerPage/project-detail.js");
            BundleTable.Bundles.Add(projectDetailJsBundle);

            var projectListingJsBundle = new Bundle("~/content/projectlisting.js", new JsMinify());
            projectListingJsBundle.AddFile("~/Scripts/PerPage/project-listing.js");
            BundleTable.Bundles.Add(projectListingJsBundle);

            if (siteIsInstalled)
            {
                // Kick off the scheduled tasks
                TaskManager.Instance.Initialize();
                TaskManager.Instance.Start();
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            string installUrl = string.Format("{0}install", webHelper.GetSiteLocation());
            string authUrl = string.Format("{0}auth/setup", webHelper.GetSiteLocation());
            if (!DataSettingsHelper.SiteIsInstalled && !webHelper.IsStaticResource(Request) && 
                !webHelper.GetThisPageUrl(false).StartsWith(installUrl) && !webHelper.GetThisPageUrl(false).StartsWith(authUrl))
                Response.Redirect(installUrl);
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            var app = (MvcApplication)sender;
            var context = app.Context;
            var ex = app.Server.GetLastError();

            if (System.Configuration.ConfigurationManager.AppSettings["debugMode"].ToString(CultureInfo.InvariantCulture) != "true")
            {
                context.Response.Clear();
                context.ClearError();

                var httpException = ex as HttpException;

                var routeData = new RouteData();
                routeData.Values["controller"] = "Error";
                routeData.Values["action"] = "Error500";

                if (httpException != null)
                {
                    switch (httpException.GetHttpCode())
                    {
                        case 403:
                            routeData.Values["action"] = "Error403";
                            break;
                        case 404:
                            routeData.Values["action"] = "Error404";
                            break;
                        case 500:
                            routeData.Values["action"] = "Error500";
                            break;
                    }
                }

                LogException(ex);

                IController controller = new Controllers.ErrorController();
                controller.Execute(new RequestContext(new HttpContextWrapper(context), routeData));
            }
            else
            {
                LogException(ex);
            }
        }

        protected void LogException(Exception exc)
        {
            if (exc == null)
                return;

            try
            {
                var logger = EngineContext.Current.Resolve<ILogService>();
                var workContext = EngineContext.Current.Resolve<IWorkContext>();
                logger.Error(exc.Message, exc, workContext.CurrentUser);
            }
            catch { }
        }

    }
}