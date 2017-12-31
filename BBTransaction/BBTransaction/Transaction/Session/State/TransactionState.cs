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
    internal class TransactionState<TStepId, TData> : ITransactionState<TStepId, TData>
    {
        /// <summary>
        /// Gets the index in the definition for the current step.
        /// </summary>
        public int CurrentStepIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// Increments the state.
        /// </summary>
        /// <param name="incrementValue">The increment value.</param>
        public void Increment(int incrementValue = 1)
        {
            this.CurrentStepIndex += incrementValue;
        }

        /// <summary>
        /// Decrements the state.
        /// </summary>
        public void Decrement()
        {
            this.Increment(-1);
        }
    }
}
