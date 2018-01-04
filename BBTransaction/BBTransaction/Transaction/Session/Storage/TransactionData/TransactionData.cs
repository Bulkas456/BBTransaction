using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Transaction.Session.Storage.TransactionData
{
    /// <summary>
    /// The transaction data for the storage.
    /// </summary>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    public struct TransactionData<TStepId, TData> : ITransactionData<TData>
    {
        /// <summary>
        /// The session.
        /// </summary>
        private readonly ITransactionSession<TStepId, TData> session;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionData<TStepId, TData>"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public TransactionData(ITransactionSession<TStepId, TData> session)
        {
            this.session = session;
        }

        /// <summary>
        /// Gets the index in the definition for the current step.
        /// </summary>
        public int CurrentStepIndex
        {
            get
            {
                return this.session.StepEnumerator.CurrentStepIndex;
            }
        }

        /// <summary>
        /// Gets the session start timestamp.
        /// </summary>
        public DateTime StartTimestamp
        {
            get
            {
                return this.session.StartTimestamp;
            }
        }

        /// <summary>
        /// Gets the session id.
        /// </summary>
        public Guid SessionId
        {
            get
            {
                return this.session.SessionId;
            }
        }

        /// <summary>
        /// Gets the current data for the transaction.
        /// </summary>
        public TData Data
        {
            get
            {
                return this.session.RunSettings.Data;
            }
        }
    }
}
