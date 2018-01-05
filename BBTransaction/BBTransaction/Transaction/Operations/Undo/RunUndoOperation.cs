using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Definition;
using BBTransaction.Step;
using BBTransaction.Transaction.Operations.SessionEnd;
#if !NET35
using System.Threading.Tasks;
#endif
using BBTransaction.Transaction.Session;
using BBTransaction.Step.Settings;
using BBTransaction.Step.Executor;

namespace BBTransaction.Transaction.Operations.Undo
{
    /// <summary>
    /// The run undo operation.
    /// </summary>
    internal static class RunUndoOperation
    {
#if NET35
        public static void RunUndo<TStepId, TData>(RunUndoContext<TStepId, TData> context)
#else
        public static async Task RunUndo<TStepId, TData>(RunUndoContext<TStepId, TData> context)
#endif
        {
            while (true)
            {
                ITransactionStep<TStepId, TData> step = context.Session.StepEnumerator.CurrentStep;

                if (step == null)
                {
#if NET35
                    SessionEndPreparationOperation.PrepareEndSession(new SessionEndContext<TStepId, TData>()
#else
                    await SessionEndPreparationOperation.PrepareEndSession(new SessionEndContext<TStepId, TData>()
#endif
                    {
                        RunPostActions = false,
                        Session = context.Session
                    }
                    .AddError(context.CaughtException));
                    return;
                }

                ITransactionSession<TStepId, TData> session = context.Session;

                if (session.Recovered
                    && !step.Settings.RunOnRecovered())
                {
                    session.TransactionContext
                           .Logger
                           .DebugFormat(
                              "Transaction '{0}': ignoring the undo step action for step '{1}' with id '{2}' as the step cannot be executed on a recovered transaction.",
                              session.TransactionContext.Info.Name,
                              session.StepEnumerator.CurrentStepIndex,
                              session.StepEnumerator.CurrentStep.Id);
                    session.StepEnumerator.Decrement();
                    continue;
                }
#if NET35
                if (step.UndoAction == null)
#else
                if (step.UndoAction == null
                    && step.AsyncUndoAction == null)
#endif
                {
                    session.TransactionContext
                           .Logger
                           .DebugFormat(
                              "Transaction '{0}': no undo step action for step '{1}' with id '{2}'.",
                              session.TransactionContext.Info.Name,
                              session.StepEnumerator.CurrentStepIndex,
                              session.StepEnumerator.CurrentStep.Id);
                    session.StepEnumerator.Decrement();
                    continue;
                }

                IStepExecutor executor = step.UndoActionExecutor != null
                                            ? step.UndoActionExecutor
                                            : step.Settings.SameExecutorForAllActions()
                                                ? step.StepActionExecutor
                                                : null;

                if (executor != null
                    && executor.ShouldRun)
                {
#if NET35
                    executor.Run(() =>
                    {
                        ProcessUndoOperation.ProcessUndo(context);

                        if (!session.Ended)
                        {
                            session.StepEnumerator.Decrement();
                            RunUndoOperation.RunUndo(context);
                        }
                    });
#else
                    executor.Run(async () =>
                    {
                        await ProcessUndoOperation.ProcessUndo(context);

                        if (!session.Ended)
                        {
                            session.StepEnumerator.Decrement();
                            await RunUndoOperation.RunUndo(context);
                        }
                    });
#endif
                    return;
                }
                else
                {
#if NET35
                    ProcessUndoOperation.ProcessUndo(context);
#else
                    await ProcessUndoOperation.ProcessUndo(context);
#endif
                    session.StepEnumerator.Decrement();
                }
            }
        }
    }
}
