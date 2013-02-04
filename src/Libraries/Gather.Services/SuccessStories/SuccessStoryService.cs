using System;
using System.Linq;
using Gather.Core.Domain.SuccessStories;
using Gather.Core.Data;
using Gather.Core;
using Gather.Core.Domain.Users;
using Gather.Services.Users;

namespace Gather.Services.SuccessStories
{
    public class SuccessStoryService : ISuccessStoryService
    {

        #region Fields

        private readonly IRepository<SuccessStory> _successStoryRepository;
        private readonly IUserService _userService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors

        public SuccessStoryService(IRepository<SuccessStory> successStoryRepository, IUserService userService, IWorkContext workContext)
        {
            _successStoryRepository = successStoryRepository;
            _userService = userService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a story
        /// </summary>
        /// <param name="success">SuccessStory</param>
        public void DeleteSuccessStory(SuccessStory success)
        {
            if (success == null)
                throw new ArgumentNullException("success");

            if (success.Deleted)
                return;

            _successStoryRepository.Delete(success);
        }

        /// <summary>
        /// Get all success stories
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="active">Active flag</param>
        /// <param name="search">Search </param>
        /// <returns>Paginated success story collection</returns>
        public IPaginatedList<SuccessStory> GetAllSuccessStories(int pageIndex = 0, int pageSize = -1, bool? active = true, string search = "")
        {
            var query = _successStoryRepository.Table;

            query = query.Where(s => !s.Deleted);

            if (active != null)
                query = query.Where(s => s.Active == active);

            if (search != null)
                query = query.Where(s => s.Title.Contains(search) || s.ShortSummary.Contains(search) || s.Author.DisplayName.Contains(search));

            query = query.OrderByDescending(s => s.Id);

            var stories = new PaginatedList<SuccessStory>(query, pageIndex, pageSize);
            return stories;
        }

        /// <summary>
        /// Get a category by id
        /// </summary>
        /// <param name="successStoryId">Id of success story to retrieve</param>
        /// <returns>User</returns>
        public SuccessStory GetSuccessStoryById(int successStoryId)
        {
            if (successStoryId == 0)
                return null;

            var success = _successStoryRepository.GetById(successStoryId);
            return success;
        }

        /// <summary>
        /// Insert a category
        /// </summary>
        /// <param name="success">SuccessStory</param>
        public void InsertSuccessStory(SuccessStory success)
        {
            if (success == null)
                throw new ArgumentNullException("success");

            success.CreatedBy = _workContext.CurrentUser.Id;
            success.CreatedDate = DateTime.Now;
            success.Deleted = false;
            success.LastModifiedBy = _workContext.CurrentUser.Id;
            success.LastModifiedDate = DateTime.Now;

            _successStoryRepository.Insert(success);
        }

        /// <summary>
        /// Change the author of success stories
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="newUser"></param>
        public void MigrateSuccessStoryAuthor(int currentUserId, User newUser)
        {
            if (currentUserId == 0 || newUser == null)
                return;

            var stories = _successStoryRepository.Table.Where(x => x.CreatedBy == currentUserId || x.Author.Id == currentUserId).ToList();

            foreach (var story in stories)
            {
                if (story.Author.Id == currentUserId)
                    story.Author = newUser;

                if (story.CreatedBy == currentUserId)
                    story.CreatedBy = newUser.Id;
            }

            _successStoryRepository.BulkUpdate(stories);
        }

        /// <summary>
        /// Unassociates all success stories from a user
        /// </summary>
        /// <param name="userId">User id</param>
        /// /// <param name="newUser">User to replace with</param>
        public void UnassociateSuccessStoryUser(int userId, User newUser = null)
        {
            if (userId == 0)
                return;

            if (newUser == null)
                newUser = _userService.GetSiteOwner();

            var stories = _successStoryRepository.Table.Where(s => s.Author.Id == userId || s.CreatedBy == userId).ToList();

            foreach (var story in stories)
            {
                if (story.CreatedBy == userId)
                    story.CreatedBy = newUser.Id;

                if (story.Author.Id == userId)
                    story.Author = newUser;
            }

            _successStoryRepository.BulkUpdate(stories);
        }

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="success">SuccessStory</param>
        public void UpdateSuccessStory(SuccessStory success)
        {
            if (success == null)
                throw new ArgumentNullException("success");

            success.LastModifiedBy = _workContext.CurrentUser.Id;
            success.LastModifiedDate = DateTime.Now;

            _successStoryRepository.Update(success);
        }

        #endregion

    }
}