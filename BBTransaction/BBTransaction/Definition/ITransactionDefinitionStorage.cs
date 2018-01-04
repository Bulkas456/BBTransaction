using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Transaction.Session.State;

namespace BBTransaction.Definition
{
    /// <summary>
    /// The definition storage for the transaction.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    public interface ITransactionDefinitionStorage<TStepId, TData> : ITransactionDefinition<TStepId, TData>
    {
        /// <summary>
        /// Returns a step details for a step index.
        /// </summary>
        /// <param name="stepIndex">The step index.</param>
        /// <returns>The step details for the step index.</returns>
        IStepDetails<TStepId, TData> GetByIndex(int stepIndex);

        /// <summary>
        /// Returns a step details for a step id.
        /// </summary>
        /// <param name="id">The step id.</param>
        /// <returns>The step details for the step id.</returns>
        IStepDetails<TStepId, TData> GetById(TStepId id);

        /// <summary>
        /// Notifies the definition that a transaction was started.
        /// </summary>
        void NotifyTransactionStarted();
    }
}
