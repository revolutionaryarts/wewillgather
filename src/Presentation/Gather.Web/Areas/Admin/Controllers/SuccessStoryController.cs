using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Security;
using Gather.Core.Domain.Slug;
using Gather.Core.Seo;
using Gather.Services.Security;
using Gather.Services.Slugs;
using Gather.Services.Users;
using Gather.Web.Areas.Admin.Extensions;
using Gather.Web.Areas.Admin.Models.Common;
using Gather.Web.Extensions;
using Gather.Web.Framework.Controllers;
using Gather.Web.Framework.Mvc;
using Gather.Services.SuccessStories;
using Gather.Web.Areas.Admin.Models.SuccessStory;
using Gather.Core.Domain.SuccessStories;
using Gather.Web.Models.SuccessStory;
using Gather.Core;
using Gather.Services.MediaFile;
using Gather.Core.Domain.MediaFile;

namespace Gather.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class SuccessStoryController : ModuleController
    {

        #region Fields

        private readonly CoreSettings _coreSettings;
        private readonly IMediaService _mediaService;
        private readonly IPermissionService _permissionService;
        private readonly ISuccessStoryService _successStoryService;
        private readonly IUserService _userService;
        private readonly ISlugService _slugService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Constructors

        public SuccessStoryController() { }

        public SuccessStoryController(CoreSettings coreSettings, IPermissionService permissionService, ISuccessStoryService successStoryService, IUserService userService, ISlugService slugService, IMediaService mediaService, IWebHelper webHelper)
        {
            _coreSettings = coreSettings;
            _mediaService = mediaService;
            _permissionService = permissionService;
            _successStoryService = successStoryService;
            _slugService = slugService;
            _userService = userService;
            _webHelper = webHelper;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageSuccessStories))
                return AccessDeniedView();

            var successStory = _successStoryService.GetAllSuccessStories(Page, _coreSettings.AdminGridPageSize, null, Search);

            var model = new SuccessStoryListModel
            {
                PageIndex = Page,
                PageSize = _coreSettings.AdminGridPageSize,
                TotalCount = successStory.TotalCount,
                TotalPages = successStory.TotalPages,
                Search = Search,
                SuccessStories = successStory.Select(PrepareListSuccessStoryModel).OrderByDescending(x => x.CreatedDate).ToList()
            };

            PrepareBreadcrumbs();
            
            return View(model);
        }

        public ActionResult Add()
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageSuccessStories))
                return AccessDeniedView();

            PrepareBreadcrumbs();
            AddBreadcrumb("Add New Story", null);

            return View(PrepareSuccessStoryModel(null));
        }

        [HttpPost, ValidateInput(false)]
        [FormValueRequired("add")]
        public ActionResult Add(SuccessStoryModel model, string add)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageSuccessStories))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                try
                {
                    // Build the story entity
                    SuccessStory success = model.ToEntity();
                    success.Active = (add.ToLower() == "publish");
                    success.Author = _userService.GetUserById(model.AuthorId);

                    // Insert the story entity
                    _successStoryService.InsertSuccessStory(success);
                    _slugService.InsertSlug(new Slug{ SlugUrl = SeoExtensions.GetSeoName(success.Title), SuccessStoryId = success.Id});

                    // Build the uploaded file name and store the file
                    string path = HttpContext.Server.MapPath("/Uploads/Media/");
                    string filename = _webHelper.GetUniqueFileName(path, model.UploadedFile.FileName);
                    model.UploadedFile.SaveAs(path + filename);
                    
                    // Build the media entity
                    var media = new Media
                    {
                        EntityId = success.Id,
                        EntityType = EntityType.SuccessStory,
                        FileName = filename
                    };

                    // Insert the media entity
                    _mediaService.InsertMedia(media);

                    SuccessNotification("The success story details have been added successfully.");
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ErrorNotification(ex.ToString());
                }
            }

            PrepareBreadcrumbs();
            AddBreadcrumb("Add New Story", null);

            return View(PrepareSuccessStoryModel(null));
        }

        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageSuccessStories))
                return AccessDeniedView();

            var success = _successStoryService.GetSuccessStoryById(id);
            if (success == null)
                return RedirectToAction("Index", new { page = Page, search = Search });

            try
            {
                _successStoryService.DeleteSuccessStory(success);
                _slugService.DeleteSlugsBySuccessStoryId(success.Id);
                SuccessNotification(success.Title + " has been successfully deleted.");
            }
            catch (Exception)
            {
                ErrorNotification("An error occurred deleting " + success.Title + ", please try again");
            }

            return RedirectToAction("Index", new { page = Page, search = Search });
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageSuccessStories))
                return AccessDeniedView();

            // get the story
            SuccessStory story = _successStoryService.GetSuccessStoryById(id);
            
            // check we have a story and they are not deleted
            if (story == null || story.Deleted)
            {
                ErrorNotification("The story couldn't be found or has been deleted.");
                return RedirectToAction("Index");
            }

            if (!story.Active)
                WarningNotification("This story is currently unpublished. To publish it, use the 'publish' button on the right.");

            PrepareBreadcrumbs();
            AddBreadcrumb("Edit Story", null);

            return View(PrepareSuccessStoryModel(story));
        }

        [HttpPost, ValidateInput(false)]
        [FormValueRequired("save")]
        public ActionResult Edit(SuccessStoryModel model, string save)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageSuccessStories))
                return AccessDeniedView();

            save = save.ToLower();

            // get the category
            var story = _successStoryService.GetSuccessStoryById(model.Id);

            // check we have a category and they are not deleted
            if (story == null || story.Deleted)
            {
                ErrorNotification("The story couldn't be found or has been deleted.");
                return RedirectToAction("Index");
            }

            story.Active = (save == "update" ? story.Active : (save == "publish"));
            story.Author = _userService.GetUserById(model.AuthorId);
            story.Title = model.Title;
            story.ShortSummary = model.ShortSummary;
            story.Article = model.Article;
            story.MetaTitle = model.MetaTitle;
            story.MetaDescription = model.MetaDescription;
            story.MetaKeywords = model.MetaKeywords;
            story.MetaTitle = model.MetaTitle;
            if (ModelState.IsValid)
            {
                try
                {
                    _successStoryService.UpdateSuccessStory(story);
                    _slugService.UpdateSlug(story.Id, SeoExtensions.GetSeoName(story.Title));
                   
                    if (model.UploadedFile != null && model.UploadedFile.ContentLength > 0)
                    {
                        Media currentMedia =
                       _mediaService.GetAllMediaByEntityId(EntityType.SuccessStory, model.Id).FirstOrDefault();
                        if (currentMedia != null)
                            _mediaService.DeleteMedia(currentMedia);
                        string path = HttpContext.Server.MapPath("/Uploads/Media/");
                        string filename = _webHelper.GetUniqueFileName(path, model.UploadedFile.FileName);
                        Media media = new Media
                                          {
                                              EntityId = model.Id,
                                              EntityTypeId = (int) EntityType.SuccessStory,
                                              EntityType = EntityType.SuccessStory,
                                              FileName = filename
                                          };
                        model.UploadedFile.SaveAs(path + filename);
                        _mediaService.InsertMedia(media);
                    }
                    SuccessNotification("The success story details have been updated successfully.");
                    return RedirectToAction("Edit", story.Id);
                }
                catch (Exception)
                {                    
                    ErrorNotification("An error occurred saving the success story details, please try again.");
                }
            }
            else
            {
                ErrorNotification("We were unable to make the change, please review the form and correct the errors.");
            }

            PrepareBreadcrumbs();
            AddBreadcrumb("Edit Story", null);

            return View(PrepareSuccessStoryModel(story));
        }

        private void PrepareBreadcrumbs()
        {
            AddBreadcrumb("Blog", Url.Action("index"));
        }

        #endregion

        #region Prepare Models

        [NonAction]
        private SuccessStoryModel PrepareListSuccessStoryModel(SuccessStory successStory)
        {
            var model = successStory.ToModel();

            model.Actions.Add(new ModelActionLink
            {
                Alt = "Edit",
                Icon = Url.Content("~/Areas/Admin/Content/images/icon-edit.png"),
                Target = Url.Action("edit", new { id = successStory.Id })
            });

            model.Actions.Add(new DeleteActionLink(successStory.Id, Search, Page));

            return model;
        }

        [NonAction]
        private SuccessStoryModel PrepareSuccessStoryModel(SuccessStory successStory)
        {
            var model = new SuccessStoryModel();
            if (successStory != null)
                model = successStory.ToModel();

            model.Authors = _userService.GetAllUsers().Select(u => u.ToModel()).ToList();
            return model;
        }

        #endregion

        #region Navigation

        public override IList<NavigationSectionModel> RegisterNavigation()
        {
            var sections = new List<NavigationSectionModel>();
            var httpContextWrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
            var urlHelper = new UrlHelper(new RequestContext(httpContextWrapper, new RouteData()));

            var section = new NavigationSectionModel
            {
                Icon = urlHelper.Content("~/Areas/Admin/Content/images/menu-pages.png"),
                Name = "successstory",
                Position = 5,
                RequiredPermissions = new List<PermissionRecord>
                {
                    PermissionProvider.ManageSuccessStories
                },
                Target = urlHelper.Action("index", "successstory", new { area = "admin" }),
                Title = "Blog"
            };

            section.AddChildLink("Manage Posts", urlHelper.Action("index", "successstory", new { area = "admin" }));
            sections.Add(section);

            return sections;
        }

        #endregion

    }
}