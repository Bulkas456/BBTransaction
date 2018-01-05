using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Step;

namespace BBTransaction.Definition
{
    /// <summary>
    /// The definition for the transaction.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    public interface ITransactionDefinition<TStepId, TData>
    {
        /// <summary>
        /// Adds a step.
        /// </summary>
        /// <param name="step">The step to add.</param>
        /// <returns>The definition.</returns>
        ITransactionDefinition<TStepId, TData> Add(ITransactionStep<TStepId, TData> step);
    }
}
