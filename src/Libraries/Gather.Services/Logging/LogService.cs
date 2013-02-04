using System;
using Gather.Core;
using Gather.Core.Data;
using Gather.Core.Domain.Logging;
using Gather.Core.Domain.Users;

namespace Gather.Services.Logging
{
    public class LogService : ILogService
    {

        #region Fields

        private readonly IRepository<Log> _logRepository;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Constructors

        public LogService(IRepository<Log> logRepository, IWebHelper webHelper)
        {
            _logRepository = logRepository;
            _webHelper = webHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts a log item
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="shortMessage">The short message</param>
        /// <param name="fullMessage">The full message</param>
        /// <param name="user">The user to associate log record with</param>
        /// <returns>A log item</returns>
        public Log InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "", User user = null)
        {
            var log = new Log
            {
                LogLevel = logLevel,
                ShortMessage = shortMessage,
                FullMessage = fullMessage,
                IpAddress = _webHelper.GetCurrentIpAddress(),
                User = user,
                PageUrl = _webHelper.GetThisPageUrl(true),
                ReferrerUrl = _webHelper.GetUrlReferrer(),
                CreatedOnUtc = DateTime.UtcNow
            };

            _logRepository.Insert(log);

            return log;
        }

        #endregion

    }
}