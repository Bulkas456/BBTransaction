using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Info
{
    /// <summary>
    /// The context for transaction info.
    /// </summary>
    public class TransactionInfo : ITransactionInfo
    {
        /// <summary>
        /// Gets or sets the name of the transaction (required).
        /// </summary>
        public string Name
        {
            get;
            set;
        }
    }
}
