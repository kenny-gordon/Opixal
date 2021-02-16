using Opixal.Logging.Provider;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Opixal.Logging
{
    public class LogManager : ILogger, IDisposable
    {
        #region Fields

        public ConsoleLoggerProvider ConsoleLogger;
        public FileLoggerProvider FileLogger;

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
                this.LogInfo($"{this} has been disposed.");
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