using System.Collections.Generic;
using Gather.Core;
using Gather.Core.Domain.SuccessStories;
using Gather.Core.Domain.Users;

namespace Gather.Services.SuccessStories
{
    public interface ISuccessStoryService
    {

        /// <summary>
        /// Delete a story
        /// </summary>
        /// <param name="success">SuccessStory</param>
        void DeleteSuccessStory(SuccessStory success);

        /// <summary>
        /// Get all success stories
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="active">Active flag</param>
        /// <param name="search">Search term</param>
        /// <returns>Paginated success story collection</returns>
        IPaginatedList<SuccessStory> GetAllSuccessStories(int pageIndex = 0, int pageSize = -1, bool? active = true, string search = "");

        /// <summary>
        /// Get a success story by id
        /// </summary>
        /// <param name="successStoryId">Id of success story to retrieve</param>
        /// <returns>SuccessStory</returns>
        SuccessStory GetSuccessStoryById(int successStoryId);

        /// <summary>
        /// Inserts a success story
        /// </summary>
        /// <param name="success"></param>
        void InsertSuccessStory(SuccessStory success);

        /// <summary>
        /// Change the author of success stories
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="newUser"></param>
        void MigrateSuccessStoryAuthor(int currentUserId, User newUser);

        /// <summary>
        /// Unassociates all success stories from a user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="newUser">User to replace with</param>
        void UnassociateSuccessStoryUser(int userId, User newUser = null);

        /// <summary>
        /// Updates the success story
        /// </summary>
        /// <param name="success"></param>
        void UpdateSuccessStory(SuccessStory success);

    }
}