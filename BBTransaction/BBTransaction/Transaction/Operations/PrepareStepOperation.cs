using System;
using System.Collections.Generic;
using System.Text;
#if !NET35
using System.Threading.Tasks;
#endif
using BBTransaction.Transaction.Context;
using BBTransaction.Transaction.Session;
using BBTransaction.Transaction.Session.Storage;

namespace BBTransaction.Transaction.Operations
{
    /// <summary>
    /// The prepare step operation.
    /// </summary>
    internal static class PrepareStepOperation
    {
#if NET35
        public static void PrepareStep<TStepId, TData>(this ITransactionSession<TStepId, TData> session)
#else
        public static async Task PrepareStep<TStepId, TData>(this ITransactionSession<TStepId, TData> session)
#endif
        {
            try
            {
#if NET35
                session.TransactionContext.SessionStorage.StepPrepared(session);
#else
                await session.TransactionContext.SessionStorage.StepPrepared(session);
#endif
            }
            catch (Exception e)
            {
                string info = string.Format("Transaction '{0}': an error occurred during notifying ste prepared.", session.TransactionContext.Info.Name);
                session.TransactionContext.Logger.ErrorFormat(e, info);
                session.StepEnumerator.Decrement();
#if NET35
                ProcessUndoOperation.ProcessUndo(new ProcessUndoContext<TStepId, TData>()
#else
                await ProcessUndoOperation.ProcessUndo(new ProcessUndoContext<TStepId, TData>()
#endif
                {
                    Session = session,
                    CaughtException = e
                });

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
    }
}
