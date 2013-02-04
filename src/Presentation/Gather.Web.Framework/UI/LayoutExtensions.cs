using System.Collections.Generic;
using System.Web.Mvc;
using Gather.Core.Infrastructure;

namespace Gather.Web.Framework.UI
{
    public static class LayoutExtensions
    {

        public static string CurrentSectionName(this HtmlHelper html)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            return layoutPropertyHelper.CurrentSectionName;
        }

        public static void CurrentSectionName(this HtmlHelper html, string name)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            layoutPropertyHelper.CurrentSectionName = name;
        }

        public static string Heading(this HtmlHelper html)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            return layoutPropertyHelper.Heading;
        }

        public static void Heading(this HtmlHelper html, string heading)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            layoutPropertyHelper.Heading = heading;
        }

        public static bool LinkActive(this HtmlHelper html)
        {
            if (!string.IsNullOrEmpty(html.LinkText()) && !string.IsNullOrEmpty(html.LinkTarget()))
                return true;
            return false;
        }

        public static string LinkTarget(this HtmlHelper html)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            return layoutPropertyHelper.LinkTarget;
        }

        public static void LinkTarget(this HtmlHelper html, string linkTarget)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            layoutPropertyHelper.LinkTarget = linkTarget;
        }

        public static string LinkText(this HtmlHelper html)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            return layoutPropertyHelper.LinkText;
        }

        public static void LinkText(this HtmlHelper html, string linkText)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            layoutPropertyHelper.LinkText = linkText;
        }

        #region Title

        public static void AddTitleParts(this HtmlHelper html, params string[] parts)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            layoutPropertyHelper.AddTitleParts(parts);
        }

        public static void AppendTitleParts(this HtmlHelper html, params string[] parts)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            layoutPropertyHelper.AppendTitleParts(parts);
        }

        public static MvcHtmlString Title(this HtmlHelper html, params string[] parts)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            html.AppendTitleParts(parts);
            return MvcHtmlString.Create(html.Encode(layoutPropertyHelper.GenerateTitle()));
        }

        #endregion

        #region Meta Description

        public static void AddMetaDescriptionParts(this HtmlHelper html, params string[] parts)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            layoutPropertyHelper.AddMetaDescriptionParts(parts);
        }

        public static void AppendMetaDescriptionParts(this HtmlHelper html, params string[] parts)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            layoutPropertyHelper.AppendMetaDescriptionParts(parts);
        }

        public static MvcHtmlString MetaDescription(this HtmlHelper html, params string[] parts)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            html.AppendMetaDescriptionParts(parts);
            return MvcHtmlString.Create(html.Encode(layoutPropertyHelper.GenerateMetaDescription()));
        }

        #endregion

        #region Meta Keywords

        public static void AddMetaKeywordParts(this HtmlHelper html, params string[] parts)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            layoutPropertyHelper.AddMetaKeywordParts(parts);
        }

        public static void AppendMetaKeywordParts(this HtmlHelper html, params string[] parts)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            layoutPropertyHelper.AppendMetaKeywordParts(parts);
        }

        public static MvcHtmlString MetaKeywords(this HtmlHelper html, params string[] parts)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            html.AppendMetaKeywordParts(parts);
            return MvcHtmlString.Create(html.Encode(layoutPropertyHelper.GenerateMetaKeywords()));
        }

        #endregion

        #region Meta Tags 

        public static void AddMetaTag(this HtmlHelper html, string name, string content)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            layoutPropertyHelper.AddMetaTag(name, content);
        }

        public static MvcHtmlString MetaTags(this HtmlHelper html)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            return MvcHtmlString.Create(layoutPropertyHelper.GenerateMetaTags());
        }

        #endregion

        #region Link Tags

        public static void AddAlternateTag(this HtmlHelper html, string title, string href, string type)
        {
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(href) && !string.IsNullOrEmpty(type))
            {
                var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
                layoutPropertyHelper.AddLinkTag(new Dictionary<string, string>
                {
                    { "rel", "alternate" },
                    { "type", type },
                    { "href", href },
                    { "title", title }
                });
            }
        }

        public static void AddLinkTag(this HtmlHelper html, Dictionary<string, string> attributes)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            layoutPropertyHelper.AddLinkTag(attributes);
        }

        public static MvcHtmlString LinkTags(this HtmlHelper html)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            return MvcHtmlString.Create(layoutPropertyHelper.GenerateLinkTags());
        }

        #endregion

        #region Scripts

        public static void AddScriptParts(this HtmlHelper html, params string[] parts)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            layoutPropertyHelper.AddScriptParts(parts);
        }

        public static void AppendScriptParts(this HtmlHelper html, params string[] parts)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            layoutPropertyHelper.AppendScriptParts(parts);
        }

        public static MvcHtmlString GatherScripts(this HtmlHelper html, params string[] parts)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            html.AppendScriptParts(parts);
            return MvcHtmlString.Create(layoutPropertyHelper.GenerateScripts());
        }

        #endregion

        #region Canonical URLs

        public static void AddCanonicalUrlParts(this HtmlHelper html, params string[] parts)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            layoutPropertyHelper.AddCanonicalUrlParts(parts);
        }

        public static void AppendCanonicalUrlParts(this HtmlHelper html, params string[] parts)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            layoutPropertyHelper.AppendCanonicalUrlParts(parts);
        }

        public static MvcHtmlString CanonicalUrls(this HtmlHelper html, params string[] parts)
        {
            var layoutPropertyHelper = EngineContext.Current.Resolve<ILayoutPropertyHelper>();
            html.AppendCanonicalUrlParts(parts);
            return MvcHtmlString.Create(layoutPropertyHelper.GenerateCanonicalUrls());
        }

        #endregion

        #region Google Analytics

        public static void AddTrackingRecord(this HtmlHelper html, params string[] parts)
        {
            var gaBuilder = EngineContext.Current.Resolve<IAnalyticsBuilder>();
            gaBuilder.AddTrackingRecord(parts);
        }

        public static MvcHtmlString GoogleAnalytics(this HtmlHelper html)
        {
            var gaBuilder = EngineContext.Current.Resolve<IAnalyticsBuilder>();
            return MvcHtmlString.Create(gaBuilder.GenerateAnalytics());
        }

        #endregion

    }
}