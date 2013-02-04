using System;
using Gather.Core.Domain.Logging;
using Gather.Core.Domain.Users;

namespace Gather.Services.Logging
{
    public static class LoggingExtensions
    {
        public static void Debug(this ILogService logger, string message, Exception exception = null, User user = null)
        {
            FilteredLog(logger, LogLevel.Debug, message, exception, user);
        }

        public static void Information(this ILogService logger, string message, Exception exception = null, User user = null)
        {
            FilteredLog(logger, LogLevel.Information, message, exception, user);
        }

        public static void Warning(this ILogService logger, string message, Exception exception = null, User user = null)
        {
            FilteredLog(logger, LogLevel.Warning, message, exception, user);
        }

        public static void Error(this ILogService logger, string message, Exception exception = null, User user = null)
        {
            FilteredLog(logger, LogLevel.Error, message, exception, user);
        }

        public static void Fatal(this ILogService logger, string message, Exception exception = null, User user = null)
        {
            FilteredLog(logger, LogLevel.Fatal, message, exception, user);
        }

        private static void FilteredLog(ILogService logger, LogLevel level, string message, Exception exception = null, User user = null)
        {
            // Don't bother logging thread abort exception
            if ((exception != null) && (exception is System.Threading.ThreadAbortException))
                return;

            string fullMessage = exception == null ? string.Empty : exception.ToString();
            logger.InsertLog(level, message, fullMessage, user);
        }
    }
}