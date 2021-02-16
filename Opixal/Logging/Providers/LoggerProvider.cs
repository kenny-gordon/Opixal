using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;

namespace Opixal.Logging.Provider
{
    public interface ILoggerProvider
    {
        #region Properties

        bool EnableJSON { get; set; }
        LoggingEventType LoggingLevel { get; set; }

        #endregion Properties

        #region Methods

        void Write(ILogEntry log);

        #endregion Methods
    }

    public abstract class LoggerProvider : ILoggerProvider
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

    public class ConsoleLogger : LoggerProvider
    {
        #region Methods

        public override void Write(ILogEntry log)
        {
            lock (lockObj)
            {
                string logEntryString = $"{TimeStamp} [{log.Severity}] {log.Message} {log.Type} {log.Exception}";

                if (log.Severity >= (LoggingEventType)LoggingLevel)
                {
                    switch (log.Severity)
                    {
                        case LoggingEventType.DEBUG:
                            Console.ForegroundColor = ConsoleColor.Blue;
                            break;

                        case LoggingEventType.INFO:
                            Console.ForegroundColor = ConsoleColor.White;
                            break;

                        case LoggingEventType.WARN:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;

                        case LoggingEventType.ERROR:
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;

                        case LoggingEventType.FATAL:
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            break;

                        default:
                            Console.ResetColor();
                            break;
                    }

                    if (EnableJSON)
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                        serializer.NullValueHandling = NullValueHandling.Ignore;

                        JObject logObject = JObject.FromObject(log, serializer);
                        logObject.AddFirst(new JProperty("TimeStamp", TimeStamp));

                        logEntryString = logObject.ToString(Formatting.None);
                    }

                    using (TextWriter writer = Console.Out)
                    {
                        writer.WriteLine(logEntryString);
                        writer.Flush();
                    }

                    Console.ResetColor();
                }
            }
        }

        #endregion Methods
    }

    public class FileLogger : LoggerProvider
    {
        #region Methods

        private string CurrentLogFile
        {
            get
            {
                var timestamp = DateTime.Now;
                var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var subdir = string.Format("{0}\\logs\\{1}\\{2}", root, timestamp.ToString("yyyy"), timestamp.ToString("MMMM"));
                var logfile = string.Format("{0}.log", timestamp.ToString("ddMMyyyy"));

                DirectoryInfo _directoryInfo = Directory.CreateDirectory(subdir);

                return _directoryInfo.FullName + "//" + logfile;
            }
        }

        public override void Write(ILogEntry log)
        {
            lock (lockObj)
            {
                string logEntryString = $"{TimeStamp} [{log.Severity}] {log.Message} {log.Type} {log.Exception}";

                if (log.Severity >= (LoggingEventType)LoggingLevel)
                {
                    if (EnableJSON)
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                        serializer.NullValueHandling = NullValueHandling.Ignore;

                        JObject logObject = JObject.FromObject(log, serializer);
                        logObject.AddFirst(new JProperty("TimeStamp", TimeStamp));

                        logEntryString = logObject.ToString(Formatting.None);
                    }

                    using (StreamWriter writer = File.AppendText(CurrentLogFile))
                    {
                        writer.WriteLine(logEntryString);
                        writer.Flush();
                    }
                }
            }
        }

        #endregion Methods
    }
}