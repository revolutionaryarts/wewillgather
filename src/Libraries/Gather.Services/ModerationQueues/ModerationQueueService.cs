using System;
using System.Collections.Generic;
using System.Linq;
using Gather.Core;
using Gather.Core.Domain.Comments;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.ModerationQueue;
using Gather.Core.Data;
using Gather.Core.Domain.Projects;
using Gather.Core.Domain.Users;
using Gather.Services.Comments;

namespace Gather.Services.ModerationQueues
{
    public class ModerationQueueService : IModerationQueueService
    {

        #region Fields

        private readonly ICommentService _commentService;
        private readonly IRepository<ModerationQueue> _moderationQueueRepository;
        private readonly IRepository<ProjectApproval> _moderationQueueProjectApprovalRepository;
        private readonly IRepository<ProjectChangeRequest> _moderationQueueProjectChangeRequestRepository;
        private readonly IRepository<ProjectCommentModeration> _moderationQueueProjectCommentModerationRepository;
        private readonly IRepository<ProjectModeration> _moderationQueueProjectModerationRepository;
        private readonly IRepository<ProjectWithdrawal> _moderationQueueProjectWithdrawalRepository;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors

        public ModerationQueueService(ICommentService commentService, IRepository<ProjectApproval> moderationQueueProjectApprovalRepository, IRepository<ProjectChangeRequest> moderationQueueProjectChangeRequestRepository, IRepository<ProjectCommentModeration> moderationQueueProjectCommentModerationRepository, IRepository<ModerationQueue> moderationQueueRepository, IRepository<ProjectModeration> moderationQueueProjectModerationRepository, IRepository<ProjectWithdrawal> moderationQueueProjectWithdrawalRepository, IWorkContext workContext)
        {
            _commentService = commentService;
            _moderationQueueProjectApprovalRepository = moderationQueueProjectApprovalRepository;
            _moderationQueueRepository = moderationQueueRepository;
            _moderationQueueProjectChangeRequestRepository = moderationQueueProjectChangeRequestRepository;
            _moderationQueueProjectCommentModerationRepository = moderationQueueProjectCommentModerationRepository;
            _moderationQueueProjectModerationRepository = moderationQueueProjectModerationRepository;
            _moderationQueueProjectWithdrawalRepository = moderationQueueProjectWithdrawalRepository;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        #region Common

        /// <summary>
        /// Get all comment complaint types as a dictionary
        /// </summary>
        /// <returns>Comment complaint type collection</returns>
        public Dictionary<int, string> GetAllCommentComplaintTypes()
        {
            var levels = from ProjectCommentComplaintType d in Enum.GetValues(typeof(ProjectCommentComplaintType))
                         select new { ID = (int)d, Name = d.GetDescription() };
            return levels.ToDictionary(d => d.ID, d => d.Name);
        }

        /// <summary>
        /// Get all project complaint types as a dictionary
        /// </summary>
        /// <returns>Project complaint type collection</returns>
        public Dictionary<int, string> GetAllProjectComplaintTypes()
        {
            var levels = from ProjectComplaintType d in Enum.GetValues(typeof(ProjectComplaintType))
                         select new { ID = (int)d, Name = d.GetDescription() };
            return levels.ToDictionary(d => d.ID, d => d.Name);
        }

        #endregion

        #region Moderation Queue

        /// <summary>
        /// Delete all moderation requests created by a given user
        /// </summary>
        /// <param name="createdBy">User id</param>
        public void DeleteModerationQueueByCreatedBy(int createdBy)
        {
            if (createdBy == 0)
                return;

            var requests = _moderationQueueRepository.Table.Where(x => x.CreatedBy == createdBy).ToList();
            _moderationQueueRepository.BulkDelete(requests);

            //foreach (var request in requests)
            //    _moderationQueueRepository.Delete(request);
        }

        /// <summary>
        /// Get a moderation queue by id
        /// </summary>
        /// <param name="moderationQueueId">Id of item</param>
        /// <returns>ModerationQueue</returns>
        public ModerationQueue GetById(int moderationQueueId)
        {
            if (moderationQueueId == 0)
                return null;

            var queue = _moderationQueueRepository.GetById(moderationQueueId);
            return queue;
        }

        /// <summary>
        /// Change the owner of a moderation queue request
        /// </summary>
        /// <param name="currentUserId">Current user id</param>
        /// <param name="newUserId">New user id</param>
        public void MigrateModerationQueueOwnership(int currentUserId, int newUserId)
        {
            if (currentUserId == 0 || newUserId == 0)
                return;

            var requests = _moderationQueueRepository.Table.Where(x => x.CreatedBy == currentUserId).ToList();

            foreach (var request in requests)
                request.CreatedBy = newUserId;

            _moderationQueueRepository.BulkUpdate(requests);
        }

        /// <summary>
        /// Updates the moderation gueue
        /// </summary>        
        /// <param name="queue"> ModerationQueue</param>
        public void UpdateModerationQueue(ModerationQueue queue)
        {
            if (queue == null)
                throw new ArgumentNullException("queue");

            queue.ModeratedBy = _workContext.CurrentUser.Id;
            queue.ModeratedDate = DateTime.Now;

            _moderationQueueRepository.Update(queue);
        }

        /// <summary>
        /// Get all moderation queue items
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>        
        /// <param name="search">Search term</param>
        /// <param name="statusType">Open or Closed items</param>
        /// <returns>Paginated collection</returns>
        public IPaginatedList<ModerationQueue> GetAllModerationQueueEntries(int pageIndex, int pageSize, string search = "", ModerationStatusType statusType = ModerationStatusType.Open)
        {
            var query = _moderationQueueRepository.Table;

            query = query.Where(s => s.StatusId == (int)statusType);
            query = query.Where(s => s.Deleted == false);

            if (search != null)
                query = query.Where(s => s.Notes.Contains(search));

            query = query.OrderBy(s => s.Id);

            var queue = new PaginatedList<ModerationQueue>(query, pageIndex, pageSize);
            return queue;
        }

        #endregion

        #region Project Approval

        /// <summary>
        /// Get a moderation queue item by id for project approval items
        /// </summary>
        /// <param name="moderationQueueId">Id of queue item to retrieve</param>
        /// <returns>ProjectApproval</returns>
        public ProjectApproval GetProjectApprovalByModerationQueueId(int moderationQueueId)
        {
            if (moderationQueueId == 0)
                return null;

            var query = from t in _moderationQueueProjectApprovalRepository.Table
                        where t.ModerationQueue.Id == moderationQueueId
                        select t;        

            var queue = query.FirstOrDefault();
            return queue;
        }

        /// <summary>
        /// Inserts a moderation project approval item
        /// </summary>
        /// <param name="project">Project object requesting approval</param>
        public void InsertModerationQueueProjectApproval(Project project)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            var approval = new ProjectApproval
            {
                Project = project,
                ModerationQueue = new ModerationQueue
                {
                    RequestType = ModerationRequestType.ProjectApproval,
                    StatusType = ModerationStatusType.Open,
                    CreatedDate = DateTime.Now,
                    CreatedBy = _workContext.CurrentUser.Id
                }
            };

            _moderationQueueProjectApprovalRepository.Insert(approval);
        }

