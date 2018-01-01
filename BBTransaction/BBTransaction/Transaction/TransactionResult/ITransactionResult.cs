using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Result;

namespace BBTransaction.Transaction.TransactionResult
{
    /// <summary>
    /// The transaction result.
    /// </summary>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    public interface ITransactionResult<TData> : IOperationResult
    {
        /// <summary>
        /// Gets the transaction data.
        /// </summary>
        TData Data
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the transaction was recovered.
        /// </summary>
        bool Recovered
        {
            get;
        }

        /// <summary>
        /// Gets an additional info about the result.
        /// </summary>
        string Info
        {
            get;
        }
    }
}
