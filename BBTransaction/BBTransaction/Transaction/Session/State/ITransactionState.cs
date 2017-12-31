using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Transaction.Settings;

namespace BBTransaction.Transaction.Session.State
{
    /// <summary>
    /// The state for the transaction.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    public interface ITransactionState<TStepId, TData>
    {
        /// <summary>
        /// Gets the index in the definition for the current step.
        /// </summary>
        int CurrentStepIndex
        {
            get;
        }

        /// <summary>
        /// Increments the state.
        /// </summary>
        /// <param name="incrementValue">The increment value.</param>
        void Increment(int incrementValue = 1);

        /// <summary>
        /// Decrements the state.
        /// </summary>
        void Decrement();
    }
}