        #endregion

        #region Project Change Request

        /// <summary>
        /// Get a moderation queue item by id for project change requests
        /// </summary>
        /// <param name="moderationQueueId">Id of queue item to retrieve</param>
        /// <returns>ProjectChangeRequest</returns>
        public ProjectChangeRequest GetProjectChangeRequestByModerationQueueId(int moderationQueueId)
        {
            if (moderationQueueId == 0)
                return null;

            var query = from t in _moderationQueueProjectChangeRequestRepository.Table
                        where t.ModerationQueue.Id == moderationQueueId
                        select t;

            var queue = query.FirstOrDefault();
            return queue;
        }
        
        /// <summary>
        /// Inserts a moderation project withdrawal item
        /// </summary>
        /// <param name="project">Project object requesting withdrawal</param>
        /// <param name="changeProject">Change project</param>
        public void InsertModerationQueueProjectChange(Project project, Project changeProject)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            if (changeProject == null)
                throw new ArgumentNullException("project");

            var withdrawal = new ProjectChangeRequest
            {
                ParentProject = project,
                ChangeProject = changeProject,
                ModerationQueue = new ModerationQueue
                {
                    RequestType = ModerationRequestType.ProjectChange,
                    StatusType = ModerationStatusType.Open,
                    CreatedDate = DateTime.Now,
                    CreatedBy = _workContext.CurrentUser.Id
                }
            };

            _moderationQueueProjectChangeRequestRepository.Insert(withdrawal);

        }

        #endregion

        #region Project Comments Moderation

