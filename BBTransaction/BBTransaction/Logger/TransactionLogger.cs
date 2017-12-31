using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Factory.Context.Logger;

namespace BBTransaction.Logger
{
    /// <summary>
    /// The default transaction logger.
    /// </summary>
    internal class TransactionLogger : ILogger
    {
        /// <summary>
        /// The context.
        /// </summary>
        private readonly ILoggerContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionLogger"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public TransactionLogger(ILoggerContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Logs as a debug.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The format arguments.</param>
        public void DebugFormat(string format, params object[] args)
        {
            this.context.DebugFormatAction?.Invoke(format, args);
        }

        /// <summary>
        /// Logs as an info.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The format arguments.</param>
        public void InfoFormat(string format, params object[] args)
        {
            this.context.InfoFormatAction?.Invoke(format, args);
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="e">The exception.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The format arguments.</param>
        public void ErrorFormat(Exception e, string format, params object[] args)
        {
            this.context.ErrorFormatAction?.Invoke(e, format, args);
        }

        /// <summary>
        /// Logs an execution time.
        /// </summary>
        /// <param name="info">The log info.</param>
        /// <param name="executionTime">The execution time.</param>
        public void LogExecutionTime(string info, TimeSpan executionTime)
        {
            this.context.ExecutionTimeLogAction?.Invoke(info, executionTime);
        }
    }
}
