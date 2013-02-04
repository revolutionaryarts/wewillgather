using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.MediaFile;
using Gather.Core.Domain.Security;
using Gather.Services.MediaFile;
using Gather.Services.Security;
using Gather.Web.Areas.Admin.Extensions;
using Gather.Web.Areas.Admin.Models.Common;
using Gather.Web.Extensions;
using Gather.Web.Framework.Controllers;
using Gather.Web.Framework.Mvc;
using Gather.Services.Pages;
using Gather.Web.Areas.Admin.Models.Page;
using Gather.Core.Domain.Pages;
using Gather.Web.Models.Media;
using Gather.Web.Models.Page;
using Gather.Core;

namespace Gather.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class PageController : ModuleController
    {

        #region Fields

        private readonly CoreSettings _coreSettings;
        private readonly IMediaService _mediaService;
        private readonly IPageService _pageService;
        private readonly IPermissionService _permissionService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors

        public PageController() { }

        public PageController(CoreSettings coreSettings, IPermissionService permissionService, IPageService pageService, IWorkContext workContext, IMediaService mediaService, IWebHelper webHelper)
        {
            _coreSettings = coreSettings;
            _mediaService = mediaService;
            _pageService = pageService;
            _permissionService = permissionService;
            _webHelper = webHelper;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(PermissionProvider.ManagePages))
                return AccessDeniedView();

            var page = _pageService.GetAllPages(Page, _coreSettings.AdminGridPageSize, null, Search);

            var model = new PageListModel
            {
                PageIndex = Page,
                PageSize = _coreSettings.AdminGridPageSize,
                TotalCount = page.TotalCount,
                TotalPages = page.TotalPages,
                Search = Search,
                Pages = page.Select(PrepareListPageModel).ToList()
            };

            PrepareBreadcrumbs();
            return View(model);
        }

        public ActionResult Add()
        {
            if (!_permissionService.Authorize(PermissionProvider.ManagePages))
                return AccessDeniedView();

            PrepareBreadcrumbs();
            AddBreadcrumb("Add New Page", null);

            return View(new PageModel());
        }

        [HttpPost, ValidateInput(false)]
        [FormValueRequired("add")]
        public ActionResult Add(PageModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManagePages))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                try
                {
                    Page page = model.ToEntity();
                    page.CreatedBy = _workContext.CurrentUser.Id;
                    page.LastModifiedBy = _workContext.CurrentUser.Id;
                    _pageService.InsertPage(page);

                    SuccessNotification("The page details have been added successfully.");
                    return RedirectToAction("index");
                }
                catch (Exception)
                {
                    ErrorNotification("An error occurred saving the page details, please try again.");
                }
            }

            PrepareBreadcrumbs();
            AddBreadcrumb("Add New Page", null);

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManagePages))
                return AccessDeniedView();

            var page = _pageService.GetPageById(id);
            if (page == null)
                return RedirectToAction("Index", new { page = Page, search = Search });

            try
            {
                _pageService.DeletePage(page);
                SuccessNotification(page.Title + " has been successfully deleted.");
            }
            catch (Exception)
            {
                ErrorNotification("An error occurred deleting " + page.Title + ", please try again");
            }

            return RedirectToAction("Index", new { id = page.Id, page = Page, search = Search });
        }

        [HttpPost]
        public bool DeleteMedia(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManagePages))
                return false;

            var media = _mediaService.GetMediaById(id);
            if (media == null)
                return false;

            try
            {
                _mediaService.DeleteMedia(media);
                return true;
            }
            catch (Exception) { }

            return false;
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManagePages))
                return AccessDeniedView();

            // get the page
            var page = _pageService.GetPageById(id);

            // check we have a page and they are not deleted
            if (page == null || page.Deleted)
                RedirectToAction("Index");

            var model = page.ToModel();
            model.Media = model.Media.Select(PrepareListMediaModel).OrderBy(m => m.FileName).ToList();

            PrepareBreadcrumbs();
            AddBreadcrumb("Edit Page", null);

            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [FormValueRequired("save")]
        public ActionResult Edit(PageModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManagePages))
                return AccessDeniedView();

            // get the page
            var page = _pageService.GetPageById(model.Id);

            // check we have a page and they are not deleted
            if (page == null || page.Deleted)
                return RedirectToAction("index");

            // Update fields
            page.Title = model.Title;
            page.Content = model.Content;
            page.FileTitle = model.FileTitle;
            page.MetaTitle = model.MetaTitle;
            page.MetaDescription = model.MetaDescription;
            page.MetaKeywords = model.MetaKeywords;
            page.MetaTitle = model.MetaTitle;

            if (ModelState.IsValid)
            {
                try
                {
                    _pageService.UpdatePage(page);
                    SuccessNotification("The page details have been updated successfully.");
                    return RedirectToAction("edit", page.Id);
                }
                catch (Exception)
                {
                    ErrorNotification("An error occurred saving the page details, please try again.");
                }
            }
            else
            {
                ErrorNotification("We were unable to make the change, please review the form and correct the errors.");
            }

            PrepareBreadcrumbs();
            AddBreadcrumb("Edit Page", null);

            model = page.ToModel();
            model.Media = model.Media.Select(PrepareListMediaModel).OrderBy(m => m.FileName).ToList();

            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [FormValueRequired("mediaUpload")]
        public ActionResult Edit(PageModel model, HttpPostedFileBase uploadedFile)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManagePages))
                return AccessDeniedView();

            bool validUpload = true;

            if (string.IsNullOrEmpty(model.MediaItem.Name))
            {
                if (ModelState.ContainsKey("MediaItem.Name"))
                    ModelState["MediaItem.Name"].Errors.Add(new ModelError("Name is required."));
                validUpload = false;
            }

            if (uploadedFile == null || uploadedFile.ContentLength <= 0)
            {
                ErrorNotification("Please select a file to upload.");
                validUpload = false;
            }

            if (validUpload)
            {
                string path = HttpContext.Server.MapPath("/Uploads/Media/");
                string filename = _webHelper.GetUniqueFileName(path, uploadedFile.FileName);

                // Create the media object
                var media = new Media
                {
                    EntityId = model.Id,
                    EntityTypeId = (int) EntityType.Page,
                    FileName = filename,
                    Link = model.MediaItem.Link,
                    Name = model.MediaItem.Name
                };

                // Save the file locally
                uploadedFile.SaveAs(path + filename);

                // Insert the media record
                _mediaService.InsertMedia(media);

                // Clear the fields
                model.MediaItem.Link = "";
                model.MediaItem.Name = "";
            }

            model.Media = _mediaService.GetAllMediaByEntityId(EntityType.Page, model.Id).Select(x => x.ToModel()).Select(PrepareListMediaModel).OrderBy(m => m.FileName).ToList();

            var page = _pageService.GetPageById(model.Id);
            model.IsSystemPage = page.IsSystemPage;
            model.CreatedDate = page.CreatedDate;

            PrepareBreadcrumbs();
            AddBreadcrumb("Edit Page", null);

            return View(model);
        }

        [HttpPost, ActionName("Edit"), ValidateInput(false)]
        [FormValueRequired("hide")]
        public ActionResult Hide(PageModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManagePages))
                return AccessDeniedView();

            // get the page
            var page = _pageService.GetPageById(model.Id);

            // check we have a page and they are not deleted
            if (page == null || page.Deleted || page.IsSystemPage)
                return RedirectToAction("Index");
            page.LastModifiedBy = _workContext.CurrentUser.Id;

            try
            {
                page.Active = !page.Active;
                _pageService.UpdatePage(page);

                SuccessNotification(page.Active ? "The page has been shown successfully." : "The page has been hidden successfully.");

                return RedirectToAction("Edit", page.Id);
            }
            catch (Exception)
            {
                ErrorNotification("An error occurred hiding the page, please try again.");
            }

            PrepareBreadcrumbs();
            AddBreadcrumb("Edit Page", null);

            return View(model);
        }

        private void PrepareBreadcrumbs()
        {
            AddBreadcrumb("Pages", Url.Action("index"));
        }

        #endregion

        #region Prepare Models

        [NonAction]
        private PageModel PrepareListPageModel(Page page)
        {
            var model = page.ToModel();

            model.Actions.Add(new ModelActionLink
            {
                Alt = "Edit",
                Icon = Url.Content("~/Areas/Admin/Content/images/icon-edit.png"),
                Target = Url.Action("edit", new { id = page.Id })
            });

            if(!page.IsSystemPage)
                model.Actions.Add(new DeleteActionLink(page.Id, Search, Page));

            return model;
        }

        [NonAction]
        private MediaModel PrepareListMediaModel(MediaModel media)
        {
            media.Actions.Add(new DeleteActionLink(media.Id, Search, Page));
            return media;
        }
        
        #endregion

        #region Navigation

        public override IList<NavigationSectionModel> RegisterNavigation()
        {
            var sections = new List<NavigationSectionModel>();

            var httpContextWrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
            UrlHelper urlHelper = new UrlHelper(new RequestContext(httpContextWrapper, new RouteData()));

            var section = new NavigationSectionModel
            {
                Icon = urlHelper.Content("~/Areas/Admin/Content/images/menu-pages.png"),
                Name = "page",
                Position = 4,
                RequiredPermissions = new List<PermissionRecord>
                {
                    PermissionProvider.ManagePages
                },
                Target = urlHelper.Action("index", "page", new { area = "admin" }),
                Title = "Pages"
            };

            section.AddChildLink("Manage Pages", urlHelper.Action("index", "page", new { area = "admin" }));
            sections.Add(section);

            return sections;
        }

        #endregion

    }
}