using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Transaction.Session.Info
{
    /// <summary>
    /// The transaction session info for a step.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    public interface IStepTransactionSessionInfo<TStepId> : ITransactionSessionInfo<TStepId>
    {
        /// <summary>
        /// Cancels the transaction.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Moves the transaction forward to a specific step. 
        /// </summary>
        /// <param name="id">The step id to move.</param>
        /// <param name="comparer">The equality comparer.</param>
        void GoForward(TStepId id, IEqualityComparer<TStepId> comparer = null);

        /// <summary>
        /// Moves the transaction back to a specific step (all undo functions for the back steps will be executed). 
        /// </summary>
        /// <param name="id">The step id to move.</param>
        /// <param name="comparer">The equality comparer.</param>
        void GoBack(TStepId id, IEqualityComparer<TStepId> comparer = null);
    }
}
