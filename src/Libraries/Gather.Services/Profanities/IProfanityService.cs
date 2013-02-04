using System.Collections.Generic;

namespace Gather.Services.Profanities
{
    public interface IProfanityService
    {
        /// <summary>
        /// Get all profanity filters
        /// </summary>
        IList<string> GetAll();
    }
}
