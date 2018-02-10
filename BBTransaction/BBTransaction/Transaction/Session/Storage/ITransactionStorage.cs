using System;
using System.Collections.Generic;
using System.Text;
#if !NET35 && !NOASYNC
using System.Threading.Tasks;
#endif
using BBTransaction.Transaction.Session;
using BBTransaction.Transaction.Session.Storage.TransactionData;

namespace BBTransaction.Transaction.Session.Storage
{
    /// <summary>
    /// The storage for a transaction state.
    /// </summary>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    public interface ITransactionStorage<TData>
    {
        /// <summary>
        /// Notifies that the transaction session started.
        /// </summary>
        /// <param name="data">The transaction session data.</param>
#if NET35 || NOASYNC
        void SessionStarted(ITransactionData<TData> data);
#else
        Task SessionStarted(ITransactionData<TData> data);
#endif

        /// <summary>
        /// Notifies that that a new step was prepared to run.
        /// </summary>
        /// <param name="data">The transaction session data.</param>
#if NET35 || NOASYNC
        void StepPrepared(ITransactionData<TData> data);
#else
        Task StepPrepared(ITransactionData<TData> data);
#endif

        /// <summary>
        /// Notifies that that a step was receding.
        /// </summary>
        /// <param name="data">The transaction session data.</param>
#if NET35 || NOASYNC
        void StepReceding(ITransactionData<TData> data);
#else
        Task StepReceding(ITransactionData<TData> data);
#endif

        /// <summary>
        /// Recovers the transaction session data.
        /// </summary>
        /// <returns>The transaction session data.</returns>
#if NET35 || NOASYNC
        ITransactionData<TData> RecoverTransaction();
#else
        Task<ITransactionData<TData>> RecoverTransaction();
#endif

        /// <summary>
        /// Removes the transaction session.
        /// </summary>
        /// <param name="data">The transaction session data.</param>
#if NET35 || NOASYNC
        void RemoveSession(ITransactionData<TData> data);
#else
        Task RemoveSession(ITransactionData<TData> data);
#endif
    }
}
