using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Transaction.TransactionResult
{
    /// <summary>
    /// The transaction result.
    /// </summary>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    public interface ITransactionResult<TData>
    {
        /// <summary>
        /// Gets a transaction result.
        /// </summary>
        ResultType Result
        {
            get;
        }

        /// <summary>
        /// Gets the collection of exceptions for the operation.
        /// </summary>
        IEnumerable<Exception> Errors
        {
            get;
        }

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
    }
}