        /// <summary>
        /// Get a moderation queue item by id for project comment complaints
        /// </summary>
        /// <param name="moderationQueueId">Id of queue item to retrieve</param>
        /// <returns>ProjectCommentModeration</returns>
        public ProjectCommentModeration GetProjectCommentModerationByModerationQueueId(int moderationQueueId)
        {
            if (moderationQueueId == 0)
                return null;

            var query = from t in _moderationQueueProjectCommentModerationRepository.Table
                        where t.ModerationQueue.Id == moderationQueueId
                        select t;

            var queue = query.FirstOrDefault();
            return queue;
        }

        /// <summary>
        /// Insert a moderation queue item for project comment complaints
        /// </summary>
        /// <param name="comment">Comment object</param>
        /// <param name="type">Type of complaint</param>
        /// <param name="reason">Reason for complaint</param>
        public void InsertModerationQueueCommentModeration(Comment comment, ProjectCommentComplaintType type, string reason)
        {
            if (comment == null)
                throw new ArgumentNullException("comment");

            var flag = new ProjectCommentModeration
            {
                Comment = comment,
                ComplaintType = type,
                Reason = reason,
                ModerationQueue = new ModerationQueue
                {
                    RequestType = ModerationRequestType.ProjectComment,
                    StatusType = ModerationStatusType.Open,
                    CreatedDate = DateTime.Now,
                    CreatedBy = _workContext.CurrentUser.Id
                }
            };

            // Increment the comment flag count
            comment.ModerationRequestCount++;

            // Insert the new moderation request
            _moderationQueueProjectCommentModerationRepository.Insert(flag);
            _commentService.UpdateComment(comment);
        }

        /// <summary>
        /// Updates all comment items to take them out of the queue.
        /// </summary>
        /// <param name="commentId">int</param>  
        /// <param name="excludeModerationId">int</param>       
        public void RemoveAllProjectCommentModerationEntriesByCommentId(int commentId, int excludeModerationId)
        {
            var query = _moderationQueueProjectCommentModerationRepository.Table;
            query = query.Where(s => s.Comment.Id == commentId);
            query = query.Where(s => s.ModerationQueue.Id != excludeModerationId);

            var queue = query.ToList();

            foreach (var projectCommentModeration in queue)
            {
                projectCommentModeration.ModerationQueue.Deleted = true;
                projectCommentModeration.ModerationQueue.StatusType = ModerationStatusType.Closed;
                UpdateModerationQueue(projectCommentModeration.ModerationQueue);
            }
        }

        #endregion

        #region Project Moderation

        /// <summary>
        /// Get a moderation queue item by id for project complaints
        /// </summary>
        /// <param name="moderationQueueId">Id of queue item to retrieve</param>
        /// <returns>ProjectModeration</returns>
        public ProjectModeration GetProjectModerationByModerationQueueId(int moderationQueueId)
        {
            if (moderationQueueId == 0)
                return null;

            var query = from t in _moderationQueueProjectModerationRepository.Table
                        where t.ModerationQueue.Id == moderationQueueId
                        select t;

            var queue = query.FirstOrDefault();
            return queue;
        }

        /// <summary>
        /// Inserts a moderation projectitem
        /// </summary>
        /// <param name="project">Project object requesting moderation</param>
        /// <param name="reason">string of the reason</param>
        /// <param name="type">ProjectComplaintType</param>
        public void InsertModerationQueueProjectModeration(Project project, string reason, ProjectComplaintType type)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            var moderation = new ProjectModeration
            {
                Reason = reason,
                ComplaintType = type,
                Project = project,
                ModerationQueue = new ModerationQueue
                {
                    RequestType = ModerationRequestType.ProjectModeration,
                    StatusType = ModerationStatusType.Open,
                    CreatedDate = DateTime.Now,
                    CreatedBy = _workContext.CurrentUser.Id
                }
            };

