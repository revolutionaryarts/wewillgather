using Gather.Core.Configuration;

namespace Gather.Core.Domain.Common
{
    public class SiteSettings : ISettings
    {
        /// <summary>
        /// The number of comment moderation requests a comment has to receive before it is removed automatically from the site
        /// </summary>
        public string CommentModerationRequestLimit { get; set; }

        /// <summary>
        /// The Facebook App Id used in Facebook requests
        /// </summary>
        public string FacebookAppId { get; set; }
       
        /// <summary>
        /// The Facebook App Secret used in Facebook requests
        /// </summary>
        public string FacebookAppSecret { get; set; }

        /// <summary>
        /// The Facebook profile id to be used in the Facebook Fan Box widget in the site footer
        /// </summary>
        public string FacebookWidgetProfileId { get; set; }

        /// <summary>
        /// The Flickr id to be used in the Flickr widget in the site footer
        /// </summary>
        public string FlickrWidgetId { get; set; }

        /// <summary>
        /// The Flickr URL to be linked to in the Flickr footer widget
        /// </summary>
        public string FlickrWidgetUrl { get; set; }

        /// <summary>
        /// The Google Analytics enabled flag
        /// </summary>
        public string GoogleAnalyticsEnabled { get; set; }

        /// <summary>
        /// The Google Analytics UA code to use for tracking
        /// </summary>
        public string GoogleAnalyticsUACode { get; set; }

        /// <summary>
        /// The number of success stories to display in the homepage slider
        /// </summary>
        public string HomePageSuccessStoryCount { get; set; }

        /// <summary>
        /// The number of projects to display on the project listing page
        /// </summary>
        public string ProjectListingPageSize { get; set; }

        /// <summary>
        /// The number of success stories to display on the success story listing page
        /// </summary>
        public string SuccessStoryListingPageSize { get; set; }

        /// <summary>
        /// The number of success stories to display in the success story RSS feed
        /// </summary>
        public string SuccessStoryRssSize { get; set; }

        /// <summary>
        /// The Twitter Consumer Key used in Twitter requests
        /// </summary>
        public string TwitterConsumerKey { get; set; }

        /// <summary>
        /// The Twitter Consumer Secret used in Twitter requests
        /// </summary>
        public string TwitterConsumerSecret { get; set; }

        /// <summary>
        /// The hashtag to use across the site
        /// </summary>
        public string TwitterHashTag { get; set; }

        /// <summary>
        /// The hashtag to use in the Twitter monitor task
        /// </summary>
        public string TwitterQuery { get; set; }
    }
}