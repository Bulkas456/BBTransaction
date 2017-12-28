using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Info
{
    /// <summary>
    /// The transaction info.
    /// </summary>
    public interface ITransactionCreateInfo
    {
        /// <summary>
        /// Gets the name of the transaction.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets the current time.
        /// </summary>
        DateTime Now
        {
            get;
        }
    }
}
