using System;
using System.Collections.Generic;
using System.Text;
#if !NET35
using System.Threading.Tasks;
#endif
using BBTransaction.Transaction.Session;

namespace BBTransaction.Transaction.Operations.Undo
{
    /// <summary>
    /// The process undo operation.
    /// </summary>
    internal static class RunUndoOperation
    {
#if NET35
        public static void RunUndo<TStepId, TData>(RunUndoContext<TStepId, TData> context)
#else
        public static async Task RunUndo<TStepId, TData>(RunUndoContext<TStepId, TData> context)
#endif
        {
        }
    }
}
