using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Transaction.Session;

namespace BBTransaction.Transaction.TransactionResult
{
    /// <summary>
    /// The transaction result.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    internal class TransactionResult<TStepId, TData> : ITransactionResult<TData>
    {
        /// <summary>
        /// Gets the collection of exceptions for the operation.
        /// </summary>
        public IEnumerable<Exception> Errors
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a transaction result.
        /// </summary>
        public ResultType Result
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the transaction session.
        /// </summary>
        public ITransactionSession<TStepId, TData> Session
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the transaction data.
        /// </summary>
        public TData Data
        {
            get
            {
                return this.Session == null
                        ? default(TData)
                        : this.Session.RunSettings.Data;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the transaction was recovered.
        /// </summary>
        public bool Recovered
        {
            get
            {
                return this.Session == null
                        ? false
                        : this.Session.Recovered;
            }
        }
    }
}
