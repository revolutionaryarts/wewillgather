using Gather.Core.Domain.Logging;
using Gather.Core.Domain.Users;

namespace Gather.Services.Logging
{
    public interface ILogService
    {
        /// <summary>
        /// Inserts a log item
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="shortMessage">The short message</param>
        /// <param name="fullMessage">The full message</param>
        /// <param name="user">The user to associate log record with</param>
        /// <returns>A log item</returns>
        Log InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "", User user = null);
    }
}