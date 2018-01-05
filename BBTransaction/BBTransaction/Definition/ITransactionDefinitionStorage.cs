using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Step;

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
        ITransactionStep<TStepId, TData> GetByIndex(int stepIndex);

        /// <summary>
        /// Notifies the definition that a transaction was started.
        /// </summary>
        void NotifyTransactionStarted();
    }
}
