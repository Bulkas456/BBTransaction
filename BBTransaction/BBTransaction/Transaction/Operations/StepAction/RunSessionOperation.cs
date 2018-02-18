using System;
using System.Collections.Generic;
using System.Text;
#if !NET35 && !NOASYNC
using System.Threading.Tasks;
#endif
using BBTransaction.Definition;
using BBTransaction.Executor;
using BBTransaction.Transaction.Session;
using BBTransaction.Step.Settings;
using BBTransaction.Transaction.Operations.SessionEnd;
using BBTransaction.Step;
using BBTransaction.Transaction.TransactionResult;

namespace BBTransaction.Transaction.Operations.StepAction
{
    /// <summary>
    /// The run session operation.
    /// </summary>
    internal static class RunSessionOperation
    {
#if NET35 || NOASYNC
        public static void RunSession<TStepId, TData>(this ITransactionSession<TStepId, TData> session)
#else
        public static async Task RunSession<TStepId, TData>(ITransactionSession<TStepId, TData> session)
#endif
        {
            while (true)
            {
                ITransactionStep<TStepId, TData> step = session.StepEnumerator.CurrentStep;

                if (step == null)
                {
#if NET35 || NOASYNC
                    SessionEndPreparationOperation.PrepareEndSession(new SessionEndContext<TStepId, TData>()
#else
                    await SessionEndPreparationOperation.PrepareEndSession(new SessionEndContext<TStepId, TData>()
#endif
                    {
                        Session = session,
                        RunPostActions = true,
                        Result = ResultType.Success
                    });
                    return;
                }

#if NET35 || NOASYNC
                session.PrepareStep();
#else
                await session.PrepareStep();
#endif

                if (session.Ended)
                {
                    return;
                }

                if (session.Recovered
                    && step.Settings.NotRunOnRecovered())
                {
                    session.TransactionContext
                           .Logger
                           .DebugFormat(
                              "Transaction '{0}': ignoring step '{1}' with id '{2}' as the step cannot be executed on a recovered transaction.",
                              session.TransactionContext.Info.Name,
                              session.StepEnumerator.CurrentStepIndex,
                              session.StepEnumerator.CurrentStep.Id);
                    session.StepEnumerator.MoveNext();
                    continue;
                }

                session.TransactionContext
                           .Logger
                           .DebugFormat(
                              "Transaction '{0}: running step '{1}' with id '{2}'.", 
                              session.TransactionContext.Info.Name,
                              session.StepEnumerator.CurrentStepIndex,
                              session.StepEnumerator.CurrentStep.Id);

                IExecutor executor = session.StepEnumerator.CurrentStep.StepActionExecutor;

                if (executor != null
                    && executor.ShouldRun)
                {
#if NET35 || NOASYNC
                    executor.Run(() =>
                    {
                        session.ProcessStep();

                        if (!session.Ended)
                        {
                            RunSessionOperation.MoveToNextStep(session, true);
                        }
                    });
#else
                    executor.Run(async () =>
                    {
                        await session.ProcessStep();

                        if (!session.Ended)
                        {
                            await RunSessionOperation.MoveToNextStep(session, true);
                        }
                    });
#endif
                    return;
                }
                else
                {
#if NET35 || NOASYNC
                    session.ProcessStep();
#else
                    await session.ProcessStep();
#endif

                    if (session.Ended)
                    {
                        return;
                    }

#if NET35 || NOASYNC
                    RunSessionOperation.MoveToNextStep(session, false);
#else
                    await RunSessionOperation.MoveToNextStep(session, false);
#endif
                }
            }
        }

#if NET35 || NOASYNC
        private static void MoveToNextStep<TStepId, TData>(ITransactionSession<TStepId, TData> session, bool runSession)
#else
        private static async Task MoveToNextStep<TStepId, TData>(ITransactionSession<TStepId, TData> session, bool runSession)
#endif
        {
            if (session.MoveInfo == null)
            {
                session.StepEnumerator.MoveNext();

                if (runSession
                    && !session.Ended)
                {
#if NET35 || NOASYNC
                    RunSessionOperation.RunSession(session);
#else
                    await RunSessionOperation.RunSession(session);
#endif
                }

            }
            else
            {
#if NET35 || NOASYNC
                MoveToStepOperation.MoveToStep(new MoveToStepContext<TStepId, TData>()
#else
                await MoveToStepOperation.MoveToStep(new MoveToStepContext<TStepId, TData>()
#endif
                {
                    Session = session,
#if NET35 || NOASYNC
                    MoveToStepFinishAction = () => 
#else
                    MoveToStepFinishAction = async () =>
#endif
                    {
                        if (runSession
                            && !session.Ended)
                        {
#if NET35 || NOASYNC
                            RunSessionOperation.RunSession(session);
#else
                            await RunSessionOperation.RunSession(session);
#endif
                        }
                    }
                });
            }
        }
    }
}
