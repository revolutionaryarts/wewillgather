using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Gather.Core.Domain.Common;
using Gather.Core.Infrastructure;
using Gather.Core.Seo;
using Gather.Services.Locations;
using Gather.Services.Pages;
using Gather.Services.Projects;
using Gather.Services.SuccessStories;
using Gather.Services.Tasks;

namespace Gather.Services.Sitemap.Tools
{
    public class XmlSitemapTask : ITask
    {
        public void Execute()
        {
            // Initialize engine
            EngineContext.Initialize(false);

            // Resolve any services
            var coreSettings = EngineContext.Current.Resolve<CoreSettings>();
            var httpContext = EngineContext.Current.Resolve<HttpContextBase>();
            var locationService = EngineContext.Current.Resolve<ILocationService>();
            var pageService = EngineContext.Current.Resolve<IPageService>();
            var projectService = EngineContext.Current.Resolve<IProjectService>();
            var successStoryService = EngineContext.Current.Resolve<ISuccessStoryService>();

            var sb = new StringBuilder();
            var nodes = new List<XmlNode>();
            var urlHelper = new UrlHelper(httpContext.Request.RequestContext);

            nodes.Add(new XmlNode(urlHelper.RouteUrl("HomePage"), 1));
            nodes.Add(new XmlNode(urlHelper.RouteUrl("Contact"), 0.1m));
            nodes.Add(new XmlNode(urlHelper.RouteUrl("SiteMap"), 0.1m));
            nodes.Add(new XmlNode(urlHelper.RouteUrl("AuthenticationLogin"), 0.0m));
            nodes.Add(new XmlNode(urlHelper.RouteUrl("SuccessStoryListing"), 0.7m));
            nodes.Add(new XmlNode(urlHelper.RouteUrl("ProjectListing"), 0.8m));
            nodes.Add(new XmlNode(urlHelper.RouteUrl("AddProject"), 0.0m));

            // Success stories
            nodes.AddRange(successStoryService.GetAllSuccessStories().Select(x => new XmlNode(urlHelper.RouteUrl("SuccessStory", new { seoName = SeoExtensions.GetSeoName(x.Title) }), 0.6m)));

            // Pages
            nodes.AddRange(pageService.GetAllPages(1, -1).Select(x => new XmlNode(urlHelper.Action("Detail", "Static", new { x.Id }), Math.Round(x.Priority, 1))));

            // Locations
            nodes.AddRange(locationService.GetAllCachedLocations().Select(x => new XmlNode(urlHelper.RouteUrl("ProjectListingLocation", new { locationSeoName = x.SeoName }), 0.9m)));

            // Projects
            foreach (var project in projectService.GetAllCachedProjects())
            {
                var primaryLocation = project.Locations.First(l => l.Primary).Location;
                nodes.Add(new XmlNode(urlHelper.RouteUrl("ProjectDetail", new { locationSeoName = primaryLocation.SeoName, seoName = project.GetSeoName(), id = project.Id }), 0.8m));
            }

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            foreach (var node in nodes.OrderByDescending(x => x.Priority))
            {
                sb.AppendLine("<url>");
                sb.AppendLine("<loc>" + coreSettings.Domain.TrimEnd('/') + node.Url + "</loc>");
                sb.AppendLine("<lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>");
                sb.AppendLine("<changefreq>monthly</changefreq>");
                sb.AppendLine("<priority>" + node.Priority + "</priority>");
                sb.AppendLine("</url>");
            }

            sb.AppendLine("</urlset>");

            string filePath = HostingEnvironment.MapPath("~/site-map.xml");
            File.WriteAllText(filePath, sb.ToString());
        }
    }

    public class XmlNode
    {
        public XmlNode(string url, decimal priority)
        {
            Url = url;
            Priority = priority;
        }

        public decimal Priority { get; set; }

        public string Url { get; set; }
    }
}