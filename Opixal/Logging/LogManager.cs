using Opixal.Logging.Provider;
using System;
using System.Collections.Concurrent;
using System.Threading;

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
            logger.Log(new LogEntry(LoggingEventType.INFO, message));
        }

        public static void LogInfo<T>(this ILogger logger, string message, T type)
        {
            logger.Log(new LogEntry<T>(LoggingEventType.INFO, message, type));
        }

        public static void LogWarn(this ILogger logger, string message)
        {
            logger.Log(new LogEntry(LoggingEventType.WARN, message));
        }

        public static void LogWarn<T>(this ILogger logger, string message, T type)
        {
            logger.Log(new LogEntry<T>(LoggingEventType.WARN, message, type));
        }

        #endregion Methods
    }

    public class LogManager : ILogger, IDisposable
    {
        #region Fields

        public ConsoleLogger ConsoleLogger;
        public FileLogger FileLogger;

        private readonly Thread _consumerThread;
        private readonly BlockingCollection<ILogEntry> _logEntriesQueue = new BlockingCollection<ILogEntry>();

        #endregion Fields

        #region Constructors

        public LogManager()
        {
            _consumerThread = new Thread(new ThreadStart(OnStart))
            {
                Name = "Logger",
                IsBackground = true
            };
            _consumerThread.Start();
        }

        #endregion Constructors

        #region Methods

        public void Dispose()
        {
            try
            {
                _logEntriesQueue.CompleteAdding();
                _consumerThread.Join(TimeSpan.FromSeconds(5));
            }
            catch (ThreadStateException)
            {
                throw;
            }
        }

        public void Log(LogEntry entry)
        {
            _logEntriesQueue.Add(entry);
        }

        public void Log<TState>(LogEntry<TState> entry)
        {
            _logEntriesQueue.Add(entry);
        }

        private void OnStart()
        {
            try
            {
                foreach (var logEntry in _logEntriesQueue.GetConsumingEnumerable())
                {
                    try
                    {
                        // do something to process log entries.

                        if (this.ConsoleLogger != null)
                        {
                            ConsoleLogger.Write(logEntry);
                        }

                        if (this.FileLogger != null)
                        {
                            FileLogger.Write(logEntry);
                        }
                    }
                    catch (Exception ex)
                    {
                        // do something, otherwise one failure to write a log
                        // will bring down your whole logging
                        Console.WriteLine(ex);
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                // do something, don't let exceptions go unnoticed
                Console.WriteLine(ex);
                throw;
            }
        }

        #endregion Methods
    }
}
