using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using BBTransaction.Step;
using BBTransaction.Transaction.Operations;
#if !NET35
using System.Threading.Tasks;
#endif
using BBTransaction.Transaction.Session;

namespace BBTransaction.Transaction.Operations
{
    /// <summary>
    /// The step processing operation.
    /// </summary>
    internal static class ProcessStepOperation
    {
#if NET35
        public static void ProcessStep<TStepId, TData>(this ITransactionSession<TStepId, TData> session)
#else
        public static async Task ProcessStep<TStepId, TData>(this ITransactionSession<TStepId, TData> session)
#endif
        {
            ITransactionStep<TStepId, TData> currentStep = session.StepEnumerator.CurrentStep.Step;
            Stopwatch watch = new Stopwatch();

            try
            {
                watch.Start();
#if NET35
                currentStep.StepAction(session.StepEnumerator.Data, session);
#else
                if (currentStep.StepAction != null)
                {
                    currentStep.StepAction(session.StepEnumerator.Data, session);
                }
                else
                {
                    await currentStep.AsyncStepAction(session.StepEnumerator.Data, session);
                }
#endif
                watch.Stop();

                if (session.ShouldLogStepExecution())
                {
                    session.TransactionContext
                           .Logger
                           .LogExecutionTime(
                               watch.Elapsed, 
                               "Transaction '{0}': execution time for step '{1}' with id '{2}'.",
                               session.TransactionContext.Info.Name, 
                               session.StepEnumerator.CurrentStepIndex, 
                               currentStep.Id);
                }
            }
            catch (Exception e)
            {
                watch.Stop();
                string info = string.Format("Transaction '{0}': an error occurred during processing step '{1}' with id '{2}', execution time '{3}'.", session.TransactionContext.Info.Name, session.StepEnumerator.CurrentStepIndex, currentStep.Id, watch.Elapsed);
                session.TransactionContext.Logger.ErrorFormat(e, info);
#if NET35
                ProcessUndoOperation.ProcessUndo(new ProcessUndoContext<TStepId, TData>()
#else
                await ProcessUndoOperation.ProcessUndo(new ProcessUndoContext<TStepId, TData>()
#endif
                {
                    Session = session,
                    CaughtException = e
                });

                session.StepEnumerator.Decrement();
#if NET35
                SessionEndPreparationOperation.PrepareEndSession(new SessionEndContext<TStepId, TData>()
#else
                await SessionEndPreparationOperation.PrepareEndSession(new SessionEndContext<TStepId, TData>()
#endif
                {
                    Session = session,
                    RunPostActions = true
                }
                .AddError(e));
            }
        }
    }
}
