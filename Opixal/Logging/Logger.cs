using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks;

namespace Opixal.Logging
{
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