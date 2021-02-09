using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks;

namespace Opixal.Logging
{
    public class Logger : ILogger
    {
        private readonly BlockingCollection<Param> _Params = new BlockingCollection<Param>();

        public Logger()
        {
            Task.Factory.StartNew(() =>
            {
                foreach (Param param in _Params.GetConsumingEnumerable())
                {
                    switch (param.LogLevel)
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

                    (string TimeStamp, Param param) context = (TimeStamp, param);

                    Console.WriteLine(JsonSerializer.Serialize(context, new JsonSerializerOptions { IgnoreNullValues = true }));
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            });
        }

        ~Logger()
        {
            _Params.CompleteAdding();
        }

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
            Param param = new Param(logLevel, null, null, null, logMessage);
            _Params.Add(param);
        }

        public void Log(LogLevel logLevel, Exception logException)
        {
            Param param = new Param(logLevel, null, null, logException, null);
            _Params.Add(param);
        }

        public void Log<T>(LogLevel logLevel, T logObject, string logMessage)
        {
            Param param = new Param(logLevel, logObject, null, null, logMessage);
            _Params.Add(param);
        }

        public void Log<T>(LogLevel logLevel, T logObject, Exception logException)
        {
            Param param = new Param(logLevel, logObject, null, logException, null);
            _Params.Add(param);
        }

        public void Log<T>(LogLevel logLevel, T logObject, string logAction, string logMessage)
        {
            Param param = new Param(logLevel, logObject, logAction, null, logMessage);
            _Params.Add(param);
        }

        public void Log<T>(LogLevel logLevel, T logObject, string logAction, Exception logException)
        {
            Param param = new Param(logLevel, logObject, logAction, logException, null);
            _Params.Add(param);
        }
    }
}