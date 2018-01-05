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
using BBTransaction.Transaction.Session.StepEnumerator;
using BBTransaction.Transaction.Session.Storage.TransactionData;
using BBTransaction.Transaction.Session.Storage;
using BBTransaction.Step.Executor;
using BBTransaction.Step;
using BBTransaction.Transaction.Operations;
using BBTransaction.Transaction.Operations.StepAction;
using BBTransaction.Transaction.Operations.SessionEnd;

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
            RunSessionOperation.RunSession(session);
#else
            await RunSessionOperation.RunSession(session);
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
#if NET35
                SessionEndPreparationOperation.PrepareEndSession(new SessionEndContext<TStepId, TData>()
#else
                await SessionEndPreparationOperation.PrepareEndSession(new SessionEndContext<TStepId, TData>()
#endif
                {
                    Session = session,
                    RunPostActions = false
                }
                .AddError(e));
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
            TransactionSession<TStepId, TData> session = new TransactionSession<TStepId, TData>
            {
                RunSettings = runSettings,
                TransactionContext = this.context
            };
            session.StepEnumeratorInstance = new StepEnumerator<TStepId, TData>(session)
            {
                Data = runSettings.Data
            };

            switch (runSettings.Mode)
            {
                case RunMode.Run:
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
#if NET35
                        SessionEndPreparationOperation.PrepareEndSession(new SessionEndContext<TStepId, TData>()
#else
                        await SessionEndPreparationOperation.PrepareEndSession(new SessionEndContext<TStepId, TData>()
#endif
                        {
                            Session = session,
                            RunPostActions = false,
                        }
                        .AddError(e));
                        return session;
                    }

                    if (recoveredData == null)
                    {
                        this.context.Logger.InfoFormat("Transaction '{0}': no session to recover.", this.context.Info.Name);
#if NET35
                        SessionEndPreparationOperation.PrepareEndSession(new SessionEndContext<TStepId, TData>()
#else
                        await SessionEndPreparationOperation.PrepareEndSession(new SessionEndContext<TStepId, TData>()
#endif
                        {
                            Session = session,
                            AdditionalInfo = "No session to recover.",
                            RunPostActions = false
                        });
                        return session;
                    }
                    else
                    {
                        session.Recover(recoveredData);
                    }

                    break;

                default:
                    throw new ArgumentException(string.Format("Transaction '{0}': unknown run mode '{1}'.", this.context.Info.Name, runSettings.Mode));
            }

            session.StepEnumeratorInstance.FillStep();
            return session;
        }
    }
}
