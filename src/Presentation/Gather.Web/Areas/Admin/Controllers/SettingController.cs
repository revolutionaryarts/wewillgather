using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Gather.Core;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Security;
using Gather.Core.Domain.Settings;
using Gather.Services.Security;
using Gather.Services.Settings;
using Gather.Web.Areas.Admin.Extensions;
using Gather.Web.Areas.Admin.Models.Common;
using Gather.Web.Areas.Admin.Models.Setting;
using Gather.Web.Extensions;
using Gather.Web.Framework.Controllers;
using Gather.Web.Framework.Mvc;
using Gather.Web.Models.Setting;

namespace Gather.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class SettingController : ModuleController
    {

        #region Fields

        private readonly CoreSettings _coreSettings;
        private readonly OwnerSettings _ownerSettings;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors

        public SettingController() { }

        public SettingController(CoreSettings coreSettings, OwnerSettings ownerSettings, IPermissionService permissionService, ISettingService settingService, IWorkContext workContext)
        {
            _coreSettings = coreSettings;
            _ownerSettings = ownerSettings;
            _permissionService = permissionService;
            _settingService = settingService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageSettings))
                return AccessDeniedView();

            var settings = _settingService.GetAllSettings(Page, _coreSettings.AdminGridPageSize, null, Search);

            var model = new SettingListModel
            {
                PageIndex = Page,
                PageSize = _coreSettings.AdminGridPageSize,
                TotalCount = settings.TotalCount,
                TotalPages = settings.TotalPages,
                Search = Search,
                Settings = settings.Select(PrepareListSettingModel).ToList()
            };

            PrepareBreadcrumbs();
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageSettings))
                return AccessDeniedView();

            // Get the setting
            var setting = _settingService.GetSettingById(id);
            if (setting == null || setting.Deleted || setting.Name.ToLower().Contains(typeof(CoreSettings).Name.ToLower()))
                return RedirectToAction("Index");

            PrepareBreadcrumbs();
            AddBreadcrumb("Edit Setting", null);

            var model = setting.ToModel();
            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult Edit(SettingModel model)
        {
            // Check the user's permissions
            if (!_permissionService.Authorize(PermissionProvider.ManageSettings))
                return AccessDeniedView();

            // Get the setting
            var setting = _settingService.GetSettingById(model.Id);
            if (setting == null || setting.Deleted)
                return RedirectToAction("Index");

            // Ensure the form is valid
            if (ModelState.IsValid)
            {
                try
                {
                    setting.LastModifiedBy = _workContext.CurrentUser.Id;
                    setting.Value = model.Value;
                    _settingService.UpdateSetting(setting, true);

                    SuccessNotification("The setting details have been updated successfully.");
                    return RedirectToAction("Edit", setting.Id);
                }
                catch
                {
                    ErrorNotification("An error occurred saving the setting details, please try again.");
                }
            }
            else
            {
                ErrorNotification("We were unable to make the change, please review the form and correct the errors.");
            }

            // Build breacrumbs
            PrepareBreadcrumbs();
            AddBreadcrumb("Edit Setting", null);

            return View(model);
        }

        public ActionResult SiteOwner()
        {
            // Check the user's permissions
            if (!_permissionService.Authorize(PermissionProvider.ManageSiteOwner))
                return AccessDeniedView();

            // Build breadcrumbs
            PrepareBreadcrumbs();
            AddBreadcrumb("Site Owner", null);

            var model = new SiteOwnerModel
            {
                MailEnableSSL = _ownerSettings.MailEnableSSL,
                MailFromDisplayName = _ownerSettings.MailFromDisplayName,
                MailFromEmail = _ownerSettings.MailFromEmail,
                MailHost = _ownerSettings.MailHost,
                MailPassword = _ownerSettings.MailPassword,
                MailPort = int.Parse(_ownerSettings.MailPort),
                MailUseDefaultCredentials = _ownerSettings.MailDefaultCredentials,
                MailUsername = _ownerSettings.MailUsername,
                TwitterAccessToken = _ownerSettings.TwitterAccessToken,
                TwitterAccessTokenSecret = _ownerSettings.TwitterAccessTokenSecret
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult SiteOwner(SiteOwnerModel model)
        {
            // Check permissions
            if (!_permissionService.Authorize(PermissionProvider.ManageSiteOwner))
                return AccessDeniedView();

            // Build breadcrumbs
            PrepareBreadcrumbs();
            AddBreadcrumb("Site Owner", null);

            // Ensure the form is valid
            if (ModelState.IsValid)
            {
                try
                {
                    _ownerSettings.MailDefaultCredentials = model.MailUseDefaultCredentials;
                    _ownerSettings.MailEnableSSL = model.MailEnableSSL;
                    _ownerSettings.MailFromDisplayName = model.MailFromDisplayName;
                    _ownerSettings.MailFromEmail = model.MailFromEmail;
                    _ownerSettings.MailHost = model.MailHost;
                    _ownerSettings.MailPassword = model.MailPassword;
                    _ownerSettings.MailPort = model.MailPort.ToString();
                    _ownerSettings.MailUsername = model.MailUsername;
                    _ownerSettings.TwitterAccessToken = model.TwitterAccessToken;
                    _ownerSettings.TwitterAccessTokenSecret = model.TwitterAccessTokenSecret;

                    _settingService.SaveSetting(_ownerSettings);
                    SuccessNotification("The site owner settings have been updated successfully.");
                }
                catch
                {
                    ErrorNotification("An error occurred saving the site owner settings, please try again.");
                }
            }
            else
            {
                ErrorNotification("We were unable to make the change, please review the form and correct the errors.");
            }

            return View(model);
        }

        #endregion

        #region Utilities

        private void PrepareBreadcrumbs()
        {
            AddBreadcrumb("Settings", Url.Action("index"));
        }

        #endregion

        #region Prepare Models

        [NonAction]
        private SettingModel PrepareListSettingModel(Setting setting)
        {
            var model = setting.ToModel();

            model.Actions.Add(new ModelActionLink
            {
                Alt = "Edit",
                Icon = Url.Content("~/Areas/Admin/Content/images/icon-edit.png"),
                Target = Url.Action("edit", new { id = setting.Id })
            });

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
                Icon = urlHelper.Content("~/Areas/Admin/Content/images/menu-settings.png"),
                Name = "Categories",
                Position = 999,
                RequiredPermissions = new List<PermissionRecord>
                {
                    PermissionProvider.ManageSettings
                },
                Target = urlHelper.Action("index", "setting", new { area = "admin" }),
                Title = "Settings"
            };

            section.AddChildLink("General Settings", urlHelper.Action("index", "setting", new { area = "admin" }));
            section.AddChildLink("Site Owner Settings", urlHelper.Action("siteowner", "setting", new { area = "admin" }), new List<PermissionRecord> { PermissionProvider.ManageSiteOwner });
            sections.Add(section);

            return sections;
        }

        #endregion

    }
}