using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Gather.Core.Data;
using Gather.Core.Domain.Common;
using Gather.Core.Infrastructure;
using Gather.Core.Domain.Slug;

namespace Gather.Services.Slugs
{
    public class FriendlyRouteHandler : MvcRouteHandler
    {

        #region Handler Methods

        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var seName = (string)requestContext.RouteData.Values["SeName"];
            var slug = UrlLookup(seName, requestContext);

            // Found a success story
            if (slug.SuccessStoryId > 0)
                requestContext = SuccessStory(requestContext, slug.SuccessStoryId);
            
            return base.GetHttpHandler(requestContext);
        }

        private Slug UrlLookup(string seName, RequestContext requestContext)
        {
            if (DataSettingsHelper.SiteIsInstalled)
            {
                var slugService = EngineContext.Current.Resolve<ISlugService>();
                var urlSlug = slugService.SlugLookup(seName);
                var urlHelper = new UrlHelper(requestContext);

                if (urlSlug != null)
                {
                    // We may need to 301
                    if (urlSlug.LookupType == SlugLookupType.Slug301Found)
                    {
                        if (urlSlug.SuccessStoryId > 0)
                            HttpContext.Current.Response.RedirectPermanent(urlHelper.RouteUrl("SuccessStory", new {SeName = urlSlug.SlugUrl}));
                    }
                }
                else
                {
                    try
                    {
                        HttpContext.Current.Server.TransferRequest(urlHelper.RouteUrl("Error404"), false);
                        HttpContext.Current.Response.End();
                    }
                    catch (PlatformNotSupportedException)
                    {
                        // Hack for when running in integrated mode.
                        HttpContext.Current.Response.Redirect(urlHelper.RouteUrl("Error404"), true);
                        HttpContext.Current.Response.End();
                    }
                }

                return urlSlug;
            }

            return null;
        }

        #endregion

        #region Routes

        private static RequestContext SuccessStory(RequestContext requestContext, int successStoryId)
        {
            requestContext.RouteData.Values["controller"] = "SuccessStory";
            requestContext.RouteData.Values["action"] = "Detail";
            requestContext.RouteData.Values["successStoryId"] = successStoryId;
            return requestContext;
        }

        #endregion

    }
}