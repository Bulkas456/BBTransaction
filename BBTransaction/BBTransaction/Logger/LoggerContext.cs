using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Logger;

namespace BBTransaction.Logger
{
    /// <summary>
    /// The context for a logger.
    /// </summary>
    public class LoggerContext
    {
        /// <summary>
        /// Gets or sets the debug format action (optional).
        /// </summary>
        public Action<string, object[]> DebugFormatAction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the info format action (optional).
        /// </summary>
        public Action<string, object[]> InfoFormatAction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the error format action (optional).
        /// </summary>
        public Action<Exception, string, object[]> ErrorFormatAction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the logger instance which will be used in the transaciton (optional).
        /// </summary>
        public ILogger Logger
        {
            get;
            set;
        }
    }
}
