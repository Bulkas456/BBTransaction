using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Result;

namespace BBTransaction.Transaction.TransactionResult
{
    /// <summary>
    /// The transaction result.
    /// </summary>
    /// <typeparam name="TData">The type of the transaciton data.</typeparam>
    public interface ITransactionResult<TData> : IOperationResult
    {
        /// <summary>
        /// Gets the transaction data.
        /// </summary>
        TData Data
        {
            get;
        }
    }
}
