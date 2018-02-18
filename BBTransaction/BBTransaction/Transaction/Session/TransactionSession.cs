using System;
using System.Collections.Generic;
using System.Text;
#if !NET35 && !NOASYNC
using System.Threading;
using System.Threading.Tasks;
#endif
using BBTransaction.Transaction.Context;
using BBTransaction.Transaction.Session.StepEnumerator;
using BBTransaction.Transaction.Session.Storage.TransactionData;
using BBTransaction.Transaction.Settings;
using BBTransaction.Transaction.TransactionResult;
using BBTransaction.Executor;
using BBTransaction.Transaction.Session.StepEnumerator.StepMove;

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

#if !NET35 && !NOASYNC
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
        /// Gets the current step id.
        /// </summary>
        public TStepId CurrentStepId => this.StepEnumerator.CurrentStep.Id;

        /// <summary>
        /// Gets the transaction state.
        /// </summary>
        public IStepEnumerator<TStepId, TData> StepEnumerator => this.StepEnumeratorInstance;

        /// <summary>
        /// Gets the transaction state.
        /// </summary>
        public StepEnumerator<TStepId, TData> StepEnumeratorInstance
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

        /// <summary>
        /// Gets a value indicating whether the transaction is cancelled.
        /// </summary>
        public bool Cancelled
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the move info.
        /// </summary>
        public IMoveInfo<TStepId> MoveInfo
        {
            get;
            set;
        }

#if !NET35 && !NOASYNC
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
                this.started = true;
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
            if (this.Ended)
            {
                return;
            }

            this.Ended = true;
            this.result = result;
            IExecutor callbackExecutor = this.RunSettings.TransactionResultCallbackExecutor;

            if (callbackExecutor != null
                && callbackExecutor.ShouldRun)
            {
#if NET35 || NOASYNC
                callbackExecutor.Run(() => 
#else
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                callbackExecutor.Run(async () =>
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
#endif
                {
                    this.RunSettings.TransactionResultCallback?.Invoke(this.result);
#if !NET35 && !NOASYNC
                    this.waitForResultSemaphor.Release();
#endif
                });
            }
            else
            {
                this.RunSettings.TransactionResultCallback?.Invoke(this.result);
#if !NET35 && !NOASYNC
                this.waitForResultSemaphor.Release();
#endif
            }
        }

        /// <summary>
        /// Recovers the session.
        /// </summary>
        /// <param name="recoveredData">The recovered data.</param>
        public void Recover(ITransactionData<TData> recoveredData)
        {
            this.started = true;
            this.Recovered = true;
            this.StepEnumeratorInstance.CurrentStepIndex = recoveredData.CurrentStepIndex;
            this.StepEnumeratorInstance.Data = this.RunSettings.DontRecoverTransactionData()
                                                 ? this.RunSettings.Data
                                                 : recoveredData.Data;
            this.SessionId = recoveredData.SessionId;
            this.StartTimestamp = recoveredData.StartTimestamp;
        }

        /// <summary>
        /// Cancels the transaction.
        /// </summary>
        public void Cancel()
        {
            this.Cancelled = true;
        }

        /// <summary>
        /// Moves the transaction forward to a specific step. 
        /// </summary>
        /// <param name="id">The step id to move.</param>
        /// <param name="comparer">The equality comparer.</param>
        public void GoForward(TStepId id, IEqualityComparer<TStepId> comparer)
        {
            this.MoveInfo = new MoveInfo<TStepId>()
            {
                Id = id,
                Comparer = comparer ?? EqualityComparer<TStepId>.Default,
                MoveType = MoveType.Forward
            };
        }

        /// <summary>
        /// Moves the transaction back to a specific step (all undo functions for the back steps will be executed). 
        /// </summary>
        /// <param name="id">The step id to move.</param>
        /// <param name="comparer">The equality comparer.</param>
        public void GoBack(TStepId id, IEqualityComparer<TStepId> comparer)
        {
            this.MoveInfo = new MoveInfo<TStepId>()
            {
                Id = id,
                Comparer = comparer ?? EqualityComparer<TStepId>.Default,
                MoveType = MoveType.Back
            };
        }
    }
}
