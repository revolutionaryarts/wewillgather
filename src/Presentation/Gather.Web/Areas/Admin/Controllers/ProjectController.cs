using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Projects;
using Gather.Core.Domain.Security;
using Gather.Services.MessageQueues;
using Gather.Services.ModerationQueues;
using Gather.Services.Projects;
using Gather.Services.Security;
using Gather.Services.Users;
using Gather.Web.Areas.Admin.Extensions;
using Gather.Web.Areas.Admin.Models.Common;
using Gather.Web.Areas.Admin.Models.Project;
using Gather.Web.Extensions;
using Gather.Web.Framework.Controllers;
using Gather.Web.Framework.Mvc;
using Gather.Web.Framework.UI.Tabbing;
using Gather.Web.Models.Project;
using Gather.Core;
using Gather.Services.Categories;
using Newtonsoft.Json;

namespace Gather.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class ProjectController : ModuleController
    {

        #region Fields

        private readonly CoreSettings _coreSettings;
        private readonly ICategoryService _categoryService;
        private readonly IModerationQueueService _moderationQueueService;
        private readonly IMessageQueueService _messageQueueService;
        private readonly IPermissionService _permissionService;
        private readonly IProjectService _projectService;
        private readonly ITabHelper _tabHelper;
        private readonly IUserService _userService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors

        public ProjectController() { }

        public ProjectController(CoreSettings coreSettings, ICategoryService categoryService, IModerationQueueService moderationQueueService, IPermissionService permissionService, IProjectService projectService, ITabHelper tabHelper, IUserService userService, IMessageQueueService messageQueueService, IWebHelper webHelper, IWorkContext workContext)
        {
            _coreSettings = coreSettings;
            _categoryService = categoryService;
            _moderationQueueService = moderationQueueService;
            _messageQueueService = messageQueueService;
            _permissionService = permissionService;
            _projectService = projectService;
            _tabHelper = tabHelper;
            _userService = userService;
            _webHelper = webHelper;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        #region List

        public ActionResult Index()
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageProjects))
                return AccessDeniedView();

            if (Filter == null)
                Filter = Convert.ToString((int)ProjectStatus.Open);

            if (Filter == Convert.ToString((int)ProjectStatus.Deleted))
            {
                Filter = Convert.ToString((int) ProjectStatus.Open);
            }
            
            int filter; int.TryParse(Filter, out filter);

            var projects = _projectService.GetAllProjects(Page, _coreSettings.AdminGridPageSize, filter, Search);

            _tabHelper.CurrentValue = filter.ToString();
            _tabHelper.Add(ProjectStatus.Open.ToString(), Convert.ToString((int)ProjectStatus.Open));
            _tabHelper.Add(ProjectStatus.InProgress.GetDescription(), Convert.ToString((int)ProjectStatus.InProgress));
            _tabHelper.Add(ProjectStatus.Closed.ToString(), Convert.ToString((int)ProjectStatus.Closed));
            _tabHelper.Add(ProjectStatus.Rejected.ToString(), Convert.ToString((int)ProjectStatus.Rejected));
            _tabHelper.Add(ProjectStatus.Banned.ToString(), Convert.ToString((int)ProjectStatus.Banned));
            _tabHelper.Add(ProjectStatus.Withdrawn.ToString(), Convert.ToString((int)ProjectStatus.Withdrawn));
            _tabHelper.Add("Uncategorised", "-1");

            var model = new ProjectListModel
            {
                PageIndex = Page,
                PageSize = _coreSettings.AdminGridPageSize,
                TotalCount = projects.TotalCount,
                TotalPages = projects.TotalPages,
                Search = Search,
                Projects = projects.Select(PrepareListProjectModel).ToList(),
                Filter = filter.ToString(),
            };

            PrepareBreadcrumbs();
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageProjects))
                return AccessDeniedView();

            var project = _projectService.GetProjectById(id);
            if (project == null)
                return RedirectToAction("index", new { page = Page, search = Search });

            try
            {
                project.Status = ProjectStatus.Deleted;
                _projectService.UpdateProject(project);
                SuccessNotification(project.Name + " has been successfully deleted.");
            }
            catch (Exception)
            {
                ErrorNotification("An error occurred deleting " + project.Name + ", please try again");
            }

            return RedirectToAction("index", new { page = Page, search = Search });
        }

        #endregion

        #region Project Updates + Project Approval

        public ActionResult Edit(int id, int moderationId = 0)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageProjects))
                return AccessDeniedView();

            if (moderationId > 0)
            {
                // Make sure that the moderation id is valid
                var queue = _moderationQueueService.GetProjectApprovalByModerationQueueId(moderationId);
                if (queue == null || queue.Deleted)
                    return RedirectToAction("index", "moderation");

                PrepareModerationBreadcrumbs();
                AddBreadcrumb("Action Approval Request", null);
            }
            else
            {
                PrepareBreadcrumbs();
                AddBreadcrumb("Edit Action", null);
            }

            // Get the action
            var project = _projectService.GetProjectById(id);
            if (project == null || project.Deleted)
                return RedirectToAction("index");

            return View(PrepareProjectModel(project, moderationId));
        }

        [HttpPost, ValidateInput(false)]
        [FormValueRequired("save")]
        public ActionResult Edit(ProjectModel model, FormCollection form)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageProjects))
                return AccessDeniedView();

            if (model.ModerationId > 0)
            {
                // Make sure that the moderation id is valid
                var queue = _moderationQueueService.GetProjectApprovalByModerationQueueId(model.ModerationId);
                if (queue == null || queue.Deleted)
                    return RedirectToAction("index", "moderation");

                PrepareModerationBreadcrumbs();
                AddBreadcrumb("Action Approval Request", null);
            }
            else
            {
                PrepareBreadcrumbs();
                AddBreadcrumb("Edit Action", null);
            }

            // Get the action
            var project = _projectService.GetProjectById(model.Id);
            if (project == null)
                return RedirectToAction("index");

            // Populate model with post back information
            var selectedCategoryIds = form["SelectedCategories"] != null ? form["SelectedCategories"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();
            var availableCategories = _categoryService.GetAllCategories();

            project.Categories.Clear();
            foreach (var categoryId in selectedCategoryIds.Select(int.Parse))
                project.Categories.Add(availableCategories.First(x => x.Id == categoryId));

            project.LastModifiedBy = _workContext.CurrentUser.Id;

            // Update fields                    
            project.ChildFriendly = model.ChildFriendly;
            project.EmailAddress = model.EmailAddress;
            project.EmailDisclosureId = model.EmailDisclosureId;
            project.EndDate = model.EndDate;
            project.Equipment = model.Equipment;
            project.GettingThere = model.GettingThere;
            project.Name = model.Name;
            project.NumberOfVolunteers = model.NumberOfVolunteers;
            project.Objective = model.Objective;
            project.RecurrenceIntervalId = model.RecurrenceIntervalId;
            project.Recurrence = model.Recurrence;
            project.Skills = model.Skills;
            project.StartDate = model.StartDate;
            project.Telephone = model.Telephone;
            project.TelephoneDisclosureId = model.TelephoneDisclosureId;
            project.VolunteerBenefits = model.VolunteerBenefits;
            project.Website = model.Website;
            project.WebsiteDisclosureId = model.WebsiteDisclosureId;
            project.IsRecurring = model.IsRecurring;
            project.ModeratorNotes = model.ModerationNotes;

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the project status
                    if (model.ModerationId > 0)
                    {
                        project.StatusId = (int)ProjectStatus.Open;
                    }
                    else
                    {
                        // Make sure we have the correct status for the date provided
                        // This is important if a project date is changed after it's already started
                        if (project.Status == ProjectStatus.InProgress)
                        {
                            if (project.StartDate > DateTime.Now)
                            {
                                project.StatusId = (int) ProjectStatus.Open;
                            }
                            else if (project.EndDate < DateTime.Now)
                            {
                                project.StatusId = (int) ProjectStatus.Closed;
                            }
                        }
                        else if (project.Status == ProjectStatus.Closed && project.StartDate > DateTime.Now)
                        {
                            project.StatusId = (int) ProjectStatus.Open;
                        }
                        else if (project.Status == ProjectStatus.Closed && project.StartDate < DateTime.Now && project.EndDate > DateTime.Now)
                        {
                            project.StatusId = (int) ProjectStatus.InProgress;
                        }
                        else if (project.Status == ProjectStatus.Open && project.StartDate < DateTime.Now)
                        {
                            if (project.EndDate > DateTime.Now)
                            {
                                project.StatusId = (int) ProjectStatus.InProgress;
                            }
                            else
                            {
                                project.StatusId = (int) ProjectStatus.Closed;
                            }
                        }
                    }
                    
                    // Commit the changes
                    _projectService.UpdateProject(project);

                    // Work out what to do next
                    if (model.ModerationId == 0)
                    {
                        SuccessNotification("The action details have been updated successfully.");
                        return RedirectToAction("edit", project.Id);
                    }

                    // Queue a message to the project owner to say their project has been approved
                    _messageQueueService.ProjectMessage(project, MessageType.ProjectApproved, model.ModerationComment);

                    // Queue a tweet from the site owner account to say a new project has been created
                    _messageQueueService.ProjectTweet(project, MessageType.TweetProjectApproved);

                    // Post to the user's Twitter and Facebook profile to promote the action
                    _userService.Post(project.Owners.FirstOrDefault(), project, ProjectAction.Approved);

                    // Mark the moderation request as resolved
                    var queueUpdate = _moderationQueueService.GetById(model.ModerationId);
                    queueUpdate.Notes = model.ModerationComment;
                    queueUpdate.StatusType = ModerationStatusType.Closed;
                    _moderationQueueService.UpdateModerationQueue(queueUpdate);

                    SuccessNotification("The moderation request has been resolved, the action has been approved.");
                    return RedirectToAction("index", "moderation");
                }
                catch (Exception ex)
                {
                    ErrorNotification(ex.ToString());
                    ErrorNotification("An error occurred saving the action details, please try again.");
                }
            }
            else
            {
                ErrorNotification("We were unable to make the change, please review the form and correct the errors.");
            }

            return View(PrepareProjectModel(project, model.ModerationId));
        }

        [HttpPost, ActionName("Edit"), ValidateInput(false)]
        [FormValueRequired("hide")]
        public ActionResult Hide(ProjectModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageProjects))
                return AccessDeniedView();

            if (model.ModerationId > 0)
            {
                // Check the moderation id is valid
                var queue = _moderationQueueService.GetProjectApprovalByModerationQueueId(model.ModerationId);
                if (queue == null || queue.Deleted)
                    return RedirectToAction("index", "moderation");

                PrepareModerationBreadcrumbs();
                AddBreadcrumb("Action Approval Request", null);
            }
            else
            {
                PrepareBreadcrumbs();
                AddBreadcrumb("Edit Action", null);
            }

            // Get the project
            var project = _projectService.GetProjectById(model.Id);
            if (project == null)
                return RedirectToAction("index");

            project.ModeratorNotes = model.ModerationNotes;

            // Make sure a comment has been entered explaining why the project was not approved
            if (model.ModerationId > 0 && string.IsNullOrEmpty(model.ModerationComment))
                ModelState.AddModelError("ModerationComment", "Please fill in a moderation comment before rejecting the project.");

            // Make sure the model is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Process a standard show/hide
                    if (model.ModerationId == 0)
                    {
                        // Change the status
                        // If the project is already open or in progress, close it
                        // If the project is closed and the start date is in the future, open it
                        // If the project is closed and the start date has passed but the end date is in the future, mark it as in progress
                        if (project.Status == ProjectStatus.Open || project.Status == ProjectStatus.InProgress)
                            project.StatusId = (int) ProjectStatus.Closed;
                        else if (project.StartDate > DateTime.Now)
                            project.StatusId = (int) ProjectStatus.Open;
                        else if(project.EndDate > DateTime.Now)
                            project.StatusId = (int) ProjectStatus.InProgress;

                        // Commit the changes
                        _projectService.UpdateProject(project);
                        SuccessNotification(project.Status == ProjectStatus.Open ? "The action has been opened successfully." : "The action has been closed successfully.");

                        return RedirectToAction("edit", project.Id);
                    }

                    // If we've reached this point, we must be moderating a new project
                    // Flag the project status as rejected
                    project.StatusId = (int)ProjectStatus.Rejected;

                    // Commit the changes
                    _projectService.UpdateProject(project);
                    SuccessNotification("The action has been rejected.");

                    // Queue messages
                    _messageQueueService.ProjectMessage(project, MessageType.ProjectRejected, model.ModerationComment);

                    // Mark the moderation request as resolved
                    var queueUpdate = _moderationQueueService.GetById(model.ModerationId);
                    queueUpdate.Notes = model.ModerationComment;
                    queueUpdate.StatusType = ModerationStatusType.Closed;
                    _moderationQueueService.UpdateModerationQueue(queueUpdate);

                    return RedirectToRoute("Admin_default", new { Controller = "moderation", Action = "index" });
                }
                catch
                {
                    ErrorNotification("An error occurred hiding the action, please try again.");
                }
            }            

            return View(PrepareProjectModel(project, model.ModerationId));
        }

        #endregion

        #region Change Requests

        public ActionResult Change(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageProjects))
                return AccessDeniedView();

            // Get the queue item
            var queue = _moderationQueueService.GetProjectChangeRequestByModerationQueueId(id);
            if (queue == null || queue.Deleted)
                return RedirectToAction("index", "moderation");

            // Get the action
            var project = queue.ChangeProject;
            if (project == null || project.Deleted)
                return RedirectToAction("index");

            // Check we have a parent
            var parentProject = queue.ParentProject;
            if (parentProject == null || parentProject.Deleted)
                return RedirectToAction("index");

            var model = new ProjectEditModel
            {
                Project = PrepareProjectModel(project),
                ParentProject = parentProject.ToModel(),
            };

            model.Project.Volunteers = model.ParentProject.Volunteers;
            model.Project.ModerationNotes = model.ParentProject.ModerationNotes;

            PrepareModerationBreadcrumbs();
            AddBreadcrumb("Action Content Change Request", null);
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [FormValueRequired("change")]
        public ActionResult Change(ProjectEditModel model, FormCollection form)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageProjects))
                return AccessDeniedView();

            var queue = _moderationQueueService.GetProjectChangeRequestByModerationQueueId(model.Id);
            if (queue == null || queue.Deleted)
                return RedirectToAction("index", "moderation");

            // get the action
            var project = queue.ChangeProject;
            if (project == null)
                return RedirectToAction("index");

            // Store the parent
            var parentProject = queue.ParentProject;

            if (ModelState.IsValid)
            {
                try
                {
                    // Update fields 
                    var selectedCategoryIds = form["SelectedCategories"] != null ? form["SelectedCategories"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();
                    var availableCategories = _categoryService.GetAllCategories();

                    parentProject.Categories.Clear();
                    foreach (var categoryId in selectedCategoryIds.Select(int.Parse))
                        parentProject.Categories.Add(availableCategories.First(x => x.Id == categoryId));

                    parentProject.LastModifiedBy = _workContext.CurrentUser.Id;
                    parentProject.ChildFriendly = model.Project.ChildFriendly;
                    parentProject.EmailAddress = model.Project.EmailAddress;
                    parentProject.EmailDisclosureId = model.Project.EmailDisclosureId;
                    parentProject.EndDate = model.Project.EndDate;
                    parentProject.Equipment = model.Project.Equipment;
                    parentProject.GettingThere = model.Project.GettingThere;
                    parentProject.Name = model.Project.Name;
                    parentProject.NumberOfVolunteers = model.Project.NumberOfVolunteers;
                    parentProject.Objective = model.Project.Objective;
                    parentProject.RecurrenceIntervalId = model.Project.RecurrenceIntervalId;
                    parentProject.Recurrence = model.Project.Recurrence;
                    parentProject.Skills = model.Project.Skills;
                    parentProject.StartDate = model.Project.StartDate;
                    parentProject.Telephone = model.Project.Telephone;
                    parentProject.TelephoneDisclosureId = model.Project.TelephoneDisclosureId;
                    parentProject.VolunteerBenefits = model.Project.VolunteerBenefits;
                    parentProject.Website = model.Project.Website;
                    parentProject.WebsiteDisclosureId = model.Project.WebsiteDisclosureId;
                    parentProject.IsRecurring = model.Project.IsRecurring;
                    parentProject.ModeratorNotes = model.Project.ModerationNotes;

                    // Make sure we have the correct status for the date provided
                    // This is important if a project date is changed after it's already started
                    if (parentProject.Status == ProjectStatus.InProgress)
                    {
                        if (parentProject.StartDate > DateTime.Now)
                        {
                            parentProject.StatusId = (int) ProjectStatus.Open;
                        }
                        else if (parentProject.EndDate < DateTime.Now)
                        {
                            parentProject.StatusId = (int) ProjectStatus.Closed;
                        }
                    }
                    else if (parentProject.Status == ProjectStatus.Closed && parentProject.StartDate > DateTime.Now)
                    {
                        parentProject.StatusId = (int)ProjectStatus.Open;
                    }
                    else if (parentProject.Status == ProjectStatus.Closed && parentProject.StartDate < DateTime.Now && parentProject.EndDate > DateTime.Now)
                    {
                        parentProject.StatusId = (int)ProjectStatus.InProgress;
                    }
                    else if (parentProject.Status == ProjectStatus.Open && parentProject.StartDate < DateTime.Now)
                    {
                        if (parentProject.EndDate > DateTime.Now)
                        {
                            parentProject.StatusId = (int)ProjectStatus.InProgress;
                        }
                        else
                        {
                            parentProject.StatusId = (int) ProjectStatus.Closed;
                        }
                    }

                    // Commit the parent project changes
                    _projectService.UpdateProject(parentProject);

                    // Update change request record
                    project.Status = ProjectStatus.Deleted;
                    parentProject.LastModifiedBy = _workContext.CurrentUser.Id;
                    _projectService.UpdateProject(project);

                    SuccessNotification("The action has been updated.");
                    _messageQueueService.ProjectMessage(parentProject, MessageType.ProjectChangeApproved, model.AuthorMessage, model.VolunteersMessage, model.NotifyVolunteers);

                    var queueUpdate = _moderationQueueService.GetById(model.Id);
                    queueUpdate.StatusType = ModerationStatusType.Closed;
                    _moderationQueueService.UpdateModerationQueue(queueUpdate);

                    return RedirectToRoute("Admin_default", new { Controller = "moderation", Action = "index" });
                }
                catch (Exception ex)
                {
                    ErrorNotification(ex.ToString());
                    ErrorNotification("An error occurred saving the action details, please try again.");
                }
            }

            var updateModel = new ProjectEditModel
            {
                Id = model.Id,
                ParentProject = parentProject.ToModel(),
                Project = PrepareProjectModel(project)
            };

            PrepareModerationBreadcrumbs();
            AddBreadcrumb("Action Content Change Request", null);
            return View(updateModel);
        }

        [HttpPost, ActionName("Change"), ValidateInput(false)]
        [FormValueRequired("reject")]
        public ActionResult Reject(ProjectEditModel model)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageProjects))
                return AccessDeniedView();

            // Get the queue item
            var queue = _moderationQueueService.GetProjectChangeRequestByModerationQueueId(model.Id);
            if (queue == null || queue.Deleted)
                return RedirectToAction("index", "moderation");

            // Get the action
            var project = queue.ChangeProject;
            if (project == null)
                return RedirectToAction("index");

            // Store the parent
            var parentProject = queue.ParentProject;

            try
            {
                project.LastModifiedBy = _workContext.CurrentUser.Id;
                project.StatusId = (int)ProjectStatus.Deleted;
                project.ModeratorNotes = model.Project.ModerationNotes;
                _projectService.UpdateProject(project);

                SuccessNotification("The action change request has been rejected.");
                _messageQueueService.ProjectMessage(parentProject, MessageType.ProjectChangeRejected, model.AuthorMessage, model.VolunteersMessage, model.NotifyVolunteers);

                var queueUpdate = _moderationQueueService.GetById(model.Id);
                queueUpdate.StatusType = ModerationStatusType.Closed;
                _moderationQueueService.UpdateModerationQueue(queueUpdate);

                return RedirectToRoute("Admin_default", new { Controller = "moderation", Action = "index" });
            }
            catch (Exception)
            {
                ErrorNotification("An error occurred hiding the action, please try again.");
            }

            var updateModel = new ProjectEditModel
            {
                Project = PrepareProjectModel(project),
                ParentProject = queue.ParentProject.ToModel(),
                Id = model.Id
            };

            PrepareModerationBreadcrumbs();
            AddBreadcrumb("Action Content Change Request", null);
            return View(updateModel);
        }

        #endregion

        #region Project Moderation

        public ActionResult Moderation(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageProjects))
                return AccessDeniedView();

            // Get the queue item
            var queue = _moderationQueueService.GetProjectModerationByModerationQueueId(id);
            if (queue == null || queue.Deleted)
                return RedirectToAction("index", "moderation");

            // Check we have a project and it's not deleted
            if (queue.Project.Deleted)
                return RedirectToAction("index");

            // Build the breadcrumbs
            PrepareModerationBreadcrumbs();

            // Return the view
            if (queue.ComplaintType == ProjectComplaintType.DisputeOwnership)
                return View("DisputeOwnership", PrepareModerationDisputeModel(queue.ToDisputeModel()));

            var model = queue.ToModel();
            if (model.Project.CreatedById != null)
                model.Project.CreatedBy = _userService.GetUserById((int) model.Project.CreatedById).ToModel();
            return View(model);
        }

        [HttpPost, FormValueExists("remove", "ban project", "type")]
        [FormValueRequired("remove")]
        public ActionResult Moderation(int id, bool type, ProjectModerationModel form)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageProjects))
                return AccessDeniedView();

            // Get the queue
            var queue = _moderationQueueService.GetProjectModerationByModerationQueueId(id);
            if (queue == null || queue.Deleted)
                return RedirectToAction("index", "moderation");

            // Get the action in preparation for updating
            var project = queue.Project;
            if (project == null)
                return RedirectToAction("index");

            // Prepare the presenation model
            var model = queue.ToModel();

            try
            {
                var queueUpdate = _moderationQueueService.GetById(id);
                project.ModeratorNotes = form.Project.ModerationNotes;

                if (type)
                {
                    // Update the project properties
                    project.LastModeratorApprovalBy = _workContext.CurrentUser.Id;
                    project.LastModeratorApprovalDate = DateTime.Now;
                    project.Status = ProjectStatus.Banned;

                    // Commit the changes
                    _projectService.UpdateProject(project);
                    SuccessNotification("The moderation request has been resolved, the action has been banned.");

                    // Queue the messages
                    _messageQueueService.ProjectMessage(project, MessageType.ProjectModerationApproved, form.ModerationQueue.Notes, form.VolunteersMessage, true, queue.ModerationQueue.CreatedBy);

                    // Remove outstanding project content complaints if any are in the queue as the action has been deleted.
                    _moderationQueueService.RemoveAllProjectModerationEntriesByProjectId(project.Id, model.ModerationQueue.Id);
                }
                else
                {
                    // Commit the changes
                    _projectService.UpdateProject(project);
                    SuccessNotification("The moderation request has been resolved, the action remains active.");

                    // Queue the messages
                    _messageQueueService.ProjectMessage(project, MessageType.ProjectModerationRejected, "", "", false, queueUpdate.CreatedBy);
                }

                // Mark the moderation request as resolved
                queueUpdate.StatusType = ModerationStatusType.Closed;
                _moderationQueueService.UpdateModerationQueue(queueUpdate);

                return RedirectToRoute("Admin_default", new { Controller = "moderation", Action = "index" });
            }
            catch (Exception ex)
            {
                ErrorNotification(ex.ToString());
                ErrorNotification("An error occurred saving the action details, please try again.");
            }

            PrepareModerationBreadcrumbs();
            return View(model);
        }

        [HttpPost, FormValueExists("dispute", "approve dispute", "type")]
        [FormValueRequired("dispute")]
        public ActionResult Moderation(int id, bool type, ProjectModerationDisputeModel form)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageProjects))
                return AccessDeniedView();

            // Get the queue item
            var queue = _moderationQueueService.GetProjectModerationByModerationQueueId(id);
            if (queue == null || queue.Deleted)
                return RedirectToAction("index", "moderation");

            // Get the action in preparation for updating
            var project = queue.Project;
            if (project == null)
                return RedirectToAction("index");

            var model = queue.ToDisputeModel();

            try
            {
                var queueUpdate = _moderationQueueService.GetById(id);
                project.ModeratorNotes = form.Project.ModerationNotes;

                if (type)
                {
                    // Update the project properties
                    project.LastModeratorApprovalBy = _workContext.CurrentUser.Id;
                    project.LastModeratorApprovalDate = DateTime.Now;
                    if (project.Owners.All(x => x.Id != model.ModerationQueue.CreatedBy))
                        project.Owners.Add(_userService.GetUserById(model.ModerationQueue.CreatedBy));

                    // Commit the changes
                    _projectService.UpdateProject(project);
                    SuccessNotification("The moderation request has been resolved, the action has been changed.");

                    // Queue the messages
                    _messageQueueService.ProjectUserMessage(project, MessageType.ProjectDisputeApproved, _userService.GetUserById(queue.ModerationQueue.CreatedBy), form.ModerationQueue.Notes);
                }
                else
                {
                    // Commit the changes
                    _projectService.UpdateProject(project);
                    SuccessNotification("The moderation request has been resolved, the action has not be changed.");

                    // Queue the messages
                    _messageQueueService.ProjectUserMessage(project, MessageType.ProjectDisputeRejected, _userService.GetUserById(queue.ModerationQueue.CreatedBy), form.ModerationQueue.Notes);
                }

                // Mark the moderation request as resolved
                queueUpdate.StatusType = ModerationStatusType.Closed;
                _moderationQueueService.UpdateModerationQueue(queueUpdate);

                return RedirectToRoute("Admin_default", new { Controller = "moderation", Action = "index" });
            }
            catch (Exception ex)
            {
                ErrorNotification(ex.ToString());
                ErrorNotification("An error occurred saving the action details, please try again.");
            }

            PrepareModerationBreadcrumbs();

            // Return the view
            if (model.ComplaintType == ProjectComplaintType.DisputeOwnership)
                return View("DisputeOwnership", PrepareModerationDisputeModel(model));
            return View(model);
        }

        [HttpPost]
        public bool DeleteOwner(int id, string additionalData)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageProjects) || string.IsNullOrEmpty(additionalData))
                return false;

            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(additionalData);
            if (data == null || !data.ContainsKey("moderationId"))
                return false;

            int modId;
            int.TryParse(data["moderationId"], out modId);

            // Make sure we have a moderation request
            var moderation = _moderationQueueService.GetProjectModerationByModerationQueueId(modId);
            if (moderation == null)
                return false;

            // Make sure we have a user
            var user = moderation.Project.Owners.FirstOrDefault(x => x.Id == id);
            if (user == null)
                return false;

            // Remove the selected user
            moderation.Project.Owners.Remove(user);

            // Commit the changes
            _projectService.UpdateProject(moderation.Project);
            _projectService.InsertProjectUserHistory(new ProjectUserHistory
            {
                AffectedUser = user,
                CommittingUser = _workContext.CurrentUser,
                ProjectUserAction = ProjectUserAction.Removed,
                Project = moderation.Project,
                ProjectUserActionId = (int)ProjectUserAction.Removed
            });

            // Queue messages
            _messageQueueService.ProjectUserMessage(moderation.Project, MessageType.ProjectDisputeOwnerRemoved, user);

            return true;
        }

        #endregion

        #region Project Withdrawal

        public ActionResult Withdrawal(int id)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageProjects))
                return AccessDeniedView();

            // Get the queue item
            var queue = _moderationQueueService.GetProjectWithdrawalByModerationQueueId(id);
            if (queue == null || queue.Deleted)
                return RedirectToAction("index", "moderation");

            // Check we have a project and it's not deleted
            if (queue.Project.Status != ProjectStatus.Open && queue.Project.Status != ProjectStatus.InProgress)
                return RedirectToAction("index");

            PrepareModerationBreadcrumbs();

            var model = queue.ToModel();
            if (model.Project.CreatedById != null)
                model.Project.CreatedBy = _userService.GetUserById((int)model.Project.CreatedById).ToModel();
            return View(model);
        }

        [HttpPost, FormValueExists("remove", "withdraw action", "type")]
        [FormValueRequired("remove")]
        public ActionResult Withdrawal(int id, bool type, ProjectWithdrawalModel form)
        {
            if (!_permissionService.Authorize(PermissionProvider.ManageProjects))
                return AccessDeniedView();

            // Get the queue item
            var queue = _moderationQueueService.GetProjectWithdrawalByModerationQueueId(id);
            if (queue == null || queue.Deleted)
                return RedirectToAction("index", "moderation");

            // Check we have a project
            var project = queue.Project;
            if (project == null)
                return RedirectToAction("index");

            var model = queue.ToModel();

            try
            {
                project.ModeratorNotes = form.Project.ModerationNotes;

                if (type)
                {
                    // Update the project properties
                    project.LastModeratorApprovalBy = _workContext.CurrentUser.Id;
                    project.LastModeratorApprovalDate = DateTime.Now;
                    project.Status = ProjectStatus.Withdrawn;

                    // Commit the changes
                    _projectService.UpdateProject(project);
                    SuccessNotification("The action has been withdrawn.");

                    // Queue the messages
                    _messageQueueService.ProjectMessage(project, MessageType.ProjectWithdrawalApproved, form.ModerationQueue.Notes, form.VolunteersMessage);

                    // Remove outstanding project related items from the queue.
                    _moderationQueueService.RemoveAllProjectEntriesByProjectId(project.Id, model.ModerationQueue.Id);
                }
                else
                {
                    // Commit the changes
                    _projectService.UpdateProject(project);
                    SuccessNotification("The action remains active.");

                    // Queue the messages
                    _messageQueueService.ProjectMessage(project, MessageType.ProjectWithdrawalRejected, form.ModerationQueue.Notes);
                }

                // Mark the moderation request as resolved
                var queueUpdate = _moderationQueueService.GetById(id);
                queueUpdate.StatusType = ModerationStatusType.Closed;
                queueUpdate.Notes = form.ModerationQueue.Notes;
                _moderationQueueService.UpdateModerationQueue(queueUpdate);

                return RedirectToRoute("Admin_default", new { Controller = "moderation", Action = "index" });
            }
            catch (Exception)
            {
                ErrorNotification("An error occurred saving the action details, please try again.");
            }

            PrepareModerationBreadcrumbs();
            return View(model);
        }

        #endregion

        #region Breadcrumbs

        private void PrepareBreadcrumbs()
        {
            AddBreadcrumb("Actions", Url.Action("index"));
        }

        private void PrepareModerationBreadcrumbs()
        {
            AddBreadcrumb("Moderation Queue", Url.Action("index", "moderation"));
        }

        #endregion

        #endregion

        #region Prepare Models

        [NonAction]
        private ProjectModerationDisputeModel PrepareModerationDisputeModel(ProjectModerationDisputeModel model)
        {
            int createdBy = model.Project.CreatedById ?? 0;
            if (model.Project.CreatedById != null)
                model.CreatedByUser = _userService.GetUserById(createdBy).ToModel();
            model.ProjectUserHistory = _projectService.GetAllProjectUserHistory(model.Project.Id).Select(x => x.ToModel()).ToList();
            foreach (var user in model.Project.Owners)
            {
                if (createdBy != user.Id)
                {
                    user.Actions.Add(new DeleteActionLink(user.Id, Search, Page));
                    model.CurrentModerators.Add(user);
                }
            }
            return model;
        }

        [NonAction]
        private ProjectModel PrepareListProjectModel(Project project)
        {
            var model = project.ToModel();

            model.Actions.Add(new ModelActionLink
            {
                Alt = "Edit",
                Icon = Url.Content("~/Areas/Admin/Content/images/icon-edit.png"),
                Target = Url.Action("edit", new { id = project.Id })
            });

            model.Actions.Add(new DeleteActionLink(project.Id, Search, Page));

            return model;
        }

        [NonAction]
        private ProjectModel PrepareProjectModel(Project project, int moderationId = 0)
        {
            var model = new ProjectModel();
            if (project != null)
                model = project.ToModel();

            model.AvailableRecurrenceIntervals = _webHelper.GetAllEnumListItems<RecurrenceInterval>();
            model.AvailableDisclosureLevels = _webHelper.GetAllEnumListItems<DisclosureLevel>();
            model.AvailableStatus = _webHelper.GetAllEnumListItems<ProjectStatus>();
            model.AvailableUsers = _userService.GetAllUsers(true).Select(x => x.ToModel()).ToList();
            model.AvailableCategories = _categoryService.GetAllCategories(0, -1, true).Select(x => x.ToModel()).ToList();
            model.ModerationId = moderationId;

            if (model.CreatedById != null)
                model.CreatedBy = _userService.GetUserById((int) model.CreatedById).ToModel();

            var selectCategories = model.AvailableCategories.Where(category => (model.Categories.Any(x => x.Id == category.Id)));
            selectCategories.ToList().ForEach(x => x.IsChecked = true);

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
                Icon = urlHelper.Content("~/Areas/Admin/Content/images/menu-projects.png"),
                Name = "project",
                Position = 2,
                RequiredPermissions = new List<PermissionRecord>
                {
                    PermissionProvider.ManageProjects
                },
                Target = urlHelper.Action("index", "project", new { area = "admin", filter = (int)ProjectStatus.Open }),
                Title = "Actions"
            };

            section.AddChildLink("Manage Actions", urlHelper.Action("index", "project", new { area = "admin", filter = (int)ProjectStatus.Open }));
            section.AddChildLink("Manage Categories", urlHelper.Action("index", "category", new { area = "admin" }));
            section.AddChildLink("Manage Comments", urlHelper.Action("index", "comment", new { area = "admin" }));
            sections.Add(section);

            return sections;
        }

        #endregion

    }
}