using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Logger
{
    /// <summary>
    /// The logger.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs as a debug.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The format arguments.</param>
        void DebugFormat(string format, params object[] args);

        /// <summary>
        /// Logs as an info.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The format arguments.</param>
        void InfoFormat(string format, params object[] args);

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="e">The exception.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The format arguments.</param>
        void ErrorFormat(Exception e, string format, params object[] args);

        /// <summary>
        /// Logs an execution time.
        /// </summary>
        /// <param name="info">The log info.</param>
        /// <param name="executionTime">The execution time.</param>
        void LogExecutionTime(string info, TimeSpan executionTime);
    }
}
