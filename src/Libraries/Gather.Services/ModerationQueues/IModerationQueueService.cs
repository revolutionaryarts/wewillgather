using System.Collections.Generic;
using Gather.Core;
using Gather.Core.Domain.Comments;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.ModerationQueue;
using Gather.Core.Domain.Projects;
using Gather.Core.Domain.Users;

namespace Gather.Services.ModerationQueues
{
    public interface IModerationQueueService
    {

        #region Common

        /// <summary>
        /// Get all comment complaint types as a dictionary
        /// </summary>
        /// <returns>Comment complaint type collection</returns>
        Dictionary<int, string> GetAllCommentComplaintTypes();

        /// <summary>
        /// Get all project complaint types as a dictionary
        /// </summary>
        /// <returns>Project complaint type collection</returns>
        Dictionary<int, string> GetAllProjectComplaintTypes();

        #endregion

        #region Moderation Queue

        /// <summary>
        /// Delete all moderation requests created by a given user
        /// </summary>
        /// <param name="createdBy">User id</param>
        void DeleteModerationQueueByCreatedBy(int createdBy);

        /// <summary>
        /// Get a moderation queue by id
        /// </summary>
        /// <param name="moderationQueueId">Id of item</param>
        /// <returns>ModerationQueue</returns>
        ModerationQueue GetById(int moderationQueueId);

        /// <summary>
        /// Change the owner of a moderation queue request
        /// </summary>
        /// <param name="currentUserId">Current user id</param>
        /// <param name="newUserId">New user id</param>
        void MigrateModerationQueueOwnership(int currentUserId, int newUserId);

        /// <summary>
        /// Updates the moderation gueue
        /// </summary>        
        /// <param name="queue"> ModerationQueue</param>
        void UpdateModerationQueue(ModerationQueue queue);

        /// <summary>
        /// Get all moderation queue items
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>        
        /// <param name="search">Search term</param>
        /// <param name="statusType">Open or Closed items</param>
        /// <returns>Paginated collection</returns>
        IPaginatedList<ModerationQueue> GetAllModerationQueueEntries(int pageIndex, int pageSize, string search = "", ModerationStatusType statusType = ModerationStatusType.Open);

        #endregion

        #region Project Approval

        /// <summary>
        /// Get a moderation queue item by id for project approval items
        /// </summary>
        /// <param name="moderationQueueId">Id of queue item to retrieve</param>
        /// <returns>ProjectApproval</returns>
        ProjectApproval GetProjectApprovalByModerationQueueId(int moderationQueueId);

        /// <summary>
        /// Inserts a moderation project approval item
        /// </summary>
        /// <param name="project">Project object requesting approval</param>
        void InsertModerationQueueProjectApproval(Project project);

        #endregion

        #region Project Change Request

        /// <summary>
        /// Get a moderation queue item by id for project change requests
        /// </summary>
        /// <param name="moderationQueueId">Id of queue item to retrieve</param>
        /// <returns>ProjectChangeRequest</returns>
        ProjectChangeRequest GetProjectChangeRequestByModerationQueueId(int moderationQueueId);

        /// <summary>
        /// Inserts a moderation project withdrawal item
        /// </summary>
        /// <param name="project">Project object requesting withdrawal</param>
        /// <param name="changeProject">Change project</param>
        void InsertModerationQueueProjectChange(Project project, Project changeProject);

        #endregion

        #region Project Comments Moderation

        /// <summary>
        /// Get a moderation queue item by id for project comment complaints
        /// </summary>
        /// <param name="moderationQueueId">Id of queue item to retrieve</param>
        /// <returns>ProjectCommentModeration</returns>
        ProjectCommentModeration GetProjectCommentModerationByModerationQueueId(int moderationQueueId);

        /// <summary>
        /// Insert a moderation queue item for project comment complaints
        /// </summary>
        /// <param name="comment">Comment object</param>
        /// <param name="type">Type of complaint</param>
        /// <param name="reason">Reason for complaint</param>
        void InsertModerationQueueCommentModeration(Comment comment, ProjectCommentComplaintType type, string reason);

        /// <summary>
        /// Updates all comment items to take them out of the queue.
        /// </summary>
        /// <param name="commentId">int</param>
        /// <param name="excludeModerationId">int</param>        
        void RemoveAllProjectCommentModerationEntriesByCommentId(int commentId, int excludeModerationId);

        #endregion

        #region Project Moderation

        /// <summary>
        /// Get a moderation queue item by id for project complaints
        /// </summary>
        /// <param name="moderationQueueId">Id of queue item to retrieve</param>
        /// <returns>ProjectModeration</returns>
        ProjectModeration GetProjectModerationByModerationQueueId(int moderationQueueId);

        /// <summary>
        /// Inserts a moderation projectitem
        /// </summary>
        /// <param name="project">Project object requesting moderation</param>
        /// <param name="reason">string of the reason</param>
        /// <param name="type">ProjectComplaintType</param>
        void InsertModerationQueueProjectModeration(Project project, string reason, ProjectComplaintType type);

        /// <summary>
        /// Updates all project items to take them out of the queue.
        /// </summary>
        /// <param name="projectId">int</param>  
        /// <param name="excludeModerationId">int</param>       
        void RemoveAllProjectModerationEntriesByProjectId(int projectId, int excludeModerationId);

        #endregion

        #region Project Withdrawal

        /// <summary>
        /// Get a moderation queue item by id for project withdrawals
        /// </summary>
        /// <param name="moderationQueueId">Id of queue item to retrieve</param>
        /// <returns>ProjectModeration</returns>
        ProjectWithdrawal GetProjectWithdrawalByModerationQueueId(int moderationQueueId);

        /// <summary>
        /// Inserts a moderation project withdrawal item
        /// </summary>
        /// <param name="project">Project object requesting withdrawal</param>
        /// <param name="reason">Reason for the project withdrawal</param>
        void InsertModerationQueueProjectWithdrawal(Project project, string reason = null);

        /// <summary>
        /// Updates all project items to take them out of the queue.
        /// </summary>
        /// <param name="projectId">int</param>  
        /// <param name="excludeModerationId">int</param>       
        void RemoveAllProjectEntriesByProjectId(int projectId, int excludeModerationId);

        #endregion

    }
}