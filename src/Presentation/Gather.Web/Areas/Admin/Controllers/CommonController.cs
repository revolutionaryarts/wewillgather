using System;
using System.Linq;
using System.Web.Mvc;
using Gather.Core;
using Gather.Core.Infrastructure;
using Gather.Services.Security;
using Gather.Web.Areas.Admin.Models.Common;
using Gather.Web.Controllers;
using Gather.Web.Extensions;
using Gather.Web.Framework.UI;

namespace Gather.Web.Areas.Admin.Controllers
{
    public class CommonController : BaseController
    {

        #region Fields

        private readonly ILayoutPropertyHelper _layoutPropertyHelper;
        private readonly IPermissionService _permissionService;
        private readonly ITypeFinder _typeFinder;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors

        public CommonController(ILayoutPropertyHelper layoutPropertyHelper, IPermissionService permissionService, ITypeFinder typeFinder, IWorkContext workContext)
        {
            _layoutPropertyHelper = layoutPropertyHelper;
            _permissionService = permissionService;
            _typeFinder = typeFinder;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public ActionResult AccessDenied()
        {
            ErrorNotification("You don't have permission to perform the selected action.");
            AddBreadcrumb("Access Denied", null);
            return View();
        }

        public ActionResult AdminBar()
        {
            var model = _workContext.CurrentUser.ToModel();
            return PartialView("_AdminBar", model);
        }

        public ActionResult Delete(string controllerName, string actionName, int id, string search, string page, string additionalControllerName, string additionalActionName, string additionalData, bool ajax)
        {
            var model = new DeleteConfirmationModel
            {
                ActionName = actionName,
                AdditionalActionName = additionalActionName,
                AdditionalControllerName = additionalControllerName,
                AdditionalData = additionalData,
                Ajax = ajax,
                ControllerName = controllerName,
                Id = id,
                Search = search,
                Page = page
            };

            return PartialView("_Delete", model);
        }

        public ActionResult Navigation()
        {
            var model = new NavigationModel();

            var moduleTypes = _typeFinder.FindClassesOfType<ModuleController>();
            var moduleInstances = moduleTypes.Select(moduleType => (ModuleController)Activator.CreateInstance(moduleType)).ToList();

            foreach (var module in moduleInstances)
            {
                var sections = module.RegisterNavigation();
                foreach (var section in sections.Where(s => s.RequiredPermissions.All(x => _permissionService.Authorize(x))))
                {
                    for (int i = section.Items.Count - 1; i >= 0; i--)
                        if(section.Items[i].RequiredPermissions.Any(x => !_permissionService.Authorize(x)))
                            section.Items.RemoveAt(i);
                    model.Sections.Add(section);
                }
            }

            model.Sections = model.Sections.AsQueryable().OrderBy(n => n.Position).ToList();

            // If the current section is provided, use it
            if (!string.IsNullOrEmpty(_layoutPropertyHelper.CurrentSectionName))
            {
                model.Sections.First(s => s.Name == _layoutPropertyHelper.CurrentSectionName).Active = true;
            }
            else
            {
                // Use the current controller/action details to work out which section we're in
                var currentController = ControllerContext.ParentActionViewContext.RouteData.Values["controller"].ToString().ToLower();
                var currentAction = ControllerContext.ParentActionViewContext.RouteData.Values["action"].ToString().ToLower();
                var currentUrl = Url.Action(currentAction, currentController);

                foreach (var section in model.Sections)
                {
                    if (section.Target == currentUrl)
                    {
                        section.Active = true;
                        break;
                    }

                    if (section.Items.Any(item => item.Target == currentUrl))
                    {
                        section.Active = true;
                        break;
                    }
                }
            }

            return PartialView("_Navigation", model);
        }

        #endregion

    }
}