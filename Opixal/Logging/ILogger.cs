namespace Opixal.Logging
{
    public interface ILogger
    {
        #region Methods

        void Log(LogEntry entry);

        void Log<TState>(LogEntry<TState> entry);

        #endregion Methods
    }
}
