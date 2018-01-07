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
        void Add(ITransactionStep<TStepId, TData> step);

        /// <summary>
        /// Inserts a step at an index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="step">The step.</param>
        void InsertAtIndex(int index, ITransactionStep<TStepId, TData> step);

        /// <summary>
        /// Inserts a collection of steps at an index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="steps">The collection of step.</param>
        void InsertAtIndex(int index, IEnumerable<ITransactionStep<TStepId, TData>> steps);

        /// <summary>
        /// Inserts a step before a specific step id.
        /// </summary>
        /// <param name="id">The step id.</param>
        /// <param name="step">The step.</param>
        /// <param name="idComparer">The step id comparer.</param>
        void InsertBefore(TStepId id, ITransactionStep<TStepId, TData> step, IEqualityComparer<TStepId> idComparer = null);

        /// <summary>
        /// Inserts a collection of steps before a specific step id.
        /// </summary>
        /// <param name="id">The step id.</param>
        /// <param name="steps">The collection of steps.</param>
        /// <param name="idComparer">The step id comparer.</param>
        void InsertBefore(TStepId id, IEnumerable<ITransactionStep<TStepId, TData>> steps, IEqualityComparer<TStepId> idComparer = null);

        /// <summary>
        /// Inserts a step after a specific step id.
        /// </summary>
        /// <param name="id">The step id.</param>
        /// <param name="step">The comparer.</param>
        /// <param name="idComparer">The step id comparer.</param>
        void InsertAfter(TStepId id, ITransactionStep<TStepId, TData> step, IEqualityComparer<TStepId> idComparer = null);

        /// <summary>
        /// Inserts a collection of steps after a specific step id.
        /// </summary>
        /// <param name="id">The step id.</param>
        /// <param name="step">The collection of steps.</param>
        /// <param name="idComparer">The step id comparer.</param>
        void InsertAfter(TStepId id, IEnumerable<ITransactionStep<TStepId, TData>> steps, IEqualityComparer<TStepId> idComparer = null);

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
