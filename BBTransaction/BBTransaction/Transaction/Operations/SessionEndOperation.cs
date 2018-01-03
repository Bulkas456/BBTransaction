using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Transaction.TransactionResult;
#if !NET35
using System.Threading.Tasks;
#endif

namespace BBTransaction.Transaction.Operations
{
    /// <summary>
    /// The session end operation.
    /// </summary>
    internal static class SessionEndOperation
    {
#if NET35
        public static void EndSession<TStepId, TData>(SessionEndContext<TStepId, TData> context)
#else
        public static async Task EndSession<TStepId, TData>(SessionEndContext<TStepId, TData> context)
#endif
        {
            if (context.Session.Ended)
            {
                return;
            }

            context.Session.End(new TransactionResult<TStepId, TData>(context.Session));
        }
    }
}
