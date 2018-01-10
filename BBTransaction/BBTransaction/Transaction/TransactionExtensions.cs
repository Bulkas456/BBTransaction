using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Step;
using System.Linq;

namespace BBTransaction.Transaction
{
    /// <summary>
    /// Extensions for transactions.
    /// </summary>
    public static class TransactionExtensions
    {
        /// <summary>
        /// Adds a collection of steps to the transaction.
        /// </summary>
        /// <param name="steps">The collection of steps to add.</param>
        /// <returns>The transaction.</returns>
        public static ITransaction<TStepId, TData> Add<TStepId, TData>(this ITransaction<TStepId, TData> transaction, IEnumerable<ITransactionStep<TStepId, TData>> steps)
        {
            foreach (ITransactionStep<TStepId, TData> step in steps)
            {
                transaction.Add(step);
            }

            return transaction;
        }

        /// <summary>
        /// Adds an adapter for the step to the transaciton.
        /// </summary>
        /// <typeparam name="TStepIdTo">The type of the destination step id.</typeparam>
        /// <typeparam name="TDataTo">The type of the destination data.</typeparam>
        /// <typeparam name="TStepIdFrom">The type of the original step id.</typeparam>
        /// <typeparam name="TDataFrom">The type of the source data.</typeparam>
        /// <param name="step">The original step.</param>
        /// <param name="stepConverter">The step converter.</param>
        /// <param name="reverseStepConverter">The reversed step converter.</param>
        /// <param name="dataConverter">The data converter.</param>
        /// <returns>The transaction.</returns>
        public static ITransaction<TStepIdTo, TDataTo> AddAdapter<TStepIdTo, TDataTo, TStepIdFrom, TDataFrom>(
            this ITransaction<TStepIdTo, TDataTo> transaction, 
            ITransactionStep<TStepIdFrom, TDataFrom> step,
            Func<TStepIdFrom, TStepIdTo> stepConverter,
            Func<TStepIdTo, TStepIdFrom> reverseStepConverter,
            Func<TDataTo, TDataFrom> dataConverter)
        {
            return transaction.Add(step.Adapter(stepConverter, reverseStepConverter, dataConverter));
        }

        /// <summary>
        /// Adds adapters for a collection of steps to the transaciton.
        /// </summary>
        /// <typeparam name="TStepIdTo">The type of the destination step id.</typeparam>
        /// <typeparam name="TDataTo">The type of the destination data.</typeparam>
        /// <typeparam name="TStepIdFrom">The type of the original step id.</typeparam>
        /// <typeparam name="TDataFrom">The type of the source data.</typeparam>
        /// <param name="steps">The collection fo steps.</param>
        /// <param name="stepConverter">The step converter.</param>
        /// <param name="reverseStepConverter">The reversed step converter.</param>
        /// <param name="dataConverter">The data converter.</param>
        /// <returns>The transaction.</returns>
        public static ITransaction<TStepIdTo, TDataTo> AddAdapter<TStepIdTo, TDataTo, TStepIdFrom, TDataFrom>(
            this ITransaction<TStepIdTo, TDataTo> transaction,
            IEnumerable<ITransactionStep<TStepIdFrom, TDataFrom>> steps,
            Func<TStepIdFrom, TStepIdTo> stepConverter,
            Func<TStepIdTo, TStepIdFrom> reverseStepConverter,
            Func<TDataTo, TDataFrom> dataConverter)
        {
            return transaction.Add(steps.Select(step => step.Adapter(stepConverter, reverseStepConverter, dataConverter)));
        }

