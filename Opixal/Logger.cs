using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks;

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

    public enum LogLevel
    {
        //
        // Summary:
        //     Logs that contain the most detailed messages. These messages may contain sensitive
        //     application data. These messages are disabled by default and should never be
        //     enabled in a production environment.
        Trace = 0,

        //
        // Summary:
        //     Logs that are used for interactive investigation during development. These logs
        //     should primarily contain information useful for debugging and have no long-term
        //     value.
        Debug = 1,

        //
        // Summary:
        //     Logs that track the general flow of the application. These logs should have long-term
        //     value.
        Information = 2,

        //
        // Summary:
        //     Logs that highlight an abnormal or unexpected event in the application flow,
        //     but do not otherwise cause the application execution to stop.
        Warning = 3,

        //
        // Summary:
        //     Logs that highlight when the current flow of execution is stopped due to a failure.
        //     These should indicate a failure in the current activity, not an application-wide
        //     failure.
        Error = 4,

        //
        // Summary:
        //     Logs that describe an unrecoverable application or system crash, or a catastrophic
        //     failure that requires immediate attention.
        Critical = 5,

        //
        // Summary:
        //     Not used for writing log messages. Specifies that a logging category should not
        //     write any messages.
        None = 6
    }

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

    public class Logger : ILogger
    {
        private static readonly Lazy<Logger> lazy = new Lazy<Logger>(() => new Logger());
        public static Logger Instance { get { return lazy.Value; } }

        private readonly BlockingCollection<Param> _Params = new BlockingCollection<Param>();

        private Logger()
        {
            Task.Factory.StartNew(() =>
            {
                foreach (Param p in _Params.GetConsumingEnumerable())
                {
                    switch (p.LogLevel)
                    {
                        case LogLevel.Trace:
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;

                        case LogLevel.Debug:
                            Console.ForegroundColor = ConsoleColor.Cyan;

                            break;

                        case LogLevel.Information:
                            Console.ForegroundColor = ConsoleColor.White;

                            break;

                        case LogLevel.Warning:
                            Console.ForegroundColor = ConsoleColor.Yellow;

                            break;

                        case LogLevel.Error:
                            Console.ForegroundColor = ConsoleColor.Magenta;

                            break;

                        case LogLevel.Critical:
                            Console.ForegroundColor = ConsoleColor.Red;

                            break;

                        case LogLevel.None:
                            Console.ForegroundColor = ConsoleColor.Gray;

                            break;

                        default:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                    }

                    Console.WriteLine(JsonSerializer.Serialize(p, new JsonSerializerOptions { IgnoreNullValues = true }));
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            });
        }

        ~Logger()
        {
            _Params.CompleteAdding();
        }

        // TIMESTAMP | LOGLEVEL | LOGOBJECT | LOGACTION | LOGEXCEPTION | LOGMESSAGE
        private string TimeStamp
        {
            get
            {
                DateTime current = DateTime.UtcNow;
                return current.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        public void Log(LogLevel logLevel, string logMessage)
        {
            Param param = new Param(TimeStamp, logLevel, null, null, null, logMessage);
            _Params.Add(param);
        }

        public void Log(LogLevel logLevel, Exception logException)
        {
            Param param = new Param(TimeStamp, logLevel, null, null, logException, null);
            _Params.Add(param);
        }

        public void Log<T>(LogLevel logLevel, T logObject, string logMessage)
        {
            Param param = new Param(TimeStamp, logLevel, logObject, null, null, logMessage);
            _Params.Add(param);
        }

        public void Log<T>(LogLevel logLevel, T logObject, Exception logException)
        {
            Param param = new Param(TimeStamp, logLevel, logObject, null, logException, null);
            _Params.Add(param);
        }

        public void Log<T>(LogLevel logLevel, T logObject, string logAction, string logMessage)
        {
            Param param = new Param(TimeStamp, logLevel, logObject, logAction, null, logMessage);
            _Params.Add(param);
        }

        public void Log<T>(LogLevel logLevel, T logObject, string logAction, Exception logException)
        {
            Param param = new Param(TimeStamp, logLevel, logObject, logAction, logException, null);
            _Params.Add(param);
        }

        public void Log<T>(LogLevel logLevel, T logObject, string logAction = null, Exception logException = null, string logMessage = null)
        {
            Param param = new Param(TimeStamp, logLevel, logObject, logAction, logException, logMessage);
            _Params.Add(param);
        }
    }
}