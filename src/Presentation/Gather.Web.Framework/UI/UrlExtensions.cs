using System;
using System.Web.Mvc;
using Gather.Core;
using Gather.Core.Infrastructure;

namespace Gather.Web.Framework.UI
{
    public static class UrlExtensions
    {

        public static string ContentAbsolute(this UrlHelper url, string path)
        {
            string contentPath = url.Content(path);
            var uri = new Uri(contentPath, UriKind.RelativeOrAbsolute);
            if (uri.IsAbsoluteUri)
                return contentPath;
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            return webHelper.GetSiteLocation().TrimEnd('/') + contentPath;
        }

    }
}