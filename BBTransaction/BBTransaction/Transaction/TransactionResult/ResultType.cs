using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Transaction.TransactionResult
{
    /// <summary>
    /// The transaction result type.
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// The transaction was successfull.
        /// </summary>
        Success,

        /// <summary>
        /// The transaciton was cancelled.
        /// </summary>
        Cancelled,

        /// <summary>
        /// The transaction was failed.
        /// </summary>
        Failed,

        /// <summary>
        /// There was no transaction to recover.
        /// </summary>
        NoTransactionToRecover
    }
}
