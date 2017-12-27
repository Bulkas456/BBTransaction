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
    public class StepDetails<TStepId, TData>
    {
        /// <summary>
        /// The index of the step in the definition.
        /// </summary>
        private readonly int index;

        /// <summary>
        /// The step.
        /// </summary>
        private readonly ITransactionStep<TStepId, TData> step;

        /// <summary>
        /// Initializes a new instance of the <see cref="StepDetails<TStepId, TData>"/> class.
        /// </summary>
        /// <param name="index">The index of the step in the definition.</param>
        /// <param name="step">The step.</param>
        public StepDetails(int index, ITransactionStep<TStepId, TData> step)
        {
            this.index = index;
            this.step = step;
        }

        /// <summary>
        /// Gets the index of the step in the definition.
        /// </summary>
        public int Index
        {
            get
            {
                return this.index;
            }
        }

        /// <summary>
        /// Gets the step.
        /// </summary>
        public ITransactionStep<TStepId, TData> Step
        {
            get
            {
                return this.step;
            }
        }
    }
}
