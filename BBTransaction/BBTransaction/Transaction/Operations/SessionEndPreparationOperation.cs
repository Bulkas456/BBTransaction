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
    internal static class SessionEndPreparationOperation
    {
#if NET35
        public static void PrepareEndSession<TStepId, TData>(SessionEndContext<TStepId, TData> context)
#else
        public static async Task PrepareEndSession<TStepId, TData>(SessionEndContext<TStepId, TData> context)
#endif
        {
            if (context.Session.Ended)
            {
                return;
            }

            if (context.RunPostActions)
            {
                context.Session.StepEnumerator.Restart();
#if NET35
                RunPostOperation.RunPost(context);
#else
                await RunPostOperation.RunPost(context);
#endif
            }
            else
            {
                SessionEndOperation.EndSession(context);
            }
        }
    }
}
