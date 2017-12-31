using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Transaction.Session.State;
using BBTransaction.Transaction.Settings;

namespace BBTransaction.Transaction.Session
{
    /// <summary>
    /// The session for the transaction.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaciton data.</typeparam>
    internal class TransactionSession<TStepId, TData> : ITransactionSession<TStepId, TData>
    {
        /// <summary>
        /// Gets the transaction state.
        /// </summary>
        public ITransactionState<TStepId, TData> State
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the run transaction settings.
        /// </summary>
        public IRunSettings<TStepId, TData> RunSettings
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the transaction was recovered.
        /// </summary>
        public bool Recovered
        {
            get;
            set;
        }
    }
}
