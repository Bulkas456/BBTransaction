using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Step;
using BBTransaction.Step.Validator;
using System.Linq;
using BBTransaction.Transaction.Session.StepEnumerator;
using BBTransaction.Info;

namespace BBTransaction.Definition
{
    /// <summary>
    /// The standard definition storage for the transaction.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    internal class TransactionDefinition<TStepId, TData> : ITransactionDefinition<TStepId, TData>
    {
        /// <summary>
        /// The synchronization lock.
        /// </summary>
        private readonly object syncLock = new object();

        /// <summary>
        /// The collection of steps.
        /// </summary>
        private readonly List<ITransactionStep<TStepId, TData>> steps = new List<ITransactionStep<TStepId, TData>>();

        /// <summary>
        /// The info.
        /// </summary>
        private readonly ITransactionCreateInfo<TStepId> info;

        /// <summary>
        /// A value indicating whether a new steps can be added.
        /// </summary>
        private bool canAddStep = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionDefinition<TStepId, TData>"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        public TransactionDefinition(ITransactionCreateInfo<TStepId> info)
        {
            this.info = info;
        }

        /// <summary>
        /// Returns a step details for a step index.
        /// </summary>
        /// <param name="stepIndex">The step index.</param>
        /// <returns>The step details for the step index.</returns>
        public ITransactionStep<TStepId, TData> GetByIndex(int stepIndex)
        {
            return stepIndex > -1 
                   && stepIndex < this.steps.Count
                     ? this.steps[stepIndex]
                     : null;
        }

        /// <summary>
        /// Adds a step.
        /// </summary>
        /// <param name="step">The step to add.</param>
        /// <returns>The definition.</returns>
        public void Add(ITransactionStep<TStepId, TData> step)
        {
            lock (this.syncLock)
            {
                this.AssertCanAddStep();
                this.steps.Add(step.Validate());
            }
        }

        /// <summary>
        /// Inserts a step at an index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="step">The step.</param>
        public void InsertAtIndex(int index, ITransactionStep<TStepId, TData> step)
        {
            lock (this.syncLock)
            {
                this.AssertCanAddStep();
                this.steps.Insert(index, step);
            }
        }

        /// <summary>
        /// Inserts a collection of steps at an index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="steps">The collection of step.</param>
        public void InsertAtIndex(int index, IEnumerable<ITransactionStep<TStepId, TData>> steps)
        {
            lock (this.syncLock)
            {
                this.AssertCanAddStep();
                this.steps.InsertRange(index, steps);
            }
        }

        /// <summary>
        /// Inserts a step before a specific step id.
        /// </summary>
        /// <param name="id">The step id.</param>
        /// <param name="step">The step.</param>
        /// <param name="idComparer">The step id comparer.</param>
        public void InsertBefore(TStepId id, ITransactionStep<TStepId, TData> step, IEqualityComparer<TStepId> idComparer = null)
        {
            lock (this.syncLock)
            {
                this.AssertCanAddStep();
                int index = this.GetIndex(id, idComparer);
                this.steps.Insert(Math.Max(0, index), step);
            }
        }

        /// <summary>
        /// Inserts a collection of steps before a specific step id.
        /// </summary>
        /// <param name="id">The step id.</param>
        /// <param name="steps">The collection of steps.</param>
        /// <param name="idComparer">The step id comparer.</param>
        public void InsertBefore(TStepId id, IEnumerable<ITransactionStep<TStepId, TData>> steps, IEqualityComparer<TStepId> idComparer = null)
        {
            lock (this.syncLock)
            {
                this.AssertCanAddStep();
                int index = this.GetIndex(id, idComparer);
                this.steps.InsertRange(Math.Max(0, index), steps);
            }
        }

        /// <summary>
        /// Inserts a step after a specific step id.
        /// </summary>
        /// <param name="id">The step id.</param>
        /// <param name="step">The comparer.</param>
        /// <param name="idComparer">The step id comparer.</param>
        public void InsertAfter(TStepId id, ITransactionStep<TStepId, TData> step, IEqualityComparer<TStepId> idComparer = null)
        {
            lock (this.syncLock)
            {
                this.AssertCanAddStep();
                int index = this.GetIndex(id, idComparer);
                this.steps.Insert(Math.Max(0, index + 1), step);
            }
        }

        /// <summary>
        /// Inserts a collection of steps after a specific step id.
        /// </summary>
        /// <param name="id">The step id.</param>
        /// <param name="step">The collection of steps.</param>
        /// <param name="idComparer">The step id comparer.</param>
        public void InsertAfter(TStepId id, IEnumerable<ITransactionStep<TStepId, TData>> steps, IEqualityComparer<TStepId> idComparer = null)
        {
            lock (this.syncLock)
            {
                this.AssertCanAddStep();
                int index = this.GetIndex(id, idComparer);
                this.steps.InsertRange(Math.Max(0, index + 1), steps);
            }
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
                throw new InvalidOperationException(string.Format("Transaction '{0}': cannot add a step when a transaction was started.", this.info.Name));
            }
        }

        /// <summary>
        /// Gets an index of the step with a specific id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="idComparer">The comparer.</param>
        /// <returns>The index of the step with the id, if not found returns <c>-1</c>.</returns>
        private int GetIndex(TStepId id, IEqualityComparer<TStepId> idComparer)
        {
            IEqualityComparer<TStepId> comparer = idComparer ?? EqualityComparer<TStepId>.Default;

            for (int i = 0; i < this.steps.Count; ++i)
            {
                ITransactionStep<TStepId, TData> step = this.steps[i];

                if (comparer.Equals(step.Id, id))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
