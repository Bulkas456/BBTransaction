using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
#if !NET35 
using System.Threading.Tasks;
#endif
using BBTransaction.Definition;
using BBTransaction.Result;
using BBTransaction.State;
using BBTransaction.Transaction.Context;
using BBTransaction.Transaction.Settings;
using BBTransaction.Transaction.Settings.Validator;
using BBTransaction.Transaction.TransactionResult;
using BBTransaction.Step.Settings;

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

        /// <summary>
        /// Runs the transaction.
        /// </summary>
        /// <param name="settings">The action to set settings.</param>
        public void Run(Action<IRunSettings<TStepId, TData>> settings)
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
        }

        protected virtual void NotifyTransactionEnded(ITransactionResult<TData> result)
        {
        }

        private void Run(IRunSettings<TStepId, TData> settings)
        {
            TransactionState<TStepId, TData> state = new TransactionState<TStepId, TData>()
            {
                Settings = settings
            };

            this.RunTransaction(state);
        }

        private void RunTransaction(ITransactionState<TStepId, TData> state)
        {
            TransactionResult<TStepId, TData> notifyTransactionStartedResult = this.NotifyTransactionStarted(state);

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
                TransactionResult<TStepId, TData> runPostActionResult = this.RunPostAction(result.State);

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

        private TransactionResult<TStepId, TData> NotifyTransactionStarted(ITransactionState<TStepId, TData> state)
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

        private TransactionResult<TStepId, TData> RunPostAction(ITransactionState<TStepId, TData> state)
        {
            this.context.Logger.DebugFormat("Running post actions for transaction '{0}'.", this.context.Info.Name);

            foreach (IStepDetails<TStepId, TData> step in this.context.Definition.Steps)
            {
                Stopwatch postActionWatch = new Stopwatch();

                try
                {
                    postActionWatch.Start();
                    //step.Step.PostAction();
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

        private TransactionResult<TStepId, TData> RemoveState(ITransactionState<TStepId, TData> state)
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
        }
    }
}
