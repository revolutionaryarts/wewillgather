using System.Collections.Generic;
using Gather.Core;
using Gather.Core.Domain.Comments;
using Gather.Core.Domain.Users;

namespace Gather.Services.Comments
{
    public interface ICommentService
    {
        /// <summary>
        /// Delete a comment
        /// </summary>
        /// <param name="comment">Comment</param>
        /// <param name="persist">Persist delete</param>
        void DeleteComment(Comment comment, bool persist = false);

        /// <summary>
        /// Delete all comments by a given author
        /// </summary>
        /// <param name="userId">User id</param>
        void DeleteCommentsByAuthor(int userId);

        /// <summary>
        /// Get all comments
        /// </summary>
        /// <param name="pageIndex">Current page</param>
        /// <param name="pageSize">Items per page</param>
        /// <param name="active">Active flag</param>
        /// <param name="search">Search query</param>
        /// <returns>Paginated collection of comments</returns>
        IPaginatedList<Comment> GetAllComments(int pageIndex, int pageSize, bool? active = true, string search = "");

        /// <summary>
        /// Get all the comments for a project
        /// </summary>
        /// <param name="projectId">Project id</param>
        /// <returns>Collection of comments</returns>
        IList<Comment> GetCommentsByProjectId(int projectId);

        /// <summary>
        /// Get a comment by id
        /// </summary>
        /// <param name="id">Id of the comment to retrieve</param>
        /// <returns>Comment</returns>
        Comment GetCommentById(int id);

        /// <summary>
        /// Insert a project
        /// </summary>
        /// <param name="comment">Comment</param>
        void InsertComment(Comment comment);

        /// <summary>
        /// Change the owner of a comment
        /// </summary>
        /// <param name="currentUserId">Current user id</param>
        /// <param name="newUser">New user</param>
        void MigrateCommentOwnership(int currentUserId, User newUser);

        /// <summary>
        /// Updates the project
        /// </summary>
        /// <param name="comment">Comment</param>
        void UpdateComment(Comment comment);

    }
}