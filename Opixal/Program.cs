using Opixal.Logging;
using Opixal.Logging.Provider;
using Opixal.Network.Client;
using Opixal.Network.Server;
using System;

namespace Opixal
{
    internal class Program
    {
        #region Properties

        public static LogManager LogManager;

        #endregion Properties

        #region Methods

        private static void Main(string[] args)
        {
            // Start a LogManager
            LogManager = new LogManager
            {
                // Setup logging to console
                ConsoleLogger = new ConsoleLoggerProvider
                {
                    EnableJSON = false,
                    LoggingLevel = LoggingEventType.DEBUG,
                },

                // Setup logging to file
                FileLogger = new FileLoggerProvider
                {
                    EnableJSON = false,
                    LoggingLevel = LoggingEventType.DEBUG,
                }
            };

            // Start a Server on Port 5000
            Server.InitializeNetwork(5000);

            // Connect to a Server at "localhost" on Port 5000
            Client.InitializeNetwork("localhost", 5000);

            Console.ReadLine();
        }

        #endregion Methods
    }
}