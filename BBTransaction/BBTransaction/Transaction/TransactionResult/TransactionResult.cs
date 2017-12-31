using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Result;
using BBTransaction.State;

namespace BBTransaction.Transaction.TransactionResult
{
    /// <summary>
    /// The transaction result.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaciton data.</typeparam>
    internal class TransactionResult<TStepId, TData> : OperationResult,
                                                       ITransactionResult<TData>
    {
        /// <summary>
        /// The transaction state.
        /// </summary>
        private readonly ITransactionState<TStepId, TData> state;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionResult<TStepId, TData>"/> class.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="error">The error.</param>
        public TransactionResult(ITransactionState<TStepId, TData> state, Exception error = null)
        {
            this.state = state;
            this.Add(error);
        }

        /// <summary>
        /// Gets the transaction state.
        /// </summary>
        public ITransactionState<TStepId, TData> State
        {
            get
            {
                return this.state;
            }
        }

        /// <summary>
        /// Gets or sets the transaction data.
        /// </summary>
        public TData Data
        {
            get
            {
                return this.HasState
                        ? this.state.Settings.Data
                        : default(TData);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the transaction was recovered.
        /// </summary>
        public bool Recovered
        {
            get
            {
                return this.state == null
                        ? false
                        : this.state.Recovered;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the transaction result has a transaction state.
        /// </summary>
        public bool HasState
        {
            get
            {
                return this.state != null;
            }
        }
    }
}
