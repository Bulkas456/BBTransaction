using System;
using System.Collections.Generic;
using System.Text;
#if !NET35 && !NOASYNC
using System.Threading.Tasks;
#endif
using BBTransaction.Transaction.Operations.SessionEnd;
using BBTransaction.Transaction.Session;
using BBTransaction.Transaction.Session.Storage.TransactionData;
using BBTransaction.Transaction.TransactionResult;

namespace BBTransaction.Transaction.Operations.SessionPreparation
{
    internal static class RunSessionPreparationOperation
    {
#if NET35 || NOASYNC
        public static void RunSessionPreparation<TStepId, TData>(ITransactionSession<TStepId, TData> session)
#else
        public static async Task RunSessionPreparation<TStepId, TData>(ITransactionSession<TStepId, TData> session)
#endif
        {
            if (session.Recovered)
            {
            }
            else
            {
#if NET35 || NOASYNC
                StartSession(session);
#else
                await StartSession(session);
#endif

            }
        }

        /// <summary>
        /// Starts the session. 
        /// </summary>
        /// <param name="session">The session.</param>
#if NET35 || NOASYNC
        private static void StartSession<TStepId, TData>(ITransactionSession<TStepId, TData> session)
#else
        private static async Task StartSession<TStepId, TData>(ITransactionSession<TStepId, TData> session)
#endif
        {
            try
            {
                session.TransactionContext.Definition.NotifyTransactionStarted();
#if NET35 || NOASYNC
                session.TransactionContext.SessionStorage.SessionStarted(new TransactionData<TStepId, TData>(session));
#else
                await session.TransactionContext.SessionStorage.SessionStarted(new TransactionData<TStepId, TData>(session));
#endif
            }
            catch (Exception e)
            {
                session.TransactionContext.Logger.ErrorFormat(e, "An error occurred during starting a session for transaction '{0}'.", session.TransactionContext.Info.Name);
#if NET35 || NOASYNC
                SessionEndPreparationOperation.PrepareEndSession(new SessionEndContext<TStepId, TData>()
#else
                await SessionEndPreparationOperation.PrepareEndSession(new SessionEndContext<TStepId, TData>()
#endif
                {
                    Session = session,
                    RunPostActions = false,
                    Result = ResultType.Failed
                }
                .AddError(e));
            }
        }
    }
}
