using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Step;

namespace BBTransaction.Definition
{
    /// <summary>
    /// The details for a transaction step.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaciton data.</typeparam>
    public interface IStepDetails<TStepId, TData>
    {
        /// <summary>
        /// Gets the index of the step in the definition.
        /// </summary>
        int Index
        {
            get;
        }

        /// <summary>
        /// Gets the step.
        /// </summary>
        ITransactionStep<TStepId, TData> Step
        {
            get;
        }
    }
}
