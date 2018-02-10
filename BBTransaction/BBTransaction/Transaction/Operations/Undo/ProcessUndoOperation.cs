using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
#if !NET35 && !NOASYNC
using System.Threading.Tasks;
#endif
using BBTransaction.Step;
using BBTransaction.Transaction.Operations.SessionEnd;
using BBTransaction.Transaction.Session;
using BBTransaction.Transaction.TransactionResult;
using BBTransaction.Transaction.Session.Storage;

namespace BBTransaction.Transaction.Operations.Undo
{
    /// <summary>
    /// The process undo operation.
    /// </summary>
    internal static class ProcessUndoOperation
    {
#if NET35 || NOASYNC
        public static void ProcessUndo<TStepId, TData>(RunUndoContext<TStepId, TData> context)
#else
        public static async Task ProcessUndo<TStepId, TData>(RunUndoContext<TStepId, TData> context)
#endif
        {
            ITransactionStep<TStepId, TData> currentStep = context.Session.StepEnumerator.CurrentStep;
            ITransactionSession<TStepId, TData> session = context.Session;
            Stopwatch watch = new Stopwatch();

            try
            {
                watch.Start();
#if NET35 || NOASYNC
                currentStep.UndoAction(session.StepEnumerator.Data, session);
#else
                if (currentStep.UndoAction != null)
                {
                    currentStep.UndoAction(session.StepEnumerator.Data, session);
                }
                else
                {
                    await currentStep.AsyncUndoAction(session.StepEnumerator.Data, session);
                }
#endif
                watch.Stop();

#if NET35 || NOASYNC
                context.Session.TransactionContext.SessionStorage.StepReceding(session);
#else
                await context.Session.TransactionContext.SessionStorage.StepReceding(session);
#endif

                if (session.ShouldLogStepExecution())
                {
                    session.TransactionContext
                           .Logger
                           .LogExecutionTime(
                               watch.Elapsed,
                               "Transaction '{0}': execution time for undo step action for step '{1}' with id '{2}'.",
                               session.TransactionContext.Info.Name,
                               session.StepEnumerator.CurrentStepIndex,
                               currentStep.Id);
                }
            }
            catch (Exception e)
            {
                watch.Stop();
                string info = string.Format(
                    "Transaction '{0}': an error occurred during processing undo step action for step '{1}' with id '{2}', execution time '{3}'.",
                    session.TransactionContext.Info.Name,
                    session.StepEnumerator.CurrentStepIndex,
                    currentStep.Id,
                    watch.Elapsed);
                session.TransactionContext.Logger.ErrorFormat(e, info);
#if NET35 || NOASYNC
                SessionEndPreparationOperation.PrepareEndSession(new SessionEndContext<TStepId, TData>()
#else
                await SessionEndPreparationOperation.PrepareEndSession(new SessionEndContext<TStepId, TData>()
#endif
                {
                    Session = context.Session,
                    RunPostActions = false,
                    Result = ResultType.Failed
                }
                .AddError(context.CaughtException)
                .AddError(e));
            }
        }
    }
}
