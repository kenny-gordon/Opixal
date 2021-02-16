using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;

namespace Opixal.Logging.Provider
{
    public class FileLoggerProvider : LoggerProviderBase
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