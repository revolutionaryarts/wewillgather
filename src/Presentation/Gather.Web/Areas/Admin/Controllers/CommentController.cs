using System;
using System.Linq;
using System.Web.Mvc;
using Gather.Core;
using Gather.Core.Domain.Comments;
using Gather.Core.Domain.Common;
using Gather.Services.Comments;
using Gather.Services.MessageQueues;
using Gather.Services.ModerationQueues;
using Gather.Services.Security;
using Gather.Web.Areas.Admin.Models.Comment;
using Gather.Web.Areas.Admin.Models.Project;
using Gather.Web.Extensions;
using Gather.Web.Framework.Controllers;
using Gather.Web.Framework.Mvc;
using Gather.Web.Framework.UI.Tabbing;
using Gather.Web.Models.Comment;

namespace Gather.Web.Areas.Admin.Controllers
{
    public class CommentController : ModuleController
    {

        #region Fields

        private readonly CoreSettings _coreSettings;
        private readonly ICommentService _commentService;
        private readonly IMessageQueueService _messageQueueService;
        private readonly IModerationQueueService _moderationQueueService;
        private readonly IPermissionService _permissionService;
        private readonly ITabHelper _tabHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors

        public CommentController() { }

        public CommentController(CoreSettings coreSettings, ICommentService commentService, IModerationQueueService moderationQueueService, IMessageQueueService messageQueueService, IPermissionService permissionService, ITabHelper tabHelper, IWorkContext workContext)
        {
            _coreSettings = coreSettings;
            _commentService = commentService;
            _messageQueueService = messageQueueService;
            _moderationQueueService = moderationQueueService;
            _permissionService = permissionService;
            _tabHelper = tabHelper;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageComments))
                return AccessDeniedView();

            bool activeFilter = (Filter == null || Filter == "1");

            var comments = _commentService.GetAllComments(Page, _coreSettings.AdminGridPageSize, activeFilter, Search);

            var model = new CommentListModel
            {
                Comments = comments.Select(PrepareListCommentModel).OrderByDescending(x => x.CreatedDate).ToList(),
                Filter = activeFilter ? "1" : "0",
                PageIndex = Page,
                PageSize = _coreSettings.AdminGridPageSize,
                Search = Search,
                TotalCount = comments.TotalCount,
                TotalPages = comments.TotalPages
            };

            _tabHelper.CurrentValue = activeFilter ? "1" : "0";
            _tabHelper.Add("Public", "1");
            _tabHelper.Add("Inactive", "0");

