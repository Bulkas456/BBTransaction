using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Transaction.Session.Storage.TransactionData
{
    /// <summary>
    /// The transaction data for the storage.
    /// </summary>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    public interface ITransactionData<TData>
    {
        /// <summary>
        /// Gets the index in the definition for the current step.
        /// </summary>
        int CurrentStepIndex
        {
            get;
        }

        /// <summary>
        /// Gets the session start timestamp.
        /// </summary>
        DateTime StartTimestamp
        {
            get;
        }

        /// <summary>
        /// Gets the session id.
        /// </summary>
        Guid SessionId
        {
            get;
        }

        /// <summary>
        /// Gets the current data for the transaction.
        /// </summary>
        TData Data
        {
            get;
        }
    }
}
