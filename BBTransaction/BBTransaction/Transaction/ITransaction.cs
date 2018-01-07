using System;
using System.Collections.Generic;
using System.Text;
#if !NET35 && !NOASYNC
using System.Threading.Tasks;
#endif
using BBTransaction.Definition;
using BBTransaction.Result;
using BBTransaction.Step;
using BBTransaction.Transaction.Settings;
using BBTransaction.Transaction.TransactionResult;

namespace BBTransaction.Transaction
{
    /// <summary>
    /// The transaction.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    public interface ITransaction<TStepId, TData>
    {
        /// <summary>
        /// Adds a step.
        /// </summary>
        /// <param name="step">The step to add.</param>
        /// <returns>The transaction.</returns>
        ITransaction<TStepId, TData> Add(ITransactionStep<TStepId, TData> step);

        /// <summary>
        /// Inserts a step at an index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="step">The step.</param>
        ITransaction<TStepId, TData> InsertAtIndex(int index, ITransactionStep<TStepId, TData> step);

        /// <summary>
        /// Inserts a collection of steps at an index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="steps">The collection of step.</param>
        ITransaction<TStepId, TData> InsertAtIndex(int index, IEnumerable<ITransactionStep<TStepId, TData>> steps);

        /// <summary>
        /// Inserts a step before a specific step id.
        /// </summary>
        /// <param name="id">The step id.</param>
        /// <param name="step">The step.</param>
        /// <param name="idComparer">The step id comparer.</param>
        ITransaction<TStepId, TData> InsertBefore(TStepId id, ITransactionStep<TStepId, TData> step, IEqualityComparer<TStepId> idComparer = null);

        /// <summary>
        /// Inserts a collection of steps before a specific step id.
        /// </summary>
        /// <param name="id">The step id.</param>
        /// <param name="steps">The collection of steps.</param>
        /// <param name="idComparer">The step id comparer.</param>
        ITransaction<TStepId, TData> InsertBefore(TStepId id, IEnumerable<ITransactionStep<TStepId, TData>> steps, IEqualityComparer<TStepId> idComparer = null);

        /// <summary>
        /// Inserts a step after a specific step id.
        /// </summary>
        /// <param name="id">The step id.</param>
        /// <param name="step">The comparer.</param>
        /// <param name="idComparer">The step id comparer.</param>
        ITransaction<TStepId, TData> InsertAfter(TStepId id, ITransactionStep<TStepId, TData> step, IEqualityComparer<TStepId> idComparer = null);

        /// <summary>
        /// Inserts a collection of steps after a specific step id.
        /// </summary>
        /// <param name="id">The step id.</param>
        /// <param name="step">The collection of steps.</param>
        /// <param name="idComparer">The step id comparer.</param>
        ITransaction<TStepId, TData> InsertAfter(TStepId id, IEnumerable<ITransactionStep<TStepId, TData>> steps, IEqualityComparer<TStepId> idComparer = null);

#if NET35 || NOASYNC
        /// <summary>
        /// Runs the transaction.
        /// </summary>
        /// <param name="settings">The action to set settings.</param>
        void Run(Action<IRunSettings<TStepId, TData>> settings);
#else
        /// <summary>
        /// Runs the transaction.
        /// </summary>
        /// <param name="settings">The action to set settings.</param>
        /// <returns>The result.</returns>
        Task<ITransactionResult<TData>> Run(Action<IRunSettings<TStepId, TData>> settings);
#endif
    }
}
