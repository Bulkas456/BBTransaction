﻿using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Step;
using BBTransaction.Step.Validator;
using System.Linq;
using BBTransaction.Definition.Standard.Context;
using BBTransaction.Transaction.Session.State;

namespace BBTransaction.Definition.Standard
{
    /// <summary>
    /// The standard definition storage for the transaction.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaciton data.</typeparam>
    internal class StandardTransactionDefinitionStorage<TStepId, TData> : ITransactionDefinitionStorage<TStepId, TData>
    {
        /// <summary>
        /// The synchronization lock.
        /// </summary>
        private readonly object syncLock = new object();

        /// <summary>
        /// The collection of steps.
        /// </summary>
        private readonly List<StepDetails<TStepId, TData>> steps = new List<StepDetails<TStepId, TData>>();

        /// <summary>
        /// The context.
        /// </summary>
        private readonly ITransactionDefinitionContext<TStepId> context;

        /// <summary>
        /// A value indicating whether a new steps can be added.
        /// </summary>
        private bool canAddStep = true;

        /// <summary>
        /// Initializes a new instance of the <see cref=""/>class.
        /// </summary>
        /// <param name="context">The context.</param>
        public StandardTransactionDefinitionStorage(ITransactionDefinitionContext<TStepId> context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets the step details for the transaction state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The step details for the transaction state.</returns>
        public IStepDetails<TStepId, TData> this[ITransactionState<TStepId, TData> state]
        {
            get
            {
                return state.CurrentStepIndex < this.steps.Count
                        ? this.steps[state.CurrentStepIndex]
                        : null;
            }
        }

        /// <summary>
        /// Gets the colleciton of all steps.
        /// </summary>
        public IEnumerable<IStepDetails<TStepId, TData>> Steps
        {
            get
            {
                return this.steps.Cast<IStepDetails<TStepId, TData>>();
            }
        }

        public ITransactionDefinition<TStepId, TData> Add(ITransactionStep<TStepId, TData> step)
        {
            lock (this.syncLock)
            {
                this.AssertCanAddStep();
                this.steps.Add(new StepDetails<TStepId, TData>(this.steps.Count, step.Validate()));
            }

            return this;
        }

        /// <summary>
        /// Adds a collection of steps.
        /// </summary>
        /// <param name="steps">The collection of steps to add.</param>
        /// <returns>The definition.</returns>
        public ITransactionDefinition<TStepId, TData> Add(IEnumerable<ITransactionStep<TStepId, TData>> steps)
        {
            lock(this.syncLock)
            {
                this.AssertCanAddStep();
                int startIndex = this.steps.Count;
                this.steps.AddRange(steps.Select((step, index) => new StepDetails<TStepId, TData>(startIndex + index, step.Validate())));
            }

            return this;
        }

        /// <summary>
        /// Notifies the definition that a transaction was started.
        /// </summary>
        public void NotifyTransactionStarted()
        {
            if (!this.canAddStep)
            {
                lock (this.syncLock)
                {
                    this.canAddStep = false;
                }
            }
        }

        /// <summary>
        /// Asserts that a new step cannot be added when a transaction for this definition was started.
        /// </summary>
        private void AssertCanAddStep()
        {
            if (!this.canAddStep)
            {
                throw new InvalidOperationException(string.Format("Transaction '{0}': cannot add a step when a transaction was started.", this.context.Info.Name));
            }
        }
    }
}
