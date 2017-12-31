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
    public interface ITransactionSession<TStepId, TData>
    {
        /// <summary>
        /// Gets the transaction state.
        /// </summary>
        ITransactionState<TStepId, TData> State
        {
            get;
        }

        /// <summary>
        /// Gets the run transaction settings.
        /// </summary>
        IRunSettings<TStepId, TData> RunSettings
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the transaction was recovered.
        /// </summary>
        bool Recovered
        {
            get;
        }
    }
}
