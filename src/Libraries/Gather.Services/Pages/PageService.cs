using System;
using System.Linq;
using Gather.Core.Domain.Pages;
using Gather.Core.Data;
using Gather.Core;

namespace Gather.Services.Pages
{
    public class PageService : IPageService
    {

        #region Fields

        private readonly IRepository<Page> _pageRepository;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors

        public PageService(IRepository<Page> pageRepository, IWorkContext workContext)
        {
            _pageRepository = pageRepository;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a page
        /// </summary>
        /// <param name="page">Page</param>
        public void DeletePage(Page page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            if (page.Deleted)
                return;

            _pageRepository.Delete(page);
        }

        /// <summary>
        /// Get all pages
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="active">Active flag</param>
        /// <param name="search">search </param>
        /// <returns>Paginated page collection</returns>
        public IPaginatedList<Page> GetAllPages(int pageIndex, int pageSize, bool? active = true, string search = "")
        {
            var query = _pageRepository.Table;

            query = query.Where(s => !s.Deleted);

            if (active != null)
                query = query.Where(s => s.Active == active);

            if (search != null)
                query = query.Where(s => s.Title.Contains(search));

            query = query.OrderBy(s => s.Title);

            var pages = new PaginatedList<Page>(query, pageIndex, pageSize);
            return pages;
        }

        /// <summary>
        /// Get a page by id
        /// </summary>
        /// <param name="pageId">Id of page to retrieve</param>
        /// <returns>Page</returns>
        public Page GetPageById(int pageId)
        {
            if (pageId == 0)
                return null;

            var page = _pageRepository.GetById(pageId);
            return page;
        }

        /// <summary>
        /// Insert a page
        /// </summary>
        /// <param name="page">page</param>
        public void InsertPage(Page page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            page.Active = true;
            page.CreatedDate = DateTime.Now;
            page.Deleted = false;
            page.LastModifiedDate = DateTime.Now;

            _pageRepository.Insert(page);
        }

        /// <summary>
        /// Updates the page
        /// </summary>
        /// <param name="page">page</param>
        public void UpdatePage(Page page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            page.LastModifiedBy = _workContext.CurrentUser.Id;
            page.LastModifiedDate = DateTime.Now;

            _pageRepository.Update(page);
        }

        #endregion

    }
}
