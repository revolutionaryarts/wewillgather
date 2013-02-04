using System.Collections.Generic;
using Gather.Core;
using Gather.Core.Domain.Categories;

namespace Gather.Services.Categories
{
    public interface ICategoryService
    {

        /// <summary>
        /// Clear cache
        /// </summary>
        void ClearCache();

        /// <summary>
        /// Delete a category
        /// </summary>
        /// <param name="category">Category</param>
        void DeleteCategory(Category category);

        /// <summary>
        /// Get all cached categories
        /// </summary>
        /// <returns>Collection of categories</returns>
        IList<Category> GetAllCachedCategories();

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="active">Active flag</param>
        /// <param name="search">Search term</param>
        /// <returns>Paginated category collection</returns>
        IPaginatedList<Category> GetAllCategories(int pageIndex = 0, int pageSize = -1, bool? active = true, string search = "");

        /// <summary>
        /// Get a category by id
        /// </summary>
        /// <param name="categoryId">Id of category to retrieve</param>
        /// <returns>Category</returns>
        Category GetCategoryById(int categoryId);

        /// <summary>
        /// Inserts a category
        /// </summary>
        /// <param name="category"></param>
        void InsertCategory(Category category);

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="category"></param>
        void UpdateCategory(Category category);

    }
}