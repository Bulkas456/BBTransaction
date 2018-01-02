﻿using System;
using System.Collections.Generic;
using System.Text;
#if !NET35
using System.Threading;
using System.Threading.Tasks;
#endif
using BBTransaction.Transaction.Context;
using BBTransaction.Transaction.Session.State;
using BBTransaction.Transaction.Session.Storage.TransactionData;
using BBTransaction.Transaction.Settings;
using BBTransaction.Transaction.TransactionResult;

namespace BBTransaction.Transaction.Session
{
    /// <summary>
    /// The session for the transaction.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    internal class TransactionSession<TStepId, TData> : ITransactionSession<TStepId, TData>
    {
        /// <summary>
        /// A value indicating whether the session has started.
        /// </summary>
        private bool started;

#if !NET35
        /// <summary>
        /// The wait for result semaphore.
        /// </summary>
        private SemaphoreSlim waitForResultSemaphor = new SemaphoreSlim(0);
#endif

        /// <summary>
        /// The transaction result.
        /// </summary>
        private ITransactionResult<TData> result;

        /// <summary>
        /// Gets the transaction state.
        /// </summary>
        public ITransactionState<TStepId, TData> State => this.StateInstance;

        /// <summary>
        /// Gets the transaction state.
        /// </summary>
        public TransactionState<TStepId, TData> StateInstance
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the run transaction settings.
        /// </summary>
        public IRunSettings<TStepId, TData> RunSettings
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the transaction was recovered.
        /// </summary>
        public bool Recovered
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the session start timestamp.
        /// </summary>
        public DateTime StartTimestamp
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the session id.
        /// </summary>
        public Guid SessionId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the transaction context.
        /// </summary>
        public ITransactionContext<TStepId, TData> TransactionContext
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the session is ended.
        /// </summary>
        public bool Ended
        {
            get;
            private set;
        }

#if !NET35
        /// <summary>
        /// Waits for a transaction result.
        /// </summary>
        /// <returns>The task to wait.</returns>
        public async Task<ITransactionResult<TData>> WaitForResultAsync()
        {
            await this.waitForResultSemaphor.WaitAsync();
            this.waitForResultSemaphor.Dispose();
            this.waitForResultSemaphor = null;
            return this.result;
        }
#endif

        /// <summary>
        /// Starts the session.
        /// </summary>
        public void Start()
        {
            if (!this.started)
            {
                this.StartTimestamp = this.TransactionContext.Info.Now;
                this.SessionId = this.TransactionContext.Info.SessionIdCreator == null
                                   ? Guid.NewGuid()
                                   : this.TransactionContext.Info.SessionIdCreator();
            }
        }

        /// <summary>
        /// Ends the session.
        /// </summary>
        /// <param name="result">The transaction result.</param>
        public void End(ITransactionResult<TData> result)
        {
            this.Ended = true;
            this.result = result;
            this.RunSettings.TransactionResultCallback?.Invoke(this.result);
#if !NET35
            this.waitForResultSemaphor.Release();
#endif
        }

        /// <summary>
        /// Recovers the session.
        /// </summary>
        /// <param name="recoveredData">The recovered data.</param>
        public void Recover(ITransactionData<TData> recoveredData)
        {
            this.started = true;
            this.Recovered = true;
            this.StateInstance.CurrentStepIndex = recoveredData.CurrentStepIndex;
            this.StateInstance.Data = recoveredData.Data;
            this.SessionId = recoveredData.SessionId;
            this.StartTimestamp = recoveredData.StartTimestamp;
        }
    }
}
