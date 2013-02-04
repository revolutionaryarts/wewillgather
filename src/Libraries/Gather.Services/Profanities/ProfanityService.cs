using System;
using System.Linq;
using System.Collections.Generic;
using Gather.Core.Cache;
using Gather.Core.Data;
using Gather.Core.Domain.Profanity;

namespace Gather.Services.Profanities
{
    public class ProfanityService : IProfanityService
    {

        #region Constants

        private const string PROFANITY_ALL_KEY = "Gather.profanity.all";

        #endregion

        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IRepository<Profanity> _profanityRepository;    

        #endregion

        #region Constructor

        public ProfanityService(ICacheManager cacheManager, IRepository<Profanity> profanityRepository)
        {
            _cacheManager = cacheManager;
            _profanityRepository = profanityRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns all profanities
        /// </summary>
        public IList<String> GetAll()
        {
            return _cacheManager.Get(PROFANITY_ALL_KEY, () =>
            {
                var query = _profanityRepository.Table;
                return query.Select(x => x.Word).ToList();
            });
        }

        #endregion

    }
}