using System;
using System.Collections.Generic;
using System.Text;
#if !NET35
using System.Threading.Tasks;
#endif
using BBTransaction.Transaction.Session.Storage.TransactionData;

namespace BBTransaction.Transaction.Session.Storage
{
    /// <summary>
    /// Extensions for the transaction storage.
    /// </summary>
    internal static class TransactionStorageExtensions
    {
        /// <summary>
        /// Notifies that the transaction session started.
        /// </summary>
        /// <typeparam name="TStepId">The type of the step id.</typeparam>
        /// <typeparam name="TData">The type of the transaction data.</typeparam>
        /// <param name="storage">The storage.</param>
        /// <param name="session">the session.</param>
#if NET35
        public static void SessionStarted<TStepId, TData>(this ITransactionStorage<TData> storage, ITransactionSession<TStepId, TData> session)
        {
            storage.SessionStarted(new TransactionData<TStepId, TData>(session));
        }
#else
        public static async Task SessionStarted<TStepId, TData>(this ITransactionStorage<TData> storage, ITransactionSession<TStepId, TData> session)
        {
            await storage.SessionStarted(new TransactionData<TStepId, TData>(session));
        }
#endif

        /// <summary>
        /// Notifies that that a new step was prepared to run.
        /// </summary>
        /// <typeparam name="TStepId">The type of the step id.</typeparam>
        /// <typeparam name="TData">The type of the transaction data.</typeparam>
        /// <param name="storage">The storage.</param>
        /// <param name="session">the session.</param>
#if NET35
        public static void StepPrepared<TStepId, TData>(this ITransactionStorage<TData> storage, ITransactionSession<TStepId, TData> session)
        {
            storage.StepPrepared(new TransactionData<TStepId, TData>(session));
        }
#else
        public static async Task StepPrepared<TStepId, TData>(this ITransactionStorage<TData> storage, ITransactionSession<TStepId, TData> session)
        {
            await storage.StepPrepared(new TransactionData<TStepId, TData>(session));
        }
#endif

        /// <summary>
        /// Removes the transaction session.
        /// </summary>
        /// <typeparam name="TStepId">The type of the step id.</typeparam>
        /// <typeparam name="TData">The type of the transaction data.</typeparam>
        /// <param name="storage">The storage.</param>
        /// <param name="session">the session.</param>
#if NET35
        public static void RemoveSession<TStepId, TData>(this ITransactionStorage<TData> storage, ITransactionSession<TStepId, TData> session)
        {
            storage.RemoveSession(new TransactionData<TStepId, TData>(session));
        }
#else
        public static async Task RemoveSession<TStepId, TData>(this ITransactionStorage<TData> storage, ITransactionSession<TStepId, TData> session)
        {
            await storage.RemoveSession(new TransactionData<TStepId, TData>(session));
        }
#endif
    }
}