        /// <summary>
        /// Inserts a step adapter at an index.
        /// </summary>
        /// <typeparam name="TStepIdTo">The type of the destination step id.</typeparam>
        /// <typeparam name="TDataTo">The type of the destination data.</typeparam>
        /// <typeparam name="TStepIdFrom">The type of the original step id.</typeparam>
        /// <typeparam name="TDataFrom">The type of the source data.</typeparam>
        /// <param name="index">The index.</param>
        /// <param name="step">The step.</param>
        /// <param name="stepConverter">The step converter.</param>
        /// <param name="reverseStepConverter">The reversed step converter.</param>
        /// <param name="dataConverter">The data converter.</param>
        /// <returns>The transaction.</returns>
        public static ITransaction<TStepIdTo, TDataTo> InsertAdapterAtIndex<TStepIdTo, TDataTo, TStepIdFrom, TDataFrom>(
            this ITransaction<TStepIdTo, TDataTo> transaction,
            int index,
            ITransactionStep<TStepIdFrom, TDataFrom> step,
            Func<TStepIdFrom, TStepIdTo> stepConverter,
            Func<TStepIdTo, TStepIdFrom> reverseStepConverter,
            Func<TDataTo, TDataFrom> dataConverter)
        {
            return transaction.InsertAtIndex(index, step.Adapter(stepConverter, reverseStepConverter, dataConverter));
        }

        /// <summary>
        /// Inserts a colleciton of step adapters at an index.
        /// </summary>
        /// <typeparam name="TStepIdTo">The type of the destination step id.</typeparam>
        /// <typeparam name="TDataTo">The type of the destination data.</typeparam>
        /// <typeparam name="TStepIdFrom">The type of the original step id.</typeparam>
        /// <typeparam name="TDataFrom">The type of the source data.</typeparam>
        /// <param name="index">The index.</param>
        /// <param name="steps">The collection of steps.</param>
        /// <param name="stepConverter">The step converter.</param>
        /// <param name="reverseStepConverter">The reversed step converter.</param>
        /// <param name="dataConverter">The data converter.</param>
        /// <returns>The transaction.</returns>
        public static ITransaction<TStepIdTo, TDataTo> InsertAdapterAtIndex<TStepIdTo, TDataTo, TStepIdFrom, TDataFrom>(
            this ITransaction<TStepIdTo, TDataTo> transaction,
            int index,
            IEnumerable<ITransactionStep<TStepIdFrom, TDataFrom>> steps,
            Func<TStepIdFrom, TStepIdTo> stepConverter,
            Func<TStepIdTo, TStepIdFrom> reverseStepConverter,
            Func<TDataTo, TDataFrom> dataConverter)
        {
            return transaction.InsertAtIndex(index, steps.Select(step => step.Adapter(stepConverter, reverseStepConverter, dataConverter)));
        }

        /// <summary>
        /// Inserts a step adapter before a specific step id.
        /// </summary>
        /// <typeparam name="TStepIdTo">The type of the destination step id.</typeparam>
        /// <typeparam name="TDataTo">The type of the destination data.</typeparam>
        /// <typeparam name="TStepIdFrom">The type of the original step id.</typeparam>
        /// <typeparam name="TDataFrom">The type of the source data.</typeparam>
        /// <param name="id">The step id.</param>
        /// <param name="step">The step.</param>
        /// <param name="stepConverter">The step converter.</param>
        /// <param name="reverseStepConverter">The reversed step converter.</param>
        /// <param name="dataConverter">The data converter.</param>
        /// <param name="idComparer">The step id comparer.</param>
        /// <returns>The transaction.</returns>
        public static ITransaction<TStepIdTo, TDataTo> InsertAdapterBefore<TStepIdTo, TDataTo, TStepIdFrom, TDataFrom>(
            this ITransaction<TStepIdTo, TDataTo> transaction,
            TStepIdTo id,
            ITransactionStep<TStepIdFrom, TDataFrom> step,
            Func<TStepIdFrom, TStepIdTo> stepConverter,
            Func<TStepIdTo, TStepIdFrom> reverseStepConverter,
            Func<TDataTo, TDataFrom> dataConverter,
            IEqualityComparer<TStepIdTo> idComparer = null)
        {
            return transaction.InsertBefore(id, step.Adapter(stepConverter, reverseStepConverter, dataConverter), idComparer);
        }

