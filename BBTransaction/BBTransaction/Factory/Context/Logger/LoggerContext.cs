using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Logger;

namespace BBTransaction.Factory.Context.Logger
{
    /// <summary>
    /// The context for a transaction logger.
    /// </summary>
    public class LoggerContext : ILoggerContext
    {
        /// <summary>
        /// Gets or sets the debug format action.
        /// </summary>
        public Action<string, object[]> DebugFormatAction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the info format action.
        /// </summary>
        public Action<string, object[]> InfoFormatAction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the error format action.
        /// </summary>
        public Action<Exception, string, object[]> ErrorFormatAction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the execution time log action (optional).
        /// </summary>
        public Action<string, TimeSpan> ExecutionTimeLogAction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the logger instance which will be used in the transaction.
        /// </summary>
        public ILogger Logger
        {
            get;
            set;
        }
    }
}
