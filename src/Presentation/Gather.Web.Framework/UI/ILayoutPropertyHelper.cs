using System.Collections.Generic;

namespace Gather.Web.Framework.UI
{
    public interface ILayoutPropertyHelper
    {
        string CurrentSectionName { get; set; }
        string Heading { get; set; }
        string LinkTarget { get; set; }
        string LinkText { get; set; }

        void AddTitleParts(params string[] parts);
        void AppendTitleParts(params string[] parts);
        string GenerateTitle();

        void AddMetaDescriptionParts(params string[] parts);
        void AppendMetaDescriptionParts(params string[] parts);
        string GenerateMetaDescription();

        void AddMetaKeywordParts(params string[] parts);
        void AppendMetaKeywordParts(params string[] parts);
        string GenerateMetaKeywords();

        void AddScriptParts(params string[] parts);
        void AppendScriptParts(params string[] parts);
        string GenerateScripts();

        void AddMetaTag(string name, string content);
        string GenerateMetaTags();

        void AddLinkTag(Dictionary<string, string> attributes);
        string GenerateLinkTags();

        void AddCanonicalUrlParts(params string[] parts);
        void AppendCanonicalUrlParts(params string[] parts);
        string GenerateCanonicalUrls();
    }
}