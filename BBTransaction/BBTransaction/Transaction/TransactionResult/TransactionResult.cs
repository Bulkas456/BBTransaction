using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Result;
using BBTransaction.Transaction.Session;

namespace BBTransaction.Transaction.TransactionResult
{
    /// <summary>
    /// The transaction result.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    internal class TransactionResult<TStepId, TData> : OperationResult,
                                                       ITransactionResult<TData>
    {
        /// <summary>
        /// The transaction state.
        /// </summary>
        private readonly ITransactionSession<TStepId, TData> session;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionResult<TStepId, TData>"/> class.
        /// </summary>
        /// <param name="session">The state.</param>
        /// <param name="error">The error.</param>
        public TransactionResult(ITransactionSession<TStepId, TData> session, Exception error = null)
        {
            this.session = session;
            this.Add(error);

            if (!this.Success)
            {
                this.Info = "An error occurred.";
            }
        }

        /// <summary>
        /// Gets the transaction session.
        /// </summary>
        public ITransactionSession<TStepId, TData> State
        {
            get
            {
                return this.session;
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
                        ? this.session.RunSettings.Data
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
                return this.session == null
                        ? false
                        : this.session.Recovered;
            }
        }

        /// <summary>
        /// Gets an additional info about the result.
        /// </summary>
        public string Info
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the transaction result has a transaction state.
        /// </summary>
        public bool HasState
        {
            get
            {
                return this.session != null;
            }
        }
    }
}
