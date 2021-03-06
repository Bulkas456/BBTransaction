﻿using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Transaction.Operations.Post;
using BBTransaction.Transaction.TransactionResult;
#if !NET35 && !NOASYNC
using System.Threading.Tasks;
#endif

namespace BBTransaction.Transaction.Operations.SessionEnd
{
    /// <summary>
    /// The session end operation.
    /// </summary>
    internal static class SessionEndPreparationOperation
    {
#if NET35 || NOASYNC
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
#if NET35 || NOASYNC
                RunPostOperation.RunPost(context);
#else
                await RunPostOperation.RunPost(context);
#endif
            }
            else
            {
#if NET35 || NOASYNC
                SessionEndOperation.EndSession(context);
#else
                await SessionEndOperation.EndSession(context);
#endif
            }
        }
    }
}
