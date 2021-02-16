using System;

namespace Opixal.Logging
{
    public interface ILogEntry
    {
        #region Properties

        LoggingEventType Severity { get; }
        string Message { get; }
        Object Type { get; }
        Exception Exception { get; }

        #endregion Properties
    }
}