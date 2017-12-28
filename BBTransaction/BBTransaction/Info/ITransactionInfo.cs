using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Info
{
    /// <summary>
    /// The transaction info.
    /// </summary>
    public interface ITransactionInfo
    {
        /// <summary>
        /// Gets the name of the transaction.
        /// </summary>
        string Name
        {
            get;
        }
    }
}
