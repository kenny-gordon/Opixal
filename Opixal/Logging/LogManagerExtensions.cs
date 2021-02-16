using System;

namespace Opixal.Logging
{
    public static class LogManagerExtensions
    {
        #region Methods

        public static void LogDebug(this ILogger logger, string message)
        {
            logger.Log(new LogEntry(LoggingEventType.DEBUG, message));
        }

        public static void LogDebug<T>(this ILogger logger, string message, T type)
        {
            logger.Log(new LogEntry<T>(LoggingEventType.DEBUG, message, type));
        }

        public static void LogError(this ILogger logger, Exception exception)
        {
            logger.Log(new LogEntry(LoggingEventType.ERROR, exception.Message, exception));
        }

        public static void LogError<T>(this ILogger logger, T type, Exception exception)
        {
            logger.Log(new LogEntry<T>(LoggingEventType.ERROR, exception.Message, type, exception));
        }

        public static void LogFatal(this ILogger logger, Exception exception)
        {
            logger.Log(new LogEntry(LoggingEventType.FATAL, exception.Message, exception));
        }

        public static void LogFatal<T>(this ILogger logger, T type, Exception exception)
        {
            logger.Log(new LogEntry<T>(LoggingEventType.FATAL, exception.Message, type, exception));
        }

        public static void LogInfo(this ILogger logger, string message)
        {
            logger.Log(new LogEntry(LoggingEventType.INFORMATION, message));
        }

        public static void LogInfo<T>(this ILogger logger, string message, T type)
        {
            logger.Log(new LogEntry<T>(LoggingEventType.INFORMATION, message, type));
        }

        public static void LogWarn(this ILogger logger, string message)
        {
            logger.Log(new LogEntry(LoggingEventType.WARNING, message));
        }

        public static void LogWarn<T>(this ILogger logger, string message, T type)
        {
            logger.Log(new LogEntry<T>(LoggingEventType.WARNING, message, type));
        }

        #endregion Methods
    }
}