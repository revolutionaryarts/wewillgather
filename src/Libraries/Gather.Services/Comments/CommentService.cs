using System;
using System.Collections.Generic;
using System.Linq;
using Gather.Core;
using Gather.Core.Data;
using Gather.Core.Domain.Comments;
using Gather.Core.Domain.Users;

namespace Gather.Services.Comments
{
    public class CommentService : ICommentService
    {

        #region Fields

        private readonly IRepository<Comment> _commentRepository;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors

        public CommentService(IRepository<Comment> commentRepository, IWorkContext workContext)
        {
            _commentRepository = commentRepository;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a comment
        /// </summary>
        /// <param name="comment">Comment</param>
        /// /// <param name="persist">Persist delete</param>
        public void DeleteComment(Comment comment, bool persist = false)
        {
            if (comment == null)
                throw new ArgumentNullException("comment");

            if (!persist && comment.Deleted)
                return;

            _commentRepository.Delete(comment, persist);
        }

        /// <summary>
        /// Delete all comments by a given author
        /// </summary>
        /// <param name="userId">User id</param>
        public void DeleteCommentsByAuthor(int userId)
        {
            if (userId == 0)
                return;

            var comments = _commentRepository.Table.Where(c => c.CreatedBy == userId).ToList();

            foreach (var comment in comments)
                DeleteComment(comment, true);
        }

        /// <summary>
        /// Get all comments
        /// </summary>
        /// <param name="pageIndex">Current page</param>
        /// <param name="pageSize">Items per page</param>
        /// <param name="active">Active flag</param>
        /// <param name="search">Search query</param>
        /// <returns>Paginated collection of comments</returns>
        public IPaginatedList<Comment> GetAllComments(int pageIndex, int pageSize, bool? active = true, string search = "")
        {
            var query = _commentRepository.Table;

            query = query.Where(s => !s.Deleted);

            if (active != null)
                query = query.Where(s => s.Active == active);

            if (search != null)
                query = query.Where(s => s.UserComment.Contains(search));

            query = query.OrderBy(s => s.Id);

            var comments = new PaginatedList<Comment>(query, pageIndex, pageSize);
            return comments;
        }

        /// <summary>
        /// Get all the comments for a project
        /// </summary>
        /// <param name="projectId">Project id</param>
        /// <returns>Collection of comments</returns>
        public IList<Comment> GetCommentsByProjectId(int projectId)
        {
            if (projectId == 0)
                return null;

            var query = _commentRepository.Table;

            query = query.Where(x => x.Project.Id == projectId);
            query = query.Where(x => x.Active && !x.Deleted);
            query = query.Where(x => x.ModerationRequestCount < 10);

            return query.ToList();
        }

        /// <summary>
        /// Get a comment by id
        /// </summary>
        /// <param name="id">Id of the comment to retrieve</param>
        /// <returns>Comment</returns>
        public Comment GetCommentById(int id)
        {
            if (id == 0)
                return null;

            var comment = _commentRepository.GetById(id);
            return comment;
        }

        /// <summary>
        /// Insert a project
        /// </summary>
        /// <param name="comment">Comment</param>
        public void InsertComment(Comment comment)
        {
            if (comment == null)
                throw new ArgumentNullException("comment");

            comment.Active = true;
            comment.Author = _workContext.CurrentUser;
            comment.CreatedBy = _workContext.CurrentUser.Id;
            comment.CreatedDate = DateTime.Now;

            _commentRepository.Insert(comment);
        }

        /// <summary>
        /// Change the owner of a comment
        /// </summary>
        /// <param name="currentUserId">Current user id</param>
        /// <param name="newUser">New user</param>
        public void MigrateCommentOwnership(int currentUserId, User newUser)
        {
            if (currentUserId == 0 || newUser == null)
                return;

            var comments = _commentRepository.Table.Where(x => x.CreatedBy == currentUserId).ToList();

            foreach (var comment in comments)
            {
                comment.Author = newUser;
                comment.CreatedBy = newUser.Id;
            }

            _commentRepository.BulkUpdate(comments);
        }

        /// <summary>
        /// Updates the project
        /// </summary>
        /// <param name="comment">Comment</param>
        public void UpdateComment(Comment comment)
        {
            if (comment == null)
                throw new ArgumentNullException("comment");

            comment.LastModifiedBy = _workContext.CurrentUser.Id;
            comment.LastModifiedDate = DateTime.Now;

            _commentRepository.Update(comment);
        }

        #endregion

    }
}