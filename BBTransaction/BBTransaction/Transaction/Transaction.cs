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
using BBTransaction.Transaction.Session.Storage.TransactionData;
using BBTransaction.Transaction.Session.Storage;
using BBTransaction.Step.Executor;
using BBTransaction.Step;

namespace BBTransaction.Transaction
{
    /// <summary>
    /// The transaction.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
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
        /// <summary>
        /// Runs the transaction.
        /// </summary>
        /// <param name="settings">The action to set settings.</param>
        /// <returns>The result.</returns>
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

#if NET35
            ITransactionSession<TStepId, TData> session = this.CreateSession(runSettings);
            this.Run(session);
#else
            ITransactionSession<TStepId, TData> session = await this.CreateSession(runSettings);

            if (!session.Ended)
            {
                await this.Run(session);
            }

            return await session.WaitForResultAsync();
#endif
        }

#if NET35
        /// <summary>
        /// Runs the session.
        /// </summary>
        /// <param name="session">The session.</param>
        private void Run(ITransactionSession<TStepId, TData> session)
#else
        /// <summary>
        /// Runs the session.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns>The task.</returns>
        private async Task Run(ITransactionSession<TStepId, TData> session)
#endif
        {
            session.Start();
#if NET35
            this.StartSession(session);
#else
            await this.StartSession(session);
#endif

            if (session.Ended)
            {
                return;
            }

#if NET35
            this.RunSession(session);
#else
            await this.RunSession(session);
#endif
        }

#if NET35
        /// <summary>
        /// Starts the session. 
        /// </summary>
        /// <param name="session">The session.</param>
        private void StartSession(ITransactionSession<TStepId, TData> session)
#else
        /// <summary>
        /// Starts the session. 
        /// </summary>
        /// <param name="session">The session.</param>
        private async Task StartSession(ITransactionSession<TStepId, TData> session)
#endif
        {
            try
            {
                this.context.Definition.NotifyTransactionStarted();
#if NET35
                this.context.SessionStorage.SessionStarted(session);
#else
                await this.context.SessionStorage.SessionStarted(session);
#endif
            }
            catch (Exception e)
            {
                this.context.Logger.ErrorFormat(e, "An error occurred during starting a session for transaction '{0}'.", this.context.Info.Name);
                session.End(new TransactionResult<TStepId, TData>(session, e));
            }
        }

#if NET35
         /// <summary>
        /// Creates a session.
        /// </summary>
        /// <param name="runSettings">The run settings.</param>
        /// <returns>The session.</returns>
        ITransactionSession<TStepId, TData> CreateSession(IRunSettings<TStepId, TData> runSettings)
#else
        /// <summary>
        /// Creates a session.
        /// </summary>
        /// <param name="runSettings">The run settings.</param>
        /// <returns>The session.</returns>
        private async Task<ITransactionSession<TStepId, TData>> CreateSession(IRunSettings<TStepId, TData> runSettings)
#endif
        {
            TransactionState<TStepId, TData> state = new TransactionState<TStepId, TData>()
            {
                Data = runSettings.Data
            };
            TransactionSession<TStepId, TData> session = new TransactionSession<TStepId, TData>
            {
                RunSettings = runSettings,
                StateInstance = state,
                TransactionContext = this.context
            };

            switch (runSettings.Mode)
            {
                case RunMode.Run:
                    break;

                case RunMode.RunFromStep:
                    IStepDetails<TStepId, TData> step = this.context.Definition[session.RunSettings.FirstStepId];

                    if (step == null)
                    {
                        throw new ArgumentException(string.Format("Transaction '{0}': no first step '{1}' for mode '{2}'.", this.context.Info.Name, runSettings.FirstStepId, runSettings.Mode));
                    }

                    state.CurrentStepIndex = step.Index;
                    break;

                case RunMode.RecoverAndUndoAndRun:
                case RunMode.RecoverAndContinue:

                    ITransactionData<TData> recoveredData = null;

                    try
                    {
#if NET35
                        recoveredData = this.context.SessionStorage.RecoverTransaction();
#else
                        recoveredData = await this.context.SessionStorage.RecoverTransaction();
#endif
                    }
                    catch (Exception e)
                    {
                        this.context.Logger.ErrorFormat(e, "An error occurred during recovering the transaction '{0}'.", this.context.Info.Name);
                        session.End(new TransactionResult<TStepId, TData>(session, e));
                        return session;
                    }

                    if (recoveredData == null)
                    {
                        this.context.Logger.InfoFormat("Transaction '{0}': no session to recover.", this.context.Info.Name);
                        session.End(new TransactionResult<TStepId, TData>(session)
                        {
                            Info = "No session to recover."
                        });
                    }
                    else
                    {
                        session.Recover(recoveredData);
                    }

                    break;

                default:
                    throw new ArgumentException(string.Format("Transaction '{0}': unknown run mode '{1}'.", this.context.Info.Name, runSettings.Mode));
            }

            return session;
        }

#if NET35
        private void EndSession(TransactionResult<TStepId, TData> result)
#else
        private async Task EndSession(TransactionResult<TStepId, TData> result)
