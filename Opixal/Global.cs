using Opixal.Logging;
using Opixal.Logging.Provider;

namespace Opixal
{
    public static class Global
    {
        #region Properties

        // Start a LogManager
        public static LogManager LogManager = new LogManager
        {
            // Setup logging to console
            ConsoleLogger = new ConsoleLogger
            {
                EnableJSON = false,
                LoggingLevel = LoggingEventType.DEBUG,
            },

            // Setup logging to file
            FileLogger = new FileLogger
            {
                EnableJSON = false,
                LoggingLevel = LoggingEventType.DEBUG,
            }
        };

        #endregion Properties
    }
}