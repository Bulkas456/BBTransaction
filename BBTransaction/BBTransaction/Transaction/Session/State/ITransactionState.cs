using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Definition;
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
        /// Gets the data for the transaction.
        /// </summary>
        TData Data
        {
            get;
        }

        /// <summary>
        /// Gets or sets the current step.
        /// </summary>
        IStepDetails<TStepId, TData> CurrentStep
        {
            get;
            set;
        }

        /// <summary>
        /// Increments the state.
        /// </summary>
        void Increment();

        /// <summary>
        /// Decrements the state.
        /// </summary>
        void Decrement();
    }
}
