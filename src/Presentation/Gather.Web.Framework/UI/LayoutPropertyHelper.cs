using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gather.Web.Framework.UI
{
    public class LayoutPropertyHelper : ILayoutPropertyHelper
    {

        private readonly List<string> _canonicalUrlParts;
        private readonly List<Dictionary<string, string>> _linkTags; 
        private readonly List<string> _metaDescriptionParts;
        private readonly List<string> _metaKeywordParts;
        private readonly Dictionary<string, string> _metaTags; 
        private readonly List<string> _scriptParts;
        private readonly List<string> _titleParts;

        private const string PAGE_TITLE_SEPARATOR = " | ";
        private const string DEFAULT_TITLE = "Welcome | #WeWillGather";
        private const string DEFAULT_META_DESCRIPTION = "";
        private const string DEFAULT_META_KEYWORDS = "";

        public string CurrentSectionName { get; set; }
        public string Heading { get; set; }
        public string LinkTarget { get; set; }
        public string LinkText { get; set; }

        public LayoutPropertyHelper()
        {
            _canonicalUrlParts = new List<string>();
            _linkTags = new List<Dictionary<string, string>>();
            _metaDescriptionParts = new List<string>();
            _metaKeywordParts = new List<string>();
            _metaTags = new Dictionary<string, string>();
            _scriptParts = new List<string>();
            _titleParts = new List<string>();
        }

        #region Methods

        #region Title

        public void AddTitleParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _titleParts.Add(part);
        }

        public void AppendTitleParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _titleParts.Insert(0, part);
        }

        public string GenerateTitle()
        {
            string result;
            var specificTitle = string.Join(PAGE_TITLE_SEPARATOR, _titleParts.AsEnumerable().Reverse().ToArray());
            if (!string.IsNullOrEmpty(specificTitle))
                result = specificTitle;
            else
                result = DEFAULT_TITLE;
            return result;
        }

        #endregion

        #region Meta Description

        public void AddMetaDescriptionParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _metaDescriptionParts.Add(part);
        }

        public void AppendMetaDescriptionParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _metaDescriptionParts.Insert(0, part);
        }

        public string GenerateMetaDescription()
        {
            var metaDescription = string.Join(", ", _metaDescriptionParts.AsEnumerable().Reverse().ToArray());
            var result = !string.IsNullOrEmpty(metaDescription) ? metaDescription : DEFAULT_META_DESCRIPTION;
            return result;
        }

        #endregion

        #region Meta Keywords

        public void AddMetaKeywordParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _metaKeywordParts.Add(part);
        }

        public void AppendMetaKeywordParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _metaKeywordParts.Insert(0, part);
        }

        public string GenerateMetaKeywords()
        {
            var metaKeyword = string.Join(", ", _metaKeywordParts.AsEnumerable().Reverse().ToArray());
            var result = !string.IsNullOrEmpty(metaKeyword) ? metaKeyword : DEFAULT_META_KEYWORDS;
            return result;
        }

        #endregion

        #region Meta Tags

        public void AddMetaTag(string name, string content)
        {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(content))
                _metaTags.Add(name, content);
        }

        public string GenerateMetaTags()
        {
            var result = new StringBuilder();
            foreach (var tag in _metaTags)
            {
                result.AppendFormat("<meta name=\"{0}\" content=\"{1}\" />", tag.Key, tag.Value);
                result.Append(Environment.NewLine);
            }
            return result.ToString();
        }

        #endregion

        #region Link Tags

        public void AddLinkTag(Dictionary<string, string> attributes)
        {
            if (attributes != null && attributes.Count > 0)
                _linkTags.Add(attributes);
        }

        public string GenerateLinkTags()
        {
            var result = new StringBuilder();
            foreach (var tag in _linkTags)
            {
                result.AppendFormat("<link {0} />", tag.Aggregate("", (current, attribute) => string.Format("{0} {1}=\"{2}\"", current, attribute.Key, attribute.Value)).Trim());
                result.Append(Environment.NewLine);
            }
            return result.ToString();
        }

        #endregion

        #region Scripts

        public void AddScriptParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _scriptParts.Add(part);
        }

        public void AppendScriptParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _scriptParts.Insert(0, part);
        }

        public string GenerateScripts()
        {
            var result = new StringBuilder();
            foreach (var scriptPath in _scriptParts)
            {
                result.AppendFormat("<script src=\"{0}\" type=\"text/javascript\"></script>", scriptPath);
                result.Append(Environment.NewLine);
            }
            return result.ToString();
        }

        #endregion

        #region Canonical URLs

        public void AddCanonicalUrlParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _canonicalUrlParts.Add(part);
        }

        public void AppendCanonicalUrlParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts.Where(part => !string.IsNullOrEmpty(part)))
                    _canonicalUrlParts.Insert(0, part);
        }

        public string GenerateCanonicalUrls()
        {
            var result = new StringBuilder();
            foreach (var canonicalUrl in _canonicalUrlParts)
            {
                result.AppendFormat("<link rel=\"canonical\" href=\"{0}\" />", canonicalUrl);
                result.Append(Environment.NewLine);
            }
            return result.ToString();
        }

        #endregion

        #endregion

    }
}