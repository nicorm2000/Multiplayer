namespace Net
{
    /// <summary>
    /// Provides debugging capabilities for reflection operations.
    /// </summary>
    public interface IReflectionDebugger
    {
        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Log(string message);

        /// <summary>
        /// Pauses execution for debugging purposes.
        /// </summary>
        public void Pause();
    }
}