            _moderationQueueProjectModerationRepository.Insert(moderation);
        }

        /// <summary>
        /// Updates all project items to take them out of the queue.
        /// </summary>
        /// <param name="projectId">int</param>  
        /// <param name="excludeModerationId">int</param>       
        public void RemoveAllProjectModerationEntriesByProjectId(int projectId, int excludeModerationId)
        {
            var query = _moderationQueueProjectModerationRepository.Table;
            query = query.Where(s => s.Project.Id == projectId);
            query = query.Where(s => s.ModerationQueue.Id != excludeModerationId);

            var queue = query.ToList();

            foreach (var projectCommentModeration in queue)
            {
                projectCommentModeration.ModerationQueue.Deleted = true;
                projectCommentModeration.ModerationQueue.StatusType = ModerationStatusType.Closed;
                UpdateModerationQueue(projectCommentModeration.ModerationQueue);
            }
        }

        #endregion

        #region Project Withdrawal

        /// <summary>
        /// Get a moderation queue item by id for project withdrawals
        /// </summary>
        /// <param name="moderationQueueId">Id of queue item to retrieve</param>
        /// <returns>ProjectModeration</returns>
        public ProjectWithdrawal GetProjectWithdrawalByModerationQueueId(int moderationQueueId)
        {
            if (moderationQueueId == 0)
                return null;

            var query = from t in _moderationQueueProjectWithdrawalRepository.Table
                        where t.ModerationQueue.Id == moderationQueueId
                        select t;

            var queue = query.FirstOrDefault();
            return queue;
        }

        /// <summary>
        /// Get a moderation queue item by id for project withdrawals
        /// </summary>
        /// <param name="productId">Id of productId to retrieve</param>
        /// <returns>ProjectModeration</returns>
        public ProjectWithdrawal GetProjectWithdrawalByProductId(int productId)
        {
            if (productId == 0)
                return null;

            var query = from t in _moderationQueueProjectWithdrawalRepository.Table
                        where t.Project.Id == productId && t.ModerationQueue.StatusId == (int)ModerationStatusType.Open
                        select t;

            var queue = query.FirstOrDefault();
            return queue;
        }

        /// <summary>
        /// Inserts a moderation project withdrawal item
        /// </summary>
        /// <param name="project">Project object requesting withdrawal</param>
        /// <param name="reason">Reason for the project withdrawal</param>
        public void InsertModerationQueueProjectWithdrawal(Project project, string reason = null)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            if (reason == null)
                reason = "";

            var productWithdrawal = GetProjectWithdrawalByProductId(project.Id);
            if (productWithdrawal != null) return;

            var withdrawal = new ProjectWithdrawal
            {
                Project = project,
                Reason = reason,
                ModerationQueue = new ModerationQueue
                {
                    RequestType = ModerationRequestType.ProjectWithdrawal,
                    StatusType = ModerationStatusType.Open,
                    CreatedDate = DateTime.Now,
                    CreatedBy = _workContext.CurrentUser.Id
                }
            };

            _moderationQueueProjectWithdrawalRepository.Insert(withdrawal);
        }

        /// <summary>
        /// Updates all project related items to take them out of the queue (change request, comments, moderation requests)
        /// </summary>
        /// <param name="projectId">int</param>  
        /// <param name="excludeModerationId">int</param>       
        public void RemoveAllProjectEntriesByProjectId(int projectId, int excludeModerationId)
        {

            // Remove any outstanding project change request entries
            var query = _moderationQueueProjectChangeRequestRepository.Table;
            query = query.Where(s => s.ParentProject.Id == projectId);
            query = query.Where(s => s.ModerationQueue.Id != excludeModerationId);

            var queue = query.ToList();

            foreach (var projectCommentModeration in queue)
            {
                projectCommentModeration.ModerationQueue.Deleted = true;
                projectCommentModeration.ModerationQueue.StatusType = ModerationStatusType.Closed;
                UpdateModerationQueue(projectCommentModeration.ModerationQueue);
            }

            // Remove any outstanding project comment moderation entries
            var query2 = _moderationQueueProjectCommentModerationRepository.Table;
            query2 = query2.Where(s => s.Comment.Project.Id == projectId);
            query2 = query2.Where(s => s.ModerationQueue.Id != excludeModerationId);

            var queue2 = query2.ToList();

            foreach (var projectCommentModeration in queue2)
            {
                projectCommentModeration.ModerationQueue.Deleted = true;
                projectCommentModeration.ModerationQueue.StatusType = ModerationStatusType.Closed;
                UpdateModerationQueue(projectCommentModeration.ModerationQueue);
            }

            // Remove any outstanding project content moderation entries
            var query3 = _moderationQueueProjectModerationRepository.Table;
            query3 = query3.Where(s => s.Project.Id == projectId);
            query3 = query3.Where(s => s.ModerationQueue.Id != excludeModerationId);

            var queue3 = query3.ToList();

            foreach (var projectCommentModeration in queue3)
            {
                projectCommentModeration.ModerationQueue.Deleted = true;
                projectCommentModeration.ModerationQueue.StatusType = ModerationStatusType.Closed;
                UpdateModerationQueue(projectCommentModeration.ModerationQueue);
            }

        }

        #endregion

        #endregion

    }
}