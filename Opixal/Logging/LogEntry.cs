using System;

namespace Opixal.Logging
{
    public class LogEntry : LogEntryBase
    {
        #region Constructors

        public LogEntry(LoggingEventType severity, string message, Exception exception = null)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (message == string.Empty)
            {
                throw new ArgumentException("empty", "message");
            }

            this.Severity = severity;
            this.Message = message;
            this.Exception = exception;
        }

        #endregion Constructors
    }

    public class LogEntry<T> : LogEntry
    {
        #region Constructors

        public LogEntry(LoggingEventType severity, string message, T type, Exception exception = null) : base(severity, message, exception)
        {
            this.Type = type;
        }

        #endregion Constructors
    }

    public abstract class LogEntryBase : ILogEntry
    {
        #region Properties

        public LoggingEventType Severity { get; protected set; }
        public string Message { get; protected set; }
        public Object Type { get; protected set; }
        public Exception Exception { get; protected set; }

        #endregion Properties
    }
}