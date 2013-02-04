using Gather.Core;
using Gather.Core.Domain.Pages;

namespace Gather.Services.Pages
{
    public interface IPageService
    {

        /// <summary>
        /// Delete a page
        /// </summary>
        /// <param name="page">Page</param>
        void DeletePage(Page page);

        /// <summary>
        /// Get all page
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="active">Active flag</param>
        /// <param name="search">Search term</param>
        /// <returns>Paginated page collection</returns>
        IPaginatedList<Page> GetAllPages(int pageIndex, int pageSize, bool? active = true, string search = "");

        /// <summary>
        /// Get a page by id
        /// </summary>
        /// <param name="pageId">Id of page to retrieve</param>
        /// <returns>Page</returns>
        Page GetPageById(int pageId);

        /// <summary>
        /// Inserts a page
        /// </summary>
        /// <param name="page"></param>
        void InsertPage(Page page);

        /// <summary>
        /// Updates the page
        /// </summary>
        /// <param name="page"></param>
        void UpdatePage(Page page);

    }
}
