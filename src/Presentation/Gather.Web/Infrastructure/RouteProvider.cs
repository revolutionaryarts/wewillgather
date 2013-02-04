using System.Web.Mvc;
using System.Web.Routing;
using Gather.Services.Slugs;
using Gather.Web.Framework.Mvc.Routes;
using Gather.Web.Routing;

namespace Gather.Web.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {

        public void RegisterRoutes(RouteCollection routes)
        {
            // Api test
            routes.MapRoute("ApiTest",
                "api/index",
                new { controller = "Api", action = "Index" },
                new[] { "Gather.Web.Controllers" }
            );

            // Homepage

            routes.MapRoute("HomePage",
                "",
                new { controller = "Home", action = "Index" },
                new[] { "Gather.Web.Controllers" }
            );

            // Error pages 

            routes.MapRoute("Error403",
                "error-403",
                new { controller = "Error", action = "Error403" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("Error404",
                "error-404",
                new { controller = "Error", action = "Error404" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("Error500",
                "error-500",
                new { controller = "Error", action = "Error500" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("KeepAlive",
                "keep-alive",
                new { controller = "KeepAlive", action = "KeepAlive" },
                new[] { "Gather.Web.Controllers" }
            );

            // Static pages 

            routes.MapRoute("About",
                "about-us",
                new { controller = "Static", action = "Detail", Id = 1 },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("BeSafe",
                "be-safe",
                new { controller = "Static", action = "Detail", Id = 2 },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("CookiePolicy",
                "cookie-policy",
                new { controller = "Static", action = "Detail", Id = 3 },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("Developers",
                "developers",
                new { controller = "Static", action = "Detail", Id = 4 },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("HowItWorks",
                "how-it-works",
                new { controller = "Static", action = "Detail", Id = 5 },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("OurSupporters",
                "our-supporters",
                new { controller = "Static", action = "Detail", Id = 6 }
            );

            routes.MapRoute("Privacy",
                "privacy",
                new { controller = "Static", action = "Detail", Id = 7 },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("TermsAndConditions",
                "terms-and-conditions",
                new { controller = "Static", action = "Detail", Id = 8 },
                new[] { "Gather.Web.Controllers" }
            );

            // SiteMap

            routes.MapRoute("SiteMap",
                "site-map",
                new { controller = "SiteMap", action = "SiteMap" },
                new[] { "Gather.Web.Controllers" }
            );
            
            routes.MapRoute("SiteMapLocation",
                "site-map/{locationSeoName}",
                new { controller = "SiteMap", action = "SiteMapLocation" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("SiteMapParentLocation",
                "site-map/{parentSeoName}/{locationSeoName}",
                new { controller = "SiteMap", action = "SiteMapLocation" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.Add(
                new Route(
                    url: "site-map-{locationSeoName}",
                    defaults: new RouteValueDictionary(new { controller = "SiteMap", action = "SiteMapLocation" }), 
                    routeHandler: new LegacySitemapHandler(),
                    constraints: null,
                    dataTokens: new RouteValueDictionary(new { Namespaces = "Gather.Web.Controllers" })
                )
            );

            // Contact form and thank you page

            routes.MapRoute("Contact",
                "contact-us",
                new { controller = "Contact", action = "Contact" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("ContactThanks",
                "contact-us-thanks",
                new { controller = "Contact", action = "ContactThanks" },
                new[] { "Gather.Web.Controllers" }
            );

            // Success story listing

            routes.MapRoute("SuccessStoryListing",
                "blog",
                new { controller = "SuccessStory", action = "Listing" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("SuccessStoryRss",
                "blog/rss",
                new { controller = "SuccessStory", action = "Rss" },
                new[] { "Gather.Web.Controllers" }
            );

            // Messages

            routes.MapRoute("Messages",
                "m/{id}",
                new { controller = "User", action = "Message" },
                new[] { "Gather.Web.Controllers" }
            );

            // Project pages

            routes.MapRoute("AddProject",
                "add-project",
                new { controller = "Project", action = "Add", id = 0 },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("AddTemporaryProject",
                "add-project-{id}",
                new { controller = "Project", action = "Add" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("ConfirmProject",
                "project-confirmation",
                new { controller = "Project", action = "Confirmation" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("ProjectListingLocationWithParent",
                "{parentSeoName}/{locationSeoName}-volunteer-projects",
                new { controller = "Project", action = "Listing" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("ProjectListingLocationWithParentRss",
                "{parentSeoName}/{locationSeoName}-volunteer-projects/rss",
                new { controller = "Project", action = "RssListing" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("ProjectListingLocation",
                "{locationSeoName}-volunteer-projects",
                new { controller = "Project", action = "Listing" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("ProjectListingLocationRss",
                "{locationSeoName}-volunteer-projects/rss",
                new { controller = "Project", action = "RssListing" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("ProjectListing",
                "volunteer-projects",
                new { controller = "Project", action = "Listing" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("ProjectListingRss",
                "volunteer-projects/rss",
                new { controller = "Project", action = "RssListing" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("ProjectEdit",
                "{locationSeoName}-volunteer-projects/{seoName}-{id}/edit",
                new { controller = "Project", action = "Edit" },
                new { id = @"\d+" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("ProjectDetail",
                "{locationSeoName}-volunteer-projects/{seoName}-{id}",
                new { controller = "Project", action = "Detail" },
                new { id = @"\d+" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("ProjectDetailTiny",
                "p/{id}",
                new { controller = "Project", action = "TinyUrl" },
                new { id = @"\d+" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("ProjectDetailOrganise",
                "{locationSeoName}-volunteer-projects/{seoName}-{id}/organise",
                new { controller = "Project", action = "TakeOwnership" },
                new { id = @"\d+" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("ProjectDetailVolunteer",
                "{locationSeoName}-volunteer-projects/{seoName}-{id}/volunteer",
                new { controller = "Project", action = "Volunteer" },
                new { id = @"\d+" },
                new[] { "Gather.Web.Controllers" }
            );

            // User profile pages

            routes.MapRoute("UserProfileApiEdit",
                "user/api/{id}",
                new { controller = "User", action = "ProfileApiEdit" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("UserProfileApi",
                "user/api",
                new { controller = "User", action = "ProfileApi" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("UserProfileChangePrimary",
                "user/switch-primary",
                new { controller = "User", action = "SwitchPrimaryAccount" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("UserProfileDelete",
                "user/delete",
                new { controller = "User", action = "Delete" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("UserProfileUnlink",
                "user/unlink/{profileType}",
                new { controller = "User", action = "UnlinkProfile" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("UserProfile",
                "user/{userName}",
                new { controller = "User", action = "Profile" },
                new[] { "Gather.Web.Controllers" }
            );

            // Authentication pages

            routes.MapRoute("AuthenticationError",
                "auth/login/error",
                new { controller = "Login", action = "Error" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("AuthenticationSetupLogin",
                "auth/setup/{action}",
                new { controller = "Authentication", result = "setup" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("AuthenticationSocialLogin",
                "auth/login/{action}",
                new { controller = "Authentication", result = "loginorregister" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("AuthenticationLink",
                "auth/link/{action}",
                new { controller = "Authentication", result = "linkaccount" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("AuthenticationLogin",
                "auth/login",
                new { controller = "Authentication", action = "Login" },
                new[] { "Gather.Web.Controllers" }
            );

            routes.MapRoute("AuthenticationLogout",
                "auth/logout",
                new { controller = "Authentication", action = "LogOff" },
                new[] { "Gather.Web.Controllers" }
            );

            // Install

            routes.MapRoute("Install",
                "install",
                new { controller = "Install", action = "StepOne" },
                new[] { "Gather.Web.Controllers" }
            );

            // Success story detail page

            routes.MapRoute("SuccessStory",
                "{SeName}",
                new { controller = "SuccessStory", action = "Detail" },
                new[] { "Gather.Web.Controllers" }
            ).RouteHandler = new FriendlyRouteHandler();

        }

        public int Priority
        {
            get
            {
                return 0;
            }
        }

    }
}