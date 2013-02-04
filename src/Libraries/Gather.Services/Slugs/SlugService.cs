using System;
using System.Linq;
using Gather.Core.Data;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Slug;

namespace Gather.Services.Slugs
{
    public class SlugService : ISlugService
    {

        #region Fields
        
        private readonly IRepository<Slug> _slugRepository;    

        #endregion

        #region Constructor

        public SlugService(IRepository<Slug> slugRepository)
        {
            _slugRepository = slugRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Insert a slug
        /// </summary>
        /// <param name="slug">Slug</param>
        public void InsertSlug(Slug slug)
        {
            if (slug == null)
                throw new ArgumentNullException("slug");

            _slugRepository.Insert(slug);

        }

        /// <summary>
        /// Delete a slug
        /// </summary>
        /// <param name="slug">Slug</param>
        public void DeleteSlug(Slug slug)
        {
            if (slug == null)
                throw new ArgumentNullException("slug");

            _slugRepository.Delete(slug, true);

        }

        /// <summary>
        /// Update slug, remove any old instances.
        /// </summary>
        /// <param name="successStoryId"> </param>
        /// <param name="slugUrl">Slug</param>
        public void UpdateSlug(int successStoryId, string slugUrl)
        {
            if (slugUrl == null)
                throw new ArgumentNullException("slugUrl");

            var query = from p in _slugRepository.Table
                        where (p.SlugUrl == slugUrl)
                        select p;
            var slugs = query.ToList();

            InsertSlug(new Slug { SlugUrl = slugUrl, SuccessStoryId = successStoryId });

            foreach (var slug in slugs)
            {
                DeleteSlug(slug);
            }

        }

        /// <summary>
        /// Remove slugs for success story
        /// </summary>
        /// <param name="successStoryId">the success story Id</param>
        public void DeleteSlugsBySuccessStoryId(int successStoryId)
        {
            var query = from p in _slugRepository.Table
                        where (p.SuccessStoryId == successStoryId) 
                        select p;
            var slugs = query.ToList();

            foreach (var slug in slugs)
            {
                DeleteSlug(slug);
            }            
        }

        /// <summary>
        /// Method for looking up dynamic pages.
        /// </summary>
        /// <param name="seName"></param>
        /// <returns>Slug</returns>
        public Slug SlugLookup(string seName)
        {     

            var query = from p in _slugRepository.Table
                        orderby p.Id descending 
                        where (p.SlugUrl == seName)
                        select p;
            var slug = query.FirstOrDefault();            

            // 404 the request if nothing found
            if (slug == null)
                return null;

            slug.LookupType = SlugLookupType.SlugFound;

            // Check we don't have any more recent urls for this item
            var redirectQuery = from p in _slugRepository.Table
                        orderby p.Id descending 
                        where (p.SuccessStoryId == slug.SuccessStoryId)
                        where (p.Id > slug.Id)
                        select p;
            var redirectSlug = redirectQuery.FirstOrDefault();

            if (redirectSlug == null)
                return slug;

            redirectSlug.LookupType = SlugLookupType.Slug301Found;
            return redirectSlug;
        }

        #endregion
    }
}
