using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
#if !NET35 
using System.Threading.Tasks;
#endif
using BBTransaction.Definition;
using BBTransaction.Result;
using BBTransaction.Transaction.Context;
using BBTransaction.Transaction.Settings;
using BBTransaction.Transaction.Settings.Validator;
using BBTransaction.Transaction.TransactionResult;
using BBTransaction.Step.Settings;
using System.Threading;
using BBTransaction.Transaction.Session;
using BBTransaction.Transaction.Session.State;

namespace BBTransaction.Transaction
{
    /// <summary>
    /// The transaction.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaciton data.</typeparam>
    public class Transaction<TStepId, TData> : ITransaction<TStepId, TData>
    {
        /// <summary>
        /// The context for the transaction.
        /// </summary>
        private readonly ITransactionContext<TStepId, TData> context;

        /// <summary>
        /// Initializes a new instance of the <see cref="Transaction<TStepId, TData>"/> class.
        /// </summary>
        /// <param name="context">The context for the transaction</param>
        public Transaction(ITransactionContext<TStepId, TData> context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets the definition for the transaction.
        /// </summary>
        public ITransactionDefinition<TStepId, TData> Definition => this.context.Definition;

#if NET35
        /// <summary>
        /// Runs the transaction.
        /// </summary>
        /// <param name="settings">The action to set settings.</param>
        public void Run(Action<IRunSettings<TStepId, TData>> settings)
#else
        public async Task<ITransactionResult<TData>> Run(Action<IRunSettings<TStepId, TData>> settings)
#endif
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            RunSettings<TStepId, TData> runSettings = new RunSettings<TStepId, TData>();
            settings(runSettings);
            runSettings.Validate(this.context);

            switch (runSettings.Mode)
            {
                case RunMode.Run:
                    break;
                case RunMode.RunFromStep:
                    break;
                case RunMode.RecoverAndUndoAndRun:
                    break;
                case RunMode.RecoverAndContinue:
                    break;

                default:
                    throw new ArgumentException(string.Format("Transaction '{0}': unknown run mode '{1}'.", this.context.Info.Name, runSettings.Mode));
            }

#if !NET35
            return null;
#endif
        }

        /*protected virtual void NotifyTransactionEnded(ITransactionResult<TData> result)
        {
        }

        private void Run(IRunSettings<TStepId, TData> settings)
        {
            TransactionSession<TStepId, TData> session = new TransactionSession<TStepId, TData>
            {
                RunSettings = settings,
                State = new TransactionState<TStepId, TData>()
            };

            this.RunTransaction(session);
        }

        private void RunTransaction(ITransactionSession<TStepId, TData> session)
        {
            TransactionResult<TStepId, TData> notifyTransactionStartedResult = this.NotifyTransactionStarted(session);

            if (notifyTransactionStartedResult != null)
            {
                this.TransactionEnded(notifyTransactionStartedResult);
                return;
            }

           // this.RunStep(state);
        }

        private void TransactionEnded(TransactionResult<TStepId, TData> result)
        {
            if (result.Success
                && result.HasState)
            {
                TransactionResult<TStepId, TData> runPostActionResult = null;// this.RunPostAction(result.State);

                if (runPostActionResult != null)
                {
                    result.Add(runPostActionResult);
                    this.NotifyTransactionEnded(result);
                    return;
                }
            }
            
            if (result.HasState)
            {
                TransactionResult<TStepId, TData> removeStateResult = this.RemoveState(result.State);

                if (removeStateResult != null)
                {
                    result.Add(removeStateResult);
                    this.NotifyTransactionEnded(result);
                    return;
                }
            }

            this.NotifyTransactionEnded(result);
        }

        private TransactionResult<TStepId, TData> NotifyTransactionStarted(ITransactionSession<TStepId, TData> session)
        {
            try
            {
                this.context.Definition.NotifyTransactionStarted();
                this.context.StateStorage.NotifyTransactionStarted(state);
                return null;
            }
            catch (Exception e)
            {
                this.context.Logger.ErrorFormat(e, "An error occurred during notifying a transaction start for transaction '{0}'.", this.context.Info.Name);
                return new TransactionResult<TStepId, TData>(state, e);
            }
        }

#if NET35
        private TransactionResult<TStepId, TData> RunPostAction(ITransactionSession<TStepId, TData> session)
#else
        private async Task<TransactionResult<TStepId, TData>> RunPostAction(ITransactionSession<TStepId, TData> session)
#endif
        {
            this.context.Logger.DebugFormat("Running post actions for transaction '{0}'.", this.context.Info.Name);
            foreach (IStepDetails<TStepId, TData> step in this.context.Definition.Steps)
            {
                Stopwatch postActionWatch = new Stopwatch();

                try
                {
                    postActionWatch.Start();

#if NET35
                    step.Step.PostAction?.Invoke(state.Settings.Data);
#else
                    if (step.Step.PostAction == null)
                    {
                        await step.Step.AsyncPostAction(state.Settings.Data);
                    }
                    else
                    {
                        step.Step.PostAction(state.Settings.Data);
                    }
#endif
                    postActionWatch.Stop();
                    this.context.Logger.DebugFormat("Post action ended for step '{0}', step index '{1}' for transaction '{2}'.", step.Step.Id, step.Index, this.context.Info.Name);

                    if (state.Settings.LogTimeExecutionForAllSteps()
                        || step.Step.Settings.LogExecutionTime())
                    {
                        this.context.Logger.LogExecutionTime(string.Format("Post action for step '{0}' execution time", step.Step.Id), postActionWatch.Elapsed);
                    }
                }
                catch (Exception e)
                {
                    postActionWatch.Stop();
                    string info = string.Format(
                                    "An error occurred during a post action for transaction '{0}', step index '{1}', step id '{2}', execution time '{3} ms'.",
                                    this.context.Info.Name,
                                    step.Index,
                                    step.Step.Id,
                                    postActionWatch.ElapsedMilliseconds);
                    this.context.Logger.ErrorFormat(e, info, e);
                    return new TransactionResult<TStepId, TData>(state, new InvalidOperationException(info, e));
                }
            }

            return null;
        }

        private TransactionResult<TStepId, TData> RemoveSession(ITransactionSession<TStepId, TData> session)
        {
            try
            {
               // this.context.StateStorage.RemoveStates();
                return null;
            }
            catch (Exception e)
            {
                string undoInfo = string.Format("Error during removing a transaction state, transaction '{0}'.", this.context.Info.Name);
                this.context.Logger.ErrorFormat(e, undoInfo);
                return new TransactionResult<TStepId, TData>(state, new InvalidOperationException(undoInfo, e));
            }
        }*/
    }
}
