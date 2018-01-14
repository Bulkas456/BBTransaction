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
        /// The collecion of errors for the operation result.
        /// </summary>
        private readonly List<Exception> errors = new List<Exception>();

        /// <summary>
        /// The transaction state.
        /// </summary>
        private readonly ITransactionSession<TStepId, TData> session;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionResult<TStepId, TData>"/> class.
        /// </summary>
        /// <param name="session">The state.</param>
        /// <param name="errors">The collection of errors.</param>
        public TransactionResult(ITransactionSession<TStepId, TData> session, IEnumerable<Exception> errors = null)
        {
            this.session = session;

            if (errors != null)
            {
                foreach (Exception error in errors)
                {
                    this.Add(error);
                }
            }
        }

        /// <summary>
        /// Gets the collection of exceptions for the operation.
        /// </summary>
        public IEnumerable<Exception> Errors
        {
            get
            {
                return this.errors;
            }
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
        /// Gets a value indicating whether the transaction result has a transaction state.
        /// </summary>
        public bool HasState
        {
            get
            {
                return this.session != null;
            }
        }

        /// <summary>
        /// Adds an error to the operation result.
        /// </summary>
        /// <param name="error">The error to add.</param>
        /// <returns>The operation result.</returns>
        public TransactionResult<TStepId, TData> Add(Exception error)
        {
            if (error != null)
            {
                this.errors.Add(error);
            }

            return this;
        }

        /// <summary>
        /// Merges the instance of the operation result with the other instance of an operation result.
        /// </summary>
        /// <param name="result">The operation result to merge.</param>
        /// <returns>The instance of the operation result.</returns>
        public TransactionResult<TStepId, TData> Add(ITransactionResult<TData> result)
        {
            if (result != null
                && result.Errors != null)
            {
                this.errors.AddRange(result.Errors);
            }

            return this;
        }
    }
}
