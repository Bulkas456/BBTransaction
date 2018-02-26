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
        /// The index in the definition for the current step.
        /// </summary>
        private readonly int currentStepIndex;

        /// <summary>
        /// The session start timestamp.
        /// </summary>
        private readonly DateTime startTimestamp;

        /// <summary>
        /// The session id.
        /// </summary>
        private readonly Guid sessionId;

        /// <summary>
        /// The current data for the transaction.
        /// </summary>
        private readonly TData data;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionData<TStepId, TData>"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public TransactionData(ITransactionSession<TStepId, TData> session)
        {
            this.currentStepIndex = session.StepEnumerator.CurrentStepIndex;
            this.startTimestamp = session.StartTimestamp;
            this.sessionId = session.SessionId;
            this.data = session.RunSettings.Data;
        }

        /// <summary>
        /// Gets the index in the definition for the current step.
        /// </summary>
        public int CurrentStepIndex
        {
            get
            {
                return this.currentStepIndex;
            }
        }

        /// <summary>
        /// Gets the session start timestamp.
        /// </summary>
        public DateTime StartTimestamp
        {
            get
            {
                return this.startTimestamp;
            }
        }

        /// <summary>
        /// Gets the session id.
        /// </summary>
        public Guid SessionId
        {
            get
            {
                return this.sessionId;
            }
        }

        /// <summary>
        /// Gets the current data for the transaction.
        /// </summary>
        public TData Data
        {
            get
            {
                return this.data;
            }
        }
    }
}
