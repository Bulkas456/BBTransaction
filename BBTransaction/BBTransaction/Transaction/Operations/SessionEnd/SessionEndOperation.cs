using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Transaction.TransactionResult;
#if !NET35 && !NOASYNC
using System.Threading.Tasks;
#endif
using BBTransaction.Transaction.Session.Storage;

namespace BBTransaction.Transaction.Operations.SessionEnd
{
    /// <summary>
    /// The session end operation.
    /// </summary>
    internal static class SessionEndOperation
    {
#if NET35 || NOASYNC
        public static void EndSession<TStepId, TData>(SessionEndContext<TStepId, TData> context)
#else
        public static async Task EndSession<TStepId, TData>(SessionEndContext<TStepId, TData> context)
#endif
        {
            if (context.Session.Ended)
            {
                return;
            }

#if NET35 || NOASYNC
            SessionEndOperation.RemoveSessionFromStorage(context);
#else
            await SessionEndOperation.RemoveSessionFromStorage(context);
#endif

            TransactionResult<TStepId, TData> result = new TransactionResult<TStepId, TData>()
            {
                Session = context.Session,
                Result = context.Result,
                Errors = context.CaughtExceptions
            };
            context.Session.TransactionContext.Logger.InfoFormat(
                "Transaction '{0}' ended with result '{1}'",
                context.Session.TransactionContext.Info.Name,
                result.Result);
            context.Session.End(result);
        }

#if NET35 || NOASYNC
        public static void RemoveSessionFromStorage<TStepId, TData>(SessionEndContext<TStepId, TData> context)
#else
        public static async Task RemoveSessionFromStorage<TStepId, TData>(SessionEndContext<TStepId, TData> context)
#endif
        {
            try
            {
#if NET35 || NOASYNC
                context.Session.TransactionContext.SessionStorage.RemoveSession(context.Session);
#else
                await context.Session.TransactionContext.SessionStorage.RemoveSession(context.Session);
#endif
            }
            catch (Exception e)
            {
                context.Result = ResultType.Failed;
                context.AddError(e);
            }
        }
    }
}