        /// <summary>
        /// Inserts a collection of step adapters before a specific step id.
        /// </summary>
        /// <typeparam name="TStepIdTo">The type of the destination step id.</typeparam>
        /// <typeparam name="TDataTo">The type of the destination data.</typeparam>
        /// <typeparam name="TStepIdFrom">The type of the original step id.</typeparam>
        /// <typeparam name="TDataFrom">The type of the source data.</typeparam>
        /// <param name="id">The step id.</param>
        /// <param name="steps">The collection of steps.</param>
        /// <param name="stepConverter">The step converter.</param>
        /// <param name="reverseStepConverter">The reversed step converter.</param>
        /// <param name="dataConverter">The data converter.</param>
        /// <param name="idComparer">The step id comparer.</param>
        /// <returns>The transaction.</returns>
        public static ITransaction<TStepIdTo, TDataTo> InsertAdapterBefore<TStepIdTo, TDataTo, TStepIdFrom, TDataFrom>(
            this ITransaction<TStepIdTo, TDataTo> transaction,
            TStepIdTo id,
            IEnumerable<ITransactionStep<TStepIdFrom, TDataFrom>> steps,
            Func<TStepIdFrom, TStepIdTo> stepConverter,
            Func<TStepIdTo, TStepIdFrom> reverseStepConverter,
            Func<TDataTo, TDataFrom> dataConverter,
            IEqualityComparer<TStepIdTo> idComparer = null)
        {
            return transaction.InsertBefore(id, steps.Select(step => step.Adapter(stepConverter, reverseStepConverter, dataConverter)), idComparer);
        }

        /// <summary>
        /// Inserts a step adapter after a specific step id.
        /// </summary>
        /// <typeparam name="TStepIdTo">The type of the destination step id.</typeparam>
        /// <typeparam name="TDataTo">The type of the destination data.</typeparam>
        /// <typeparam name="TStepIdFrom">The type of the original step id.</typeparam>
        /// <typeparam name="TDataFrom">The type of the source data.</typeparam>
        /// <param name="id">The step id.</param>
        /// <param name="step">The step.</param>
        /// <param name="stepConverter">The step converter.</param>
        /// <param name="reverseStepConverter">The reversed step converter.</param>
        /// <param name="dataConverter">The data converter.</param>
        /// <param name="idComparer">The step id comparer.</param>
        /// <returns>The transaction.</returns>
        public static ITransaction<TStepIdTo, TDataTo> InsertAdapterAfter<TStepIdTo, TDataTo, TStepIdFrom, TDataFrom>(
            this ITransaction<TStepIdTo, TDataTo> transaction,
            TStepIdTo id,
            ITransactionStep<TStepIdFrom, TDataFrom> step,
            Func<TStepIdFrom, TStepIdTo> stepConverter,
            Func<TStepIdTo, TStepIdFrom> reverseStepConverter,
            Func<TDataTo, TDataFrom> dataConverter,
            IEqualityComparer<TStepIdTo> idComparer = null)
        {
            return transaction.InsertAfter(id, step.Adapter(stepConverter, reverseStepConverter, dataConverter), idComparer);
        }

        /// <summary>
        /// Inserts a collection of step adapters after a specific step id.
        /// </summary>
        /// <typeparam name="TStepIdTo">The type of the destination step id.</typeparam>
        /// <typeparam name="TDataTo">The type of the destination data.</typeparam>
        /// <typeparam name="TStepIdFrom">The type of the original step id.</typeparam>
        /// <typeparam name="TDataFrom">The type of the source data.</typeparam>
        /// <param name="id">The step id.</param>
        /// <param name="steps">The collection of steps.</param>
        /// <param name="stepConverter">The step converter.</param>
        /// <param name="reverseStepConverter">The reversed step converter.</param>
        /// <param name="dataConverter">The data converter.</param>
        /// <param name="idComparer">The step id comparer.</param>
        /// <returns>The transaction.</returns>
        public static ITransaction<TStepIdTo, TDataTo> InsertAdapterAfter<TStepIdTo, TDataTo, TStepIdFrom, TDataFrom>(
            this ITransaction<TStepIdTo, TDataTo> transaction,
            TStepIdTo id,
            IEnumerable<ITransactionStep<TStepIdFrom, TDataFrom>> steps,
            Func<TStepIdFrom, TStepIdTo> stepConverter,
            Func<TStepIdTo, TStepIdFrom> reverseStepConverter,
            Func<TDataTo, TDataFrom> dataConverter,
            IEqualityComparer<TStepIdTo> idComparer = null)
        {
            return transaction.InsertAfter(id, steps.Select(step => step.Adapter(stepConverter, reverseStepConverter, dataConverter)), idComparer);
        }
    }
}
