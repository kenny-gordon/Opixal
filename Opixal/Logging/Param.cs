using System;

namespace Opixal.Logging
{
    internal class Param
    {
        public string LogTimeStamp { get; set; }
        public LogLevel LogLevel { get; set; }
        public object LogObject { get; set; }
        public string LogAction { get; set; }
        public Exception LogException { get; set; }
        public string LogMessage { get; set; }

        internal Param()
        {
            LogLevel = LogLevel.None;
            LogMessage = "";
        }

        public Param(string logTimeStamp, LogLevel logLevel, object logObject, string logAction, Exception logException, string logMessage)
        {
            LogTimeStamp = logTimeStamp;
            LogLevel = logLevel;
            LogObject = logObject;
            LogAction = logAction;
            LogException = logException;
            LogMessage = logMessage;
        }
    }
}