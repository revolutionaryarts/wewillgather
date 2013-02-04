using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Gather.Core;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Security;
using Gather.Core.Domain.Users;
using Gather.Services.Security;
using Gather.Services.Users;
using Gather.Web.Areas.Admin.Extensions;
using Gather.Web.Areas.Admin.Models.Common;
using Gather.Web.Areas.Admin.Models.User;
using Gather.Web.Extensions;
using Gather.Web.Models.User;
using Gather.Web.Framework.Controllers;
using Gather.Web.Framework.Mvc;

namespace Gather.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class UserController : ModuleController
    {

        #region Fields

        private readonly CoreSettings _coreSettings;
        private readonly IPermissionService _permissionService;
        private readonly IUserService _userService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors

        public UserController() { }

        public UserController(CoreSettings coreSettings, IPermissionService permissionService, IUserService userService, IWebHelper webHelper, IWorkContext workContext)
        {
            _coreSettings = coreSettings;
            _permissionService = permissionService;
            _userService = userService;
            _webHelper = webHelper;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            var users = _userService.GetAllUsers(Page, _coreSettings.AdminGridPageSize, null, Search);

            var model = new UserListModel
            {
                PageIndex = Page,
                PageSize = _coreSettings.AdminGridPageSize,
                TotalCount = users.TotalCount,
                TotalPages = users.TotalPages,
                Search = Search,
                Users = users.Select(PrepareListUserModel).ToList()
            };

            PrepareBreadcrumbs();
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(id);
            if (user == null)
            {
                ErrorNotification("The user couldn't be found or has already been deleted.");
                return RedirectToAction("Index");
            }

            try
            {
                _userService.DeleteUser(user);
                SuccessNotification(user.DisplayName + " has been successfully deleted.");
            }
            catch (Exception)
            {
                ErrorNotification("An error occurred deleting " + user.DisplayName + ", please try again");
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageUsers))
                return AccessDeniedView();

            // get the user
            var user = _userService.GetUserById(id);

            // check we have a user and they are not deleted
            if (user == null)
            {
                ErrorNotification("The user couldn't be found or has been deleted.");
                return RedirectToAction("Index");
            }

            // If the user is the site owner, kick out anyone but the site owner
            // OR, if the user is an admin, kick out anyone that can't edit admins
            if ((user.UserRoles.Any(x => x.SystemName == SystemUserRoleNames.SiteOwner) && _workContext.CurrentUser.UserRoles.All(x => x.SystemName != SystemUserRoleNames.SiteOwner)) ||
                (user.UserRoles.Any(x => x.SystemName == SystemUserRoleNames.Administrators) && !_permissionService.Authorize(PermissionProvider.EditAdmins)))
                return AccessDeniedView();

            if (!user.Active)
                WarningNotification("This user is currently blocked. To unblock them, use the 'unblock' button on the right.");

            PrepareBreadcrumbs();
            AddBreadcrumb("Edit User", null);

            var model = PrepareUserEditModel(user);
            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult Edit(UserEditModel model, FormCollection form)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageUsers))
                return AccessDeniedView();

            // get the user
            var user = _userService.GetUserById(model.User.Id);

            // check we have a user and they are not deleted
            if (user == null)
            {
                ErrorNotification("The user couldn't be found or has been deleted.");
                return RedirectToAction("index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.ContactUsBio = model.User.ContactUsBio;
                    user.DisplayName = model.User.DisplayName;
                    user.Email = model.User.Email;
                    user.EmailDisclosureId = model.User.EmailDisclosureId;
                    user.ShowOnContactUs = model.User.ShowOnContactUs;
                    user.Telephone = model.User.Telephone;
                    user.TelephoneDisclosureId = model.User.TelephoneDisclosureId;
                    user.UserName = model.User.UserName;
                    user.Website = model.User.Website;
                    user.WebsiteDisclosureId = model.User.WebsiteDisclosureId;

                    if (_permissionService.Authorize(PermissionProvider.PromoteUsers))
                    {
                        var assignedUserRoles = form["roles"] != null ? form["roles"].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();
                        var availableUserRoles = _userService.GetAllUserRoles(true).ToList();
                        bool isSiteOwner = user.UserRoles.Any(x => x.SystemName == SystemUserRoleNames.SiteOwner);

                        var siteOwnerRole = _userService.GetUserRoleBySystemName(SystemUserRoleNames.SiteOwner);
                        if (!isSiteOwner && assignedUserRoles.Contains(siteOwnerRole.Id.ToString()))
                        {
                            var siteOwners = _userService.GetAllUsersInRole(SystemUserRoleNames.SiteOwner);
                            foreach(var siteOwner in siteOwners)
                            {
                                siteOwner.UserRoles.Remove(siteOwnerRole);
                                _userService.UpdateUser(siteOwner);
                            }
                        }

                        user.UserRoles.Clear();
                        foreach (var userRoleId in assignedUserRoles.Select(int.Parse))
                            user.UserRoles.Add(availableUserRoles.First(x => x.Id == userRoleId));

                        if(isSiteOwner)
                            user.UserRoles.Add(availableUserRoles.First(x => x.SystemName == SystemUserRoleNames.SiteOwner));
                    }

                    _userService.UpdateUser(user);

                    SuccessNotification("The user details have been updated successfully.");
                    return RedirectToAction("edit", user.Id);
                }
                catch (Exception)
                {
                    ErrorNotification("An error occurred saving the user details, please try again.");
                }
            }
            else
            {
                ErrorNotification("We were unable to make the change, please review the form and correct the errors.");
            }

            PrepareBreadcrumbs();
            AddBreadcrumb("Edit User", null);

            model = PrepareUserEditModel(user);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("block")]
        public ActionResult Block(UserEditModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageUsers))
                return AccessDeniedView();

            // get the user
            var user = _userService.GetUserById(model.User.Id);

            // check we have a user and they are not deleted
            if (user == null)
            {
                ErrorNotification("The user couldn't be found or has been deleted.");
                return RedirectToAction("index");
            }

            try
            {
                user.Active = !user.Active;
                _userService.UpdateUser(user);

                SuccessNotification("The user has been " + (user.Active ? "unblocked" : "blocked") + " successfully.");
                return RedirectToAction("edit", user.Id);
            }
            catch (Exception)
            {
                ErrorNotification("An error occurred blocking the user, please try again.");
            }

            PrepareBreadcrumbs();
            AddBreadcrumb("Edit User", null);

            model = PrepareUserEditModel(user);
            return View(model);
        }

        private void PrepareBreadcrumbs()
        {
            AddBreadcrumb("Users", Url.Action("index"));
        }

        #endregion

        #region Prepare Models

        [NonAction]
        private UserModel PrepareListUserModel(User user)
        {
            var model = user.ToModel();

            if ((user.UserRoles.All(x => x.SystemName != SystemUserRoleNames.SiteOwner) && 
                (user.UserRoles.All(x => x.SystemName != SystemUserRoleNames.Administrators) || _permissionService.Authorize(PermissionProvider.EditAdmins))) ||
                (_workContext.CurrentUser != null && _workContext.CurrentUser.UserRoles.Any(x => x.SystemName == SystemUserRoleNames.SiteOwner)))
            {
                model.Actions.Add(new ModelActionLink
                {
                    Alt = "Edit",
                    Icon = Url.Content("~/Areas/Admin/Content/images/icon-edit.png"),
                    Target = Url.Action("edit", new {id = user.Id})
                });

                if (user.UserRoles.All(x => x.SystemName != SystemUserRoleNames.SiteOwner))
                    model.Actions.Add(new DeleteActionLink(user.Id));
            }

            return model;
        }

        [NonAction]
        private UserEditModel PrepareUserEditModel(User user)
        {
            var model = new UserEditModel
            {
                CanEditRoles = _permissionService.Authorize(PermissionProvider.PromoteUsers),
                IsSiteOwner = _workContext.CurrentUser.UserRoles.Any(x => x.SystemName == SystemUserRoleNames.SiteOwner),
                User = PrepareUserModel(user)
            };

            return model;
        }

        [NonAction]
        private UserModel PrepareUserModel(User user)
        {
            var model = user.ToModel();

            model.AvailableDisclosureLevels = _webHelper.GetAllEnumListItems<DisclosureLevel>();
            model.AvailableUserRoles = _userService.GetAllUserRoles(true).Select(x => x.ToModel()).ToList();

            return model;
        }

        #endregion

        #region Navigation

        public override IList<NavigationSectionModel> RegisterNavigation()
        {
            var sections = new List<NavigationSectionModel>();

            HttpContextWrapper httpContextWrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
            UrlHelper urlHelper = new UrlHelper(new RequestContext(httpContextWrapper, new RouteData()));

            var section = new NavigationSectionModel
            {
                Icon = urlHelper.Content("~/Areas/Admin/Content/images/menu-users.png"),
                Name = "users",
                Position = 998,
                RequiredPermissions = new List<PermissionRecord>
                {
                    PermissionProvider.ManageUsers
                },
                Target = urlHelper.Action("index", "user", new { area = "admin" }),
                Title = "Users"
            };

            section.AddChildLink("Manage Users", urlHelper.Action("index", "user", new { area = "admin" }));
            section.AddChildLink("Roles", urlHelper.Action("index", "userrole", new { area = "admin" }), new List<PermissionRecord>
            {
                PermissionProvider.ManageUserRoles
            });

            sections.Add(section);

            return sections;
        }

        #endregion

    }
}