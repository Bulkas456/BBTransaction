using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Transaction.Context;
using BBTransaction.Transaction.Session.Info;
#if !NET35 && !NOASYNC
using System.Threading.Tasks;
#endif
using BBTransaction.Transaction.Session.StepEnumerator;
using BBTransaction.Transaction.Settings;
using BBTransaction.Transaction.TransactionResult;

namespace BBTransaction.Transaction.Session
{
    /// <summary>
    /// The session for the transaction.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    public interface ITransactionSession<TStepId, TData> : ITransactionSessionInfo<TStepId>
    {
        /// <summary>
        /// Gets the transaction state.
        /// </summary>
        IStepEnumerator<TStepId, TData> StepEnumerator
        {
            get;
        }

        /// <summary>
        /// Gets the run transaction settings.
        /// </summary>
        IRunSettings<TStepId, TData> RunSettings
        {
            get;
        }

        /// <summary>
        /// Gets the transaction context.
        /// </summary>
        ITransactionContext<TStepId, TData> TransactionContext
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the transaction was recovered.
        /// </summary>
        bool Recovered
        {
            get;
        }

        /// <summary>
        /// Gets the session start timestamp.
        /// </summary>
        DateTime StartTimestamp
        {
            get;
        }

        /// <summary>
        /// Gets the session id.
        /// </summary>
        Guid SessionId
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the session is ended.
        /// </summary>
        bool Ended
        {
            get;
        }

#if !NET35 && !NOASYNC
        /// <summary>
        /// Waits for a transaction result.
        /// </summary>
        /// <returns>The task to wait.</returns>
        Task<ITransactionResult<TData>> WaitForResultAsync();
#endif

        /// <summary>
        /// Ends the session.
        /// </summary>
        /// <param name="result">The transaction result.</param>
        void End(ITransactionResult<TData> result);

        /// <summary>
        /// Starts the session.
        /// </summary>
        void Start();
    }
}
