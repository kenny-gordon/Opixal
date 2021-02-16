using System;

namespace Opixal.Logging.Provider
{
    public abstract class LoggerProviderBase : ILoggerProvider
    {
        #region Properties

        public bool EnableJSON { get; set; } = false;
        public LoggingEventType LoggingLevel { get; set; } = LoggingEventType.DEBUG;

        protected static string TimeStamp { get => DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"); }

        #endregion Properties

        #region Fields

        protected readonly object lockObj = new object();

        #endregion Fields

        #region Methods

        public abstract void Write(ILogEntry log);

        #endregion Methods
    }
}