
using Gather.Core.Domain.Slug;

namespace Gather.Services.Slugs
{
    public interface ISlugService
    {

        /// <summary>
        /// Insert a slug
        /// </summary>
        /// <param name="slug">Slug</param>
        void InsertSlug(Slug slug);

        /// <summary>
        /// Delete a slug
        /// </summary>
        /// <param name="slug">Slug</param>
        void DeleteSlug(Slug slug);

        /// <summary>
        /// Update slug, remove any old instances.
        /// </summary>
        /// <param name="successStoryId"> </param>
        /// <param name="slugUrl">Slug</param>
        void UpdateSlug(int successStoryId, string slugUrl);

        /// <summary>
        /// Remove slugs for success story
        /// </summary>
        /// <param name="successStoryId">the success story Id</param>
        void DeleteSlugsBySuccessStoryId(int successStoryId);

        /// <summary>
        /// Method for looking up dynamic pages.
        /// </summary>
        /// <param name="seName"></param>
        /// <returns>Slug</returns>
        Slug SlugLookup(string seName);

    }
}
