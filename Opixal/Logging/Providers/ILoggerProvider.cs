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
}