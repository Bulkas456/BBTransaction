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
            await this.Run(session);
            return await session.WaitForResultAsync();
#endif
        }

#if NET35
        /// <summary>
        /// Runs the session.
        /// </summary>
        /// <param name="session">The session.</param>
        public void Run(ITransactionSession<TStepId, TData> session)
#else
        /// <summary>
        /// Runs the session.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns>The task.</returns>
        public async Task Run(ITransactionSession<TStepId, TData> session)
#endif
        {
            session.Start();
#if NET35
            TransactionResult<TStepId, TData> startTransactionResult = this.StartSession(session);
#else
            TransactionResult<TStepId, TData> startTransactionResult = await this.StartSession(session);
#endif

            if (startTransactionResult != null)
            {
                session.End(startTransactionResult);
                return;
            }

            //this.RunStep(state);
        }

#if NET35
        /// <summary>
        /// Starts the session. 
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns>An instance of a <see cref="TransactionResult<TStepId, TData>"/> if an error occurred during starting the session, otherwise <c>null</c>.</returns>
        private TransactionResult<TStepId, TData> StartSession(ITransactionSession<TStepId, TData> session)
#else
        /// <summary>
        /// Starts the session. 
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns>An instance of a <see cref="TransactionResult<TStepId, TData>"/> if an error occurred during starting the session, otherwise <c>null</c>.</returns>
        private async Task<TransactionResult<TStepId, TData>> StartSession(ITransactionSession<TStepId, TData> session)
#endif
        {
            try
            {
                this.context.Definition.NotifyTransactionStarted();
#if NET35
                this.context.StateStorage.SessionStarted(new TransactionData<TStepId, TData>(session));
#else
                await this.context.StateStorage.SessionStarted(new TransactionData<TStepId, TData>(session));
#endif
                return null;
            }
            catch (Exception e)
            {
                this.context.Logger.ErrorFormat(e, "An error occurred during starting a session for transaction '{0}'.", this.context.Info.Name);
                return new TransactionResult<TStepId, TData>(session, e);
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
                        recoveredData = this.context.StateStorage.RecoverTransaction();
#else
                        recoveredData = await this.context.StateStorage.RecoverTransaction();
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
    }
}
