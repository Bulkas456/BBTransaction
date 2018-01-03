using System;
using System.Collections.Generic;
using System.Text;
#if !NET35
using System.Threading.Tasks;
#endif
using BBTransaction.Transaction.Session;

namespace BBTransaction.Transaction.Operations
{
    /// <summary>
    /// The process undo operation.
    /// </summary>
    internal static class ProcessUndoOperation
    {
#if NET35
        public static void ProcessUndo<TStepId, TData>(ProcessUndoContext<TStepId, TData> context)
#else
        public static async Task ProcessUndo<TStepId, TData>(ProcessUndoContext<TStepId, TData> context)
#endif
        {
        }
    }
}
