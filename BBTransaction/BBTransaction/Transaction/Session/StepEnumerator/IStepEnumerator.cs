using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Definition;
using BBTransaction.Step;
using BBTransaction.Transaction.Settings;

namespace BBTransaction.Transaction.Session.StepEnumerator
{
    /// <summary>
    /// The state for the transaction.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    public interface IStepEnumerator<TStepId, TData>
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
        /// Gets the current step.
        /// </summary>
        ITransactionStep<TStepId, TData> CurrentStep
        {
            get;
        }

        /// <summary>
        /// Moves to the next step.
        /// </summary>
        void MoveNext();

        /// <summary>
        /// Moves to the previous step.
        /// </summary>
        void MovePrevious();

        /// <summary>
        /// Restarts the enumerator.
        /// </summary>
        void Restart();
    }
}
