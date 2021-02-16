using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Opixal.Logging.Provider
{
    public class ConsoleLoggerProvider : LoggerProviderBase
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

                        case LoggingEventType.INFORMATION:
                            Console.ForegroundColor = ConsoleColor.White;
                            break;

                        case LoggingEventType.WARNING:
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
}