using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Gather.Core;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Users;
using Gather.Services.Security;
using Gather.Services.Users;
using Gather.Web.Areas.Admin.Models.User;
using Gather.Web.Extensions;
using Gather.Web.Framework.Controllers;
using Gather.Web.Framework.Mvc;
using Gather.Web.Models.User;

namespace Gather.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class UserRoleController : ModuleController
    {

        #region Fields

        private readonly CoreSettings _coreSettings;
        private readonly IPermissionService _permissionService;
        private readonly IUserService _userService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors

        public UserRoleController() { }

        public UserRoleController(CoreSettings coreSettings, IPermissionService permissionService, IUserService userService, IWorkContext workContext)
        {
            _coreSettings = coreSettings;
            _permissionService = permissionService;
            _userService = userService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageUserRoles))
                return AccessDeniedView();

            var roles = _userService.GetAllUserRoles(Page, _coreSettings.AdminGridPageSize, true, Search);

            var model = new UserRoleListModel
            {
                PageIndex = Page,
                PageSize = _coreSettings.AdminGridPageSize,
                TotalCount = roles.TotalCount,
                TotalPages = roles.TotalPages,
                Search = Search,
                Roles = roles.Select(PrepareListUserRoleModel).ToList()
            };

            PrepareBreadcrumbs();
            return View(model);
        }

        public ActionResult Add()
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageUserRoles))
                return AccessDeniedView();

            var model = new UserRoleModel
            {
                AvailablePermissionRecords = _permissionService.GetAllPermissionRecords().Select(p => p.ToModel()).ToList()
            };

            PrepareBreadcrumbs();
            AddBreadcrumb("Add New Role", null);

            return View(model);
        }

        [HttpPost]
        public ActionResult Add(UserRoleModel model, FormCollection form)
        {
            if(!_permissionService.Authorize(PermissionProvider.ManageUserRoles))
                return AccessDeniedView();

            if(ModelState.IsValid)
            {
                try
                {
                    var role = model.ToEntity();
                    role.CreatedBy = _workContext.CurrentUser.Id;
                    role.LastModifiedBy = _workContext.CurrentUser.Id;
                    role.LastModifiedDate = DateTime.Now;

                    var allowedPermissionRecords = form["allow"] != null ? form["allow"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();
                    var permissions = _permissionService.GetAllPermissionRecords();

                    foreach (var permissionSystemName in allowedPermissionRecords)
                        role.PermissionRecords.Add(permissions.First(x => x.SystemName == permissionSystemName));

                    _userService.InsertUserRole(role);

                    SuccessNotification("The user role has been inserted successfully.");
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ErrorNotification("An error occurred saving the user role, please try again.");
                }
            }

            PrepareBreadcrumbs();
            AddBreadcrumb("Add New Role", null);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageUserRoles))
                return AccessDeniedView();

            // get the role
            var role = _userService.GetUserRoleById(id);

            // check we have a role and it's not deleted
            if (role == null || role.Deleted)
                return RedirectToAction("Index");

            if (!role.Active)
                WarningNotification("This role is currently hidden. To re-show, use the 'show' button on the right.");

            PrepareBreadcrumbs();
            AddBreadcrumb("Edit Role", null);

            var model = PrepareUserRoleModel(role);
            return View(model);
        }
        
        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult Edit(UserRoleModel model, FormCollection form)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageUserRoles))
                return AccessDeniedView();

            // get the role
            var role = _userService.GetUserRoleById(model.Id);

            // check we have a role and it's not deleted
            if(role == null || role.Deleted)
                return RedirectToAction("Index");

            if(ModelState.IsValid)
            {
                try
                {
                    role.Name = model.Name;
                    role.SystemName = model.SystemName;

                    var allowedPermissionRecords = form["allow"] != null ? form["allow"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();
                    var availablePermissions = _permissionService.GetAllPermissionRecords();

                    role.PermissionRecords.Clear();
                    foreach(var permissionSystemName in allowedPermissionRecords)
                        role.PermissionRecords.Add(availablePermissions.First(x => x.SystemName == permissionSystemName));

                    _userService.UpdateUserRole(role);

                    SuccessNotification("The role has been updated successfully.");
                    return RedirectToAction("Edit", role.Id);
                }
                catch (Exception)
                {
                    ErrorNotification("An error occurred saving the role, please try again.");
                }
            }
            else
            {
                ErrorNotification("We were unable to make the change, please review the form and correct the errors.");
            }

            PrepareBreadcrumbs();
            AddBreadcrumb("Edit Role", null);

            model = PrepareUserRoleModel(role);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("hide")]
        public ActionResult Block(UserRoleModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageUserRoles))
                return AccessDeniedView();

            // get the role
            var role = _userService.GetUserRoleById(model.Id);
                
            // check we have a role and it's not deleted
            if (role == null || role.Deleted)
                return RedirectToAction("Index");

            try
            {
                role.Active = !role.Active;
                _userService.UpdateUserRole(role);

                SuccessNotification("The role has been " + (role.Active ? "shown" : "hidden") + " successfully.");
                return RedirectToAction("Edit", role.Id);
            }
            catch (Exception)
            {
                ErrorNotification("An error occurred " + (role.Active ? "showing" : "hiding") + " the role, please try again.");
            }

            PrepareBreadcrumbs();
            AddBreadcrumb("Edit Role", null);

            model = PrepareUserRoleModel(role);
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageCategories))
                return AccessDeniedView();

            var role = _userService.GetUserRoleById(id);
            if (role == null)
                return RedirectToAction("Index", new { page = Page, search = Search });

            try
            {
                _userService.DeleteUserRole(role);
                SuccessNotification(role.Name + " has been successfully deleted.");
            }
            catch (Exception)
            {
                ErrorNotification("An error occurred deleting " + role.Name + ", please try again");
            }

            return RedirectToAction("Index", new { page = Page, search = Search });
        }

        private void PrepareBreadcrumbs()
        {
            AddBreadcrumb("User Roles", Url.Action("index"));
        }

        #endregion

        #region Prepare Models

        [NonAction]
        public UserRoleModel PrepareUserRoleModel(UserRole userRole)
        {
            var model = userRole.ToModel();

            model.AvailablePermissionRecords = _permissionService.GetAllPermissionRecords().Select(p => p.ToModel()).ToList();

            return model;
        }

        [NonAction]
        public UserRoleModel PrepareListUserRoleModel(UserRole userRole)
        {
            var model = userRole.ToModel();

            if (userRole.SystemName != SystemUserRoleNames.SiteOwner)
            {
                model.Actions.Add(new ModelActionLink
                {
                    Alt = "Edit",
                    Icon = Url.Content("~/Areas/Admin/Content/images/icon-edit.png"),
                    Target = Url.Action("edit", new {id = userRole.Id})
                });
            }

            if (!model.IsSystemRole)
                model.Actions.Add(new DeleteActionLink(userRole.Id));

            return model;
        }

        #endregion

    }
}