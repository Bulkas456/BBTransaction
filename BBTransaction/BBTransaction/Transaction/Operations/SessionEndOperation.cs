using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Transaction.TransactionResult;

namespace BBTransaction.Transaction.Operations
{
    /// <summary>
    /// The session end operation.
    /// </summary>
    internal static class SessionEndOperation
    {
        public static void EndSession<TStepId, TData>(SessionEndContext<TStepId, TData> context)
        {
            if (context.Session.Ended)
            {
                return;
            }

            TransactionResult<TStepId, TData> result = new TransactionResult<TStepId, TData>(context.Session, context.CaughtExceptions);
            context.Session.TransactionContext.Logger.InfoFormat(
                "Transaction '{0}' ended with result '{1}'",
                context.Session.TransactionContext.Info.Name,
                result.Result);
            context.Session.End(result);
        }
    }
}
