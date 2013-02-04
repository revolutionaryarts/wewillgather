using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ServiceModel.Syndication;
using Gather.Core.ActionResults;
using Gather.Core.Domain.Common;
using Gather.Services.SuccessStories;
using Gather.Web.Extensions;
using Gather.Web.Models.SuccessStory;

namespace Gather.Web.Controllers
{
    public class SuccessStoryController : BaseController
    {

        #region Fields

        private readonly SiteSettings _siteSettings;
        private readonly ISuccessStoryService _successStoryService;

        #endregion

        #region Constructor

        public SuccessStoryController(SiteSettings siteSettings, ISuccessStoryService successStoryService)
        {
            _siteSettings = siteSettings;
            _successStoryService = successStoryService;
        }

        #endregion

        #region Methods

        public ActionResult Detail(int successStoryId)
        {
            var story = _successStoryService.GetSuccessStoryById(successStoryId);

            var model = story.ToModel();

            if (model == null)
                throw new HttpException(404, "not found");

            if (model.Deleted || !model.Active)
                throw new HttpException(404, "not found");

            AddHomeBreadcrumb();
            AddBreadcrumb("Blog", Url.RouteUrl("SuccessStoryListing"));
            AddBreadcrumb(model.Title, Url.RouteUrl("SuccessStory", new { model.SeName }));

            return View(model);
        }

        public ActionResult Listing()
        {
            AddHomeBreadcrumb();
            AddBreadcrumb("Blog", Url.RouteUrl("SuccessStoryListing"));
            
            int itemsPageSize;
            int.TryParse(_siteSettings.SuccessStoryListingPageSize, out itemsPageSize);
            if (itemsPageSize < 1)
                itemsPageSize = 6;

            var stories = _successStoryService.GetAllSuccessStories(Page, itemsPageSize);

            var model = new SuccessStoryListingModel
            {
                Hashtag = _siteSettings.TwitterHashTag,
                PageIndex = Page,
                PageSize = itemsPageSize, 
                TotalCount = stories.TotalCount, 
                TotalPages = stories.TotalPages, 
                SuccessStories = stories.Select(x => x.ToModel()).ToList()
            };

            if (model.TotalPages == 0)
                return RedirectToRoute("HomePage");

            if (Page > model.TotalPages)
                return RedirectToRoute("SuccessStoryListing");

            return View(model);
        }

        public ActionResult Rss()
        {
            var urlHelper = new UrlHelper(HttpContext.Request.RequestContext);
            int size;
            if (!int.TryParse(_siteSettings.SuccessStoryRssSize, out size))
                size = 25;

            var storyItems = _successStoryService.GetAllSuccessStories(Page, size).Select(x => x.ToModel()).ToList()
                .Select(story => new SyndicationItem(story.Title, story.Article, new Uri(Url.RouteUrl("SuccessStory", new {story.SeName}, "http")))
            {
                LastUpdatedTime = story.LastModifiedDate, 
                PublishDate = story.CreatedDate
            }).ToList();

            var feed = new SyndicationFeed("#wewillgather blog", "#wewillgather blog RSS feed - getting people together to do good things in their community", new Uri(urlHelper.RouteUrl("SuccessStoryListing", null, "http")), storyItems);
            return new FeedResult(new Rss20FeedFormatter(feed));
        }

        #endregion

    }
}