            PrepareBreadcrumbs();
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageComments))
                return AccessDeniedView();

            var comment = _commentService.GetCommentById(id);
            if (comment == null)
                return RedirectToAction("index", new { page = Page, search = Search });

            if (comment.Deleted)
            {
                ErrorNotification("");
                return RedirectToAction("index", new { page = Page, search = Search });
            }

            try
            {
                _commentService.DeleteComment(comment);
                SuccessNotification("The comment has been successfully deleted.");
            }
            catch (Exception)
            {
                ErrorNotification("An error occurred deleting the comment, please try again");
            }

            return RedirectToAction("index", new { page = Page, search = Search });
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageComments))
                return AccessDeniedView();

            var comment = _commentService.GetCommentById(id);

            if (comment == null || comment.Deleted)
                return RedirectToAction("index");

            if (!comment.Active)
                WarningNotification("This comment is currently hidden. To re-show, use the 'show' button on the right.");

            PrepareBreadcrumbs();
            AddBreadcrumb("Edit Comment", null);

            var model = PrepareCommentModel(comment);
            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult Edit(CommentModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageComments))
                return AccessDeniedView();

            var comment = _commentService.GetCommentById(model.Id);

            if (comment == null || comment.Deleted)
                return RedirectToAction("index");

            comment.UserComment = model.UserComment;

            if(ModelState.IsValid)
            {
                try
                {
                    _commentService.UpdateComment(comment);
                    SuccessNotification("The comments details have been updated successfully.");
                    return RedirectToAction("Edit", comment.Id);
                }
                catch (Exception)
                {
                    ErrorNotification("An error occurred saving the comment details, please try again.");
                }
            }

            PrepareBreadcrumbs();
            AddBreadcrumb("Edit Comment", null);

            model = PrepareCommentModel(comment);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("hide")]
        public ActionResult Hide(CommentModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManagePages))
                return AccessDeniedView();

            // get the comment
            var comment = _commentService.GetCommentById(model.Id);

            // check we have a comment and it's not deleted
            if (comment == null || comment.Deleted)
                return RedirectToAction("index");

            try
            {
                comment.Active = !comment.Active;
                _commentService.UpdateComment(comment);

                SuccessNotification(comment.Active ? "The comment has been reshown successfully." : "The comment has been hidden successfully.");
                return RedirectToAction("edit", comment.Id);
            }
            catch (Exception)
            {
                ErrorNotification("An error occurred hiding the comment, please try again.");
            }

            PrepareBreadcrumbs();
            AddBreadcrumb("Edit Comment", null);

            return View(model);
        }

        private void PrepareBreadcrumbs()
        {
            AddBreadcrumb("Actions", Url.Action("index", "project"));
            AddBreadcrumb("Comments", Url.Action("index"));
        }

        #region Comment Complaints

        public ActionResult Comment(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageProjects))
                throw new ArgumentException("Invalid permissions.");

            var queue = _moderationQueueService.GetProjectCommentModerationByModerationQueueId(id);

            // check we have a queue item and they are not deleted
            if (queue == null || queue.Deleted)
                return RedirectToAction("index", "moderation");

            var model = queue.ToModel();

            // check we have a comment and they are not deleted
            if (queue.Comment.Deleted)
               return RedirectToAction("index");

            return View(model);
        }

        [HttpPost, ActionName("Comment")]
        [FormValueExists("remove", "remove comment", "type")]
        [FormValueRequired("remove")]
        public ActionResult Remove(int id, bool type, ProjectCommentModerationModel form)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageProjects))
                throw new ArgumentException("Invalid permissions.");

            // Get the queue
            var queue = _moderationQueueService.GetProjectCommentModerationByModerationQueueId(id);
            if (queue == null || queue.Deleted)
                return RedirectToAction("index", "moderation");

            // Get the comment
            var comment = queue.Comment;
            if (comment == null)
                return RedirectToAction("index");

            var model = queue.ToModel();

            try
            {
                if (type)
                {
                    comment.ModeratedBy = _workContext.CurrentUser.Id;
                    comment.ModeratedDate = DateTime.Now;
                    comment.Deleted = true;

                    // Update the comment entity
                    _commentService.UpdateComment(comment);

                    // Create a success notification
                    SuccessNotification("The comment has been removed.");

                    // Send message to the comment flagger
                    _messageQueueService.CommentMessage(comment, MessageType.CommentRemovalApproved, queue.ModerationQueue.CreatedBy, form.ModerationQueue.Notes, form.UserMessage);

                    // Remove outstanding comment complaints if any are in the queue as the comment has been deleted.
                    _moderationQueueService.RemoveAllProjectCommentModerationEntriesByCommentId(comment.Id, model.ModerationQueue.Id);
                }
                else
                {                    
                    comment.ModeratedBy = _workContext.CurrentUser.Id;
                    comment.ModeratedDate = DateTime.Now;
                    comment.ModerationRequestCount = comment.ModerationRequestCount-1;

                    // Update the comment entity
                    _commentService.UpdateComment(comment);

                    // Create a success notification
                    SuccessNotification("The comment remains active.");

                    // Send message to comment author and the comment flagger
                    _messageQueueService.CommentMessage(comment, MessageType.CommentRemovalRejected, queue.ModerationQueue.CreatedBy, "", form.UserMessage);
                }

                // Close the moderation queue item
                var queueUpdate = _moderationQueueService.GetById(id);
                queueUpdate.StatusType = ModerationStatusType.Closed;
                _moderationQueueService.UpdateModerationQueue(queueUpdate);

                return RedirectToRoute("Admin_default", new { Controller = "moderation", Action = "index" });
            }
            catch (Exception ex)
            {
                ErrorNotification(ex.ToString());
            }

            return View(model);
        }

        #endregion

        #endregion

        #region Prepare Models

        [NonAction]
        private CommentModel PrepareCommentModel(Comment comment)
        {
            if (comment == null)
                return null;

            var model = comment.ToModel();
            model.Project = comment.Project.ToModel();

            if (comment.InResponseTo != null)
            {
                model.InResponseTo = comment.InResponseTo.ToModel();
                model.InResponseTo.Project = comment.InResponseTo.Project.ToModel();
            }

            return model;
        }
        
        [NonAction]
        private CommentModel PrepareListCommentModel(Comment comment)
        {
            if (comment == null)
                return null;

            var model = PrepareCommentModel(comment);

            model.Actions.Add(new ModelActionLink
            {
                Alt = "Edit",
                Icon = Url.Content("~/Areas/Admin/Content/images/icon-edit.png"),
                Target = Url.Action("edit", new { id = comment.Id })
            });

            model.Actions.Add(new DeleteActionLink(comment.Id, Search, Page));

            return model;
        }

        #endregion

    }
}