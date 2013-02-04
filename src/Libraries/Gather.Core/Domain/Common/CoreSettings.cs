using Gather.Core.Configuration;

namespace Gather.Core.Domain.Common
{
    public class CoreSettings : ISettings
    {
        /// <summary>
        /// The number of results per page to display in the admin modules
        /// </summary>
        public int AdminGridPageSize { get; set; }

        /// <summary>
        /// The domain on the installed site
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Secret key generated to helpsecure the Facebook communication
        /// </summary>
        public string FacebookStateSecret { get; set; }

        /// <summary>
        /// The Id of the last Tweet retrieved from the monitor service
        /// </summary>
        public string LastTweetId { get; set; }

        /// <summary>
        /// Secret key generated to helpsecure the Twitter communication
        /// </summary>
        public string TwitterStateSecret { get; set; }
    }
}