using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.State;

namespace BBTransaction.Definition
{
    /// <summary>
    /// The definition storage for the transaction.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaciton data.</typeparam>
    public interface ITransactionDefinitionStorage<TStepId, TData> : ITransactionDefinition<TStepId, TData>
    {
        /// <summary>
        /// Gets the step details for the transaction state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The step details for the transaction state.</returns>
        IStepDetails<TStepId, TData> this[ITransactionState<TStepId, TData> state]
        {
            get;
        }

        /// <summary>
        /// Notifies the definition that a transaction was started.
        /// </summary>
        void NotifyTransactionStarted();
    }
}
