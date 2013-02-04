using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.ModerationQueue;
using Gather.Core.Domain.Security;
using Gather.Services.ModerationQueues;
using Gather.Services.Security;
using Gather.Web.Areas.Admin.Extensions;
using Gather.Web.Areas.Admin.Models.Common;
using Gather.Web.Areas.Admin.Models.ModerationQueue;
using Gather.Web.Extensions;
using Gather.Web.Framework.Controllers;
using Gather.Web.Framework.Mvc;
using Gather.Web.Models.ModerationQueue;

namespace Gather.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class ModerationController : ModuleController
    {

        #region Fields

        private readonly CoreSettings _coreSettings;
        private readonly IPermissionService _permissionService;
        private readonly IModerationQueueService _moderationQueueService;

        #endregion

        #region Constructors

        public ModerationController() { }

        public ModerationController(CoreSettings coreSettings, IPermissionService permissionService, IModerationQueueService moderationQueueService)
        {
            _coreSettings = coreSettings;
            _permissionService = permissionService;
            _moderationQueueService = moderationQueueService;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageModerationQueue))
                return AccessDeniedView();

            var queue = _moderationQueueService.GetAllModerationQueueEntries(Page, _coreSettings.AdminGridPageSize, Search);

            var model = new ModerationQueueListModel
            {
                PageIndex = Page,
                PageSize = _coreSettings.AdminGridPageSize,
                TotalCount = queue.TotalCount,
                TotalPages = queue.TotalPages,
                Search = Search,
                ModerationQueue = queue.Select(PrepareListModel).ToList()
            };

            AddBreadcrumb("Moderation Queue", Url.Action("index"));
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.DeleteModerationRequests))
                return AccessDeniedView();

            var queue = _moderationQueueService.GetById(id);
            if (queue == null)
                return RedirectToAction("index", new { page = Page, search = Search });

            try
            {
                queue.Deleted = true;
                _moderationQueueService.UpdateModerationQueue(queue);
                SuccessNotification("The moderation request has been successfully deleted.");
            }
            catch (Exception)
            {
                ErrorNotification("An error occurred deleting the moderation request, please try again");
            }

            return RedirectToAction("index", new { page = Page, search = Search });
        }

        public ActionResult ProjectApproval(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageModerationQueue))
                return AccessDeniedView();

            // get the category
            var queue = _moderationQueueService.GetProjectApprovalByModerationQueueId(id);

            // check we have a category and they are not deleted
            if (queue == null || queue.Deleted)
                return RedirectToAction("Index");
            
            return RedirectToRoute("Admin_default", new { Controller = "project", Action = "edit", id = queue.Project.Id, moderationId = id});
        }

        public ActionResult ProjectChange(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageModerationQueue))
                return AccessDeniedView();

            // get the category
            var queue = _moderationQueueService.GetProjectChangeRequestByModerationQueueId(id);

            // check we have a category and they are not deleted
            if (queue == null || queue.Deleted)
                return RedirectToAction("Index");

            return RedirectToRoute("Admin_default", new { Controller = "project", Action = "change", id = queue.ChangeProject.Id, moderationid = id });
        }

        #endregion

        #region Prepare Models

        [NonAction]
        private ModerationQueueModel PrepareListModel(ModerationQueue queue)
        {
            var model = queue.ToModel();

            var editLink = new ModelActionLink
            {
                Alt = "Edit",
                Icon = Url.Content("~/Areas/Admin/Content/images/icon-edit.png"),                
            };

            switch (model.RequestType)
            {
                case ModerationRequestType.ProjectApproval:
                    editLink.Target = Url.Action("projectapproval", new { id = queue.Id });
                    var approvalRequest = _moderationQueueService.GetProjectApprovalByModerationQueueId(queue.Id);
                    if (approvalRequest != null && approvalRequest.Project != null)
                    {
                        model.RelatedDate = approvalRequest.Project.StartDate != null ? approvalRequest.Project.StartDate.Value.ToString("dd/MM/yyyy") : "";

                        var projectLocation = approvalRequest.Project.Locations.FirstOrDefault(x => x.Primary);
                        if (projectLocation != null && projectLocation.Location != null)
                            model.RelatedLocation = projectLocation.Location.Name;

                        model.RelatedProject = approvalRequest.Project.Name;
                    }
                    break;
                case ModerationRequestType.ProjectChange:
                    editLink.Target = Url.Action("change", "project", new { id = queue.Id });
                    var changeRequest = _moderationQueueService.GetProjectChangeRequestByModerationQueueId(queue.Id);
                    if (changeRequest != null && changeRequest.ParentProject != null)
                    {
                        model.RelatedDate = changeRequest.ParentProject.StartDate != null ? changeRequest.ParentProject.StartDate.Value.ToString("dd/MM/yyyy") : "";

                        var projectLocation = changeRequest.ParentProject.Locations.FirstOrDefault(x => x.Primary);
                        if (projectLocation != null && projectLocation.Location != null)
                            model.RelatedLocation = projectLocation.Location.Name;

                        model.RelatedProject = changeRequest.ParentProject.Name;
                    }
                    break;
                case ModerationRequestType.ProjectComment:
                    editLink.Target = Url.Action("comment", "comment", new {id = queue.Id});
                    var commentRequest = _moderationQueueService.GetProjectCommentModerationByModerationQueueId(queue.Id);
                    if (commentRequest != null && commentRequest.Comment != null && commentRequest.Comment.Project != null)
                    {
                        model.RelatedDate = commentRequest.Comment.Project.StartDate != null ? commentRequest.Comment.Project.StartDate.Value.ToString("dd/MM/yyyy") : "";

                        var projectLocation = commentRequest.Comment.Project.Locations.FirstOrDefault(x => x.Primary);
                        if (projectLocation != null && projectLocation.Location != null)
                            model.RelatedLocation = projectLocation.Location.Name;

                        model.RelatedProject = commentRequest.Comment.Project.Name;
                    }
                    break;
                case ModerationRequestType.ProjectModeration:
                    editLink.Target = Url.Action("moderation", "project", new { id = queue.Id });
                    var projectFlagRequest = _moderationQueueService.GetProjectModerationByModerationQueueId(queue.Id);
                    if (projectFlagRequest != null && projectFlagRequest.Project != null)
                    {
                        model.RelatedDate = projectFlagRequest.Project.StartDate != null ? projectFlagRequest.Project.StartDate.Value.ToString("dd/MM/yyyy") : "";

                        var projectLocation = projectFlagRequest.Project.Locations.FirstOrDefault(x => x.Primary);
                        if (projectLocation != null && projectLocation.Location != null)
                            model.RelatedLocation = projectLocation.Location.Name;

                        model.RelatedProject = projectFlagRequest.Project.Name;
                    }
                    break;
                case ModerationRequestType.ProjectWithdrawal:
                    editLink.Target = Url.Action("withdrawal", "project", new { id = queue.Id });
                    var withdrawalRequest = _moderationQueueService.GetProjectWithdrawalByModerationQueueId(queue.Id);
                    if (withdrawalRequest != null && withdrawalRequest.Project != null)
                    {
                        model.RelatedDate = withdrawalRequest.Project.StartDate != null ? withdrawalRequest.Project.StartDate.Value.ToString("dd/MM/yyyy") : "";

                        var projectLocation = withdrawalRequest.Project.Locations.FirstOrDefault(x => x.Primary);
                        if (projectLocation != null && projectLocation.Location != null)
                            model.RelatedLocation = projectLocation.Location.Name;

                        model.RelatedProject = withdrawalRequest.Project.Name;
                    }
                    break;
            }

            model.Actions.Add(editLink);

            if (_permissionService.Authorize(PermissionProvider.DeleteModerationRequests))
                model.Actions.Add(new DeleteActionLink(queue.Id, Search, Page));

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
                Icon = urlHelper.Content("~/Areas/Admin/Content/images/menu-inbox.png"),
                Name = "moderationqueue",
                Position = 1,
                RequiredPermissions = new List<PermissionRecord>
                {
                    PermissionProvider.ManageModerationQueue
                },
                Target = urlHelper.Action("index", "moderation", new { area = "admin" }),
                Title = "Moderation Queue"
            };

            section.AddChildLink("Manage Moderation Queue", urlHelper.Action("index", "moderation", new { area = "admin" }));
            sections.Add(section);

            return sections;
        }

        #endregion

    }
}