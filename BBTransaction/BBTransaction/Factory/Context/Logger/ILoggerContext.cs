using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Logger;

namespace BBTransaction.Factory.Context.Logger
{
    /// <summary>
    /// The context for a transaction logger.
    /// </summary>
    public interface ILoggerContext
    {
        /// <summary>
        /// Gets or sets the debug format action (optional).
        /// </summary>
        Action<string, object[]> DebugFormatAction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the info format action (optional).
        /// </summary>
        Action<string, object[]> InfoFormatAction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the error format action (optional).
        /// </summary>
        Action<Exception, string, object[]> ErrorFormatAction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the logger instance which will be used in the transaciton (optional).
        /// </summary>
        ILogger Logger
        {
            get;
            set;
        }
    }
}