#endif
        {
            if (result.Success)
            {
                // post actions
            }

            result.Session.End(result);
            /*this.context.StateStorage.RemoveSession(

                && result.HasState)
            {
                TransactionResult<TItem> runPostActionResult = this.RunPostAction(result.State);

                if (runPostActionResult != null)
                {
                    result.AddError(runPostActionResult.Errors);
                    session.End(result);
                    return;
                }
            }

            if (result.HasState)
            {
                TransactionResult<TItem> removeStateResult = this.RemoveState(result.State);

                if (removeStateResult != null)
                {
                    result.AddError(removeStateResult.Errors);
                    session.End(result);
                    return;
                }
            }

            */
        }

#if NET35
        private void RunSession(ITransactionSession<TStepId, TData> session)
#else
        private async Task RunSession(ITransactionSession<TStepId, TData> session)
#endif
        {
            while (true)
            {
                IStepDetails<TStepId, TData> step = this.context.Definition[session.State];
                session.State.CurrentStep = step;

                if (step == null)
                {
#if NET35
                    this.EndSession(new TransactionResult<TStepId, TData>(session));
#else
                    await this.EndSession(new TransactionResult<TStepId, TData>(session));
#endif
                    return;
                }

#if NET35
                this.PrepareStep(session);
#else
                await this.PrepareStep(session);
#endif

                if (session.Ended)
                {
                    return;
                }

                if (session.Recovered
                    && !step.Step.Settings.RunOnRecovered())
                {
                    this.context.Logger.DebugFormat(
                        "Transaction '{0}': ignoring step '{1}' with id '{2}' as the step cannot be executed on a recovered transaction.",
                        this.context.Info.Name,
                        session.State.CurrentStepIndex,
                        session.State.CurrentStep.Step.Id);
                    session.State.Increment();
                    continue;
                }

                this.context.Logger.DebugFormat("Transaction '{0}: running step '{1}' with id '{2}'.", this.context.Info.Name, session.State.CurrentStepIndex, session.State.CurrentStep.Step.Id);

                IStepExecutor executor = session.State.CurrentStep.Step.Executor;

                if (executor != null
                    && executor.ShouldRun)
                {
#if NET35
                    executor.Run(() =>
                    {
                        this.ProcessStep(session);

                        if (!session.Ended)
                        {
                            session.State.Increment();
                            this.RunSession(session);
                        }
                    });
#else
                    executor.Run(async () => 
                    {
                        await this.ProcessStep(session);

                        if (!session.Ended)
                        {
                            session.State.Increment();
                            await this.RunSession(session);
                        }
                    });
#endif
                    return;
                }
                else
                {
#if NET35
                    this.ProcessStep(session);
#else
                    await this.ProcessStep(session);
#endif

                    if (session.Ended)
                    {
                        return;
                    }

                    session.State.Increment();
                }
            }
        }

#if NET35
        private void PrepareStep(ITransactionSession<TStepId, TData> session)
#else
        private async Task PrepareStep(ITransactionSession<TStepId, TData> session)
#endif
        {
            try
            {
#if NET35
                this.context.SessionStorage.StepPrepared(session);
#else
                await this.context.SessionStorage.StepPrepared(session);
#endif
            }
            catch (Exception e)
            {
                string info = string.Format("Transaction '{0}': an error occurred during notifying ste prepared.", this.context.Info.Name);
                this.context.Logger.ErrorFormat(e, info);
                session.State.Decrement();
                // this.ProcessUndo(state, true, e);

                if (!session.Ended)
                {
                    session.End(new TransactionResult<TStepId, TData>(session, new InvalidOperationException(info, e)));
                }
            }
        }

#if NET35
        private void ProcessStep(ITransactionSession<TStepId, TData> session)
#else
        private async Task ProcessStep(ITransactionSession<TStepId, TData> session)
#endif
        {
            Stopwatch watch = new Stopwatch();
            ITransactionStep<TStepId, TData> currentStep = session.State.CurrentStep.Step;

            try
            {
                watch.Start();
#if NET35
                currentStep.StepAction(session.State.Data, session);
#else
                if (currentStep.StepAction != null)
                {
                    currentStep.StepAction(session.State.Data, session); 
                }
                else
                {
                   await currentStep.AsyncStepAction(session.State.Data, session); 
                }
#endif
                watch.Stop();

                if (session.ShouldLogStepExecution())
                {
                    this.context.Logger.LogExecutionTime(watch.Elapsed, "Transaction '{0}': execution time for step '{1}' with id '{2}'.", this.context.Info.Name, session.State.CurrentStepIndex, currentStep.Id);
                }
            }
            catch (Exception e)
            {
                watch.Stop();
                string info = string.Format("Transaction '{0}': an error occurred during processing step '{1}' with id '{2}', execution time '{3}'.", this.context.Info.Name, session.State.CurrentStepIndex, currentStep.Id, watch.Elapsed);
                this.context.Logger.ErrorFormat(e, info);
                // this.ProcessUndo(state, true, e);

                if (!session.Ended)
                {
                    session.End(new TransactionResult<TStepId, TData>(session, new InvalidOperationException(info, e)));
                }
            }
        }
    }
}
