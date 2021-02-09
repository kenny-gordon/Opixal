using System;

namespace Opixal.Logging
{
    public interface ILogger
    {
        void Log(LogLevel logLevel, string logMessage);

        void Log<TClass>(LogLevel logLevel, TClass logObject, string logMessage);

        void Log<TClass>(LogLevel logLevel, TClass logObject, Exception logException);

        void Log<TClass>(LogLevel logLevel, TClass logObject, string logAction, Exception logException);

        void Log<TClass>(LogLevel logLevel, TClass logObject, string logAction, string logMessage);
    }
}