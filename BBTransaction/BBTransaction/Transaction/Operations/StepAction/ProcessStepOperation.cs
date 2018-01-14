using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using BBTransaction.Step;
using BBTransaction.Transaction.Operations;
using BBTransaction.Transaction.Operations.Undo;
using BBTransaction.Transaction.Operations.SessionEnd;
using BBTransaction.Transaction.TransactionResult;
#if !NET35 && !NOASYNC
using System.Threading.Tasks;
#endif
using BBTransaction.Transaction.Session;

namespace BBTransaction.Transaction.Operations.StepAction
{
    /// <summary>
    /// The step processing operation.
    /// </summary>
    internal static class ProcessStepOperation
    {
#if NET35 || NOASYNC
        public static void ProcessStep<TStepId, TData>(this ITransactionSession<TStepId, TData> session)
#else
        public static async Task ProcessStep<TStepId, TData>(this ITransactionSession<TStepId, TData> session)
#endif
        {
            ITransactionStep<TStepId, TData> currentStep = session.StepEnumerator.CurrentStep;
            Stopwatch watch = new Stopwatch();

            try
            {
                watch.Start();
#if NET35 || NOASYNC
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

                if (session.Cancelled)
                {
                    session.TransactionContext
                           .Logger
                           .InfoFormat("Transaction '{0}' cancelled.", session.TransactionContext.Info.Name);
#if NET35 || NOASYNC
                    RunUndoOperation.RunUndo(new RunUndoContext<TStepId, TData>()
#else
                    await RunUndoOperation.RunUndo(new RunUndoContext<TStepId, TData>()
#endif
                    {
                        Session = session,
                        Result = ResultType.Cancelled
                    });
                }
            }
            catch (Exception e)
            {
                watch.Stop();
                string info = string.Format("Transaction '{0}': an error occurred during processing step '{1}' with id '{2}', execution time '{3}'.", session.TransactionContext.Info.Name, session.StepEnumerator.CurrentStepIndex, currentStep.Id, watch.Elapsed);
                session.TransactionContext.Logger.ErrorFormat(e, info);
#if NET35 || NOASYNC
                RunUndoOperation.RunUndo(new RunUndoContext<TStepId, TData>()
#else
                await RunUndoOperation.RunUndo(new RunUndoContext<TStepId, TData>()
#endif
                {
                    Session = session,
                    CaughtException = e,
                    Result = ResultType.Failed
                });

#if NET35 || NOASYNC
                SessionEndPreparationOperation.PrepareEndSession(new SessionEndContext<TStepId, TData>()
#else
                await SessionEndPreparationOperation.PrepareEndSession(new SessionEndContext<TStepId, TData>()
#endif
                {
                    Session = session,
                    RunPostActions = false,
                    Result = ResultType.Failed
                }
                .AddError(e));
            }
        }
    }
}
