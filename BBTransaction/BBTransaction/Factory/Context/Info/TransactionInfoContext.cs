using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Info
{
    /// <summary>
    /// The context for transaction info.
    /// </summary>
    public class TransactionInfoContext
    {
        /// <summary>
        /// Gets or sets the name of the transaction (required).
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the unciton which return the current time.
        /// </summary>
        public Func<DateTime> GetCurrentTimeFunction
        {
            get;
            set;
        }
    }
}
