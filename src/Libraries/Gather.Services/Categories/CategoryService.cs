using System;
using System.Collections.Generic;
using System.Linq;
using Gather.Core;
using Gather.Core.Cache;
using Gather.Core.Data;
using Gather.Core.Domain.Categories;

namespace Gather.Services.Categories
{
    public class CategoryService : ICategoryService
    {

        #region Constants

        private const string CATEGORIES_ALL_KEY = "Gather.categories.all";

        #endregion

        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IRepository<Category> _categoryRepository;

        #endregion

        #region Constructors

        public CategoryService(ICacheManager cacheManager, IRepository<Category> categoryRepository)
        {
            _cacheManager = cacheManager;
            _categoryRepository = categoryRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clear cache
        /// </summary>
        public void ClearCache()
        {
            _cacheManager.RemoveByPattern(CATEGORIES_ALL_KEY);
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        /// <param name="category">Category</param>
        public void DeleteCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            if (category.Deleted)
                return;

            _categoryRepository.Delete(category);

            ClearCache();
        }

        /// <summary>
        /// Get all cached categories
        /// </summary>
        /// <returns>Collection of categories</returns>
        public IList<Category> GetAllCachedCategories()
        {
            return _cacheManager.Get(CATEGORIES_ALL_KEY, () =>
            {
                var query = from c in _categoryRepository.Table
                            where c.Active && !c.Deleted
                            select c;
                return query.ToList();
            });
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="active">Active flag</param>
        /// <param name="search">Search </param>
        /// <returns>Paginated category collection</returns>
        public IPaginatedList<Category> GetAllCategories(int pageIndex = 0, int pageSize = -1, bool? active = true, string search = "")
        {
            var query = _categoryRepository.Table;

            query = query.Where(u => !u.Deleted);

            if (active != null)
                query = query.Where(u => u.Active == active);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(u => u.Name.Contains(search));

            query = query.OrderBy(u => u.Name);

            var categories = new PaginatedList<Category>(query, pageIndex, pageSize);
            return categories;
        }

        /// <summary>
        /// Get a category by id
        /// </summary>
        /// <param name="categoryId">Id of category to retrieve</param>
        /// <returns>Category</returns>
        public Category GetCategoryById(int categoryId)
        {
            if (categoryId == 0)
                return null;

            var category = _categoryRepository.GetById(categoryId);
            return category;
        }

        /// <summary>
        /// Insert a category
        /// </summary>
        /// <param name="category">category</param>
        public void InsertCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            category.Active = true;
            category.CreatedDate = DateTime.Now;
            category.Deleted = false;
            category.LastModifiedDate = DateTime.Now;

            _categoryRepository.Insert(category);

            ClearCache();
        }

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="category">Category</param>
        public void UpdateCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            category.LastModifiedDate = DateTime.Now;

            _categoryRepository.Update(category);

            ClearCache();
        }

        #endregion

    }
}