using System;
using System.Collections.Generic;
using System.Text;
#if !NET35
using System.Threading.Tasks;
#endif
using BBTransaction.Definition;
using BBTransaction.Step.Executor;
using BBTransaction.Transaction.Session;
using BBTransaction.Step.Settings;

namespace BBTransaction.Transaction.Operations
{
    /// <summary>
    /// The run session operation.
    /// </summary>
    internal static class RunSessionOperation
    {
#if NET35
        public static void RunSession<TStepId, TData>(this ITransactionSession<TStepId, TData> session)
#else
        public static async Task RunSession<TStepId, TData>(this ITransactionSession<TStepId, TData> session)
#endif
        {
            while (true)
            {
                IStepDetails<TStepId, TData> step = session.TransactionContext.Definition[session.State];
                session.State.CurrentStep = step;

                if (step == null)
                {
#if NET35
                    SessionEndOperation.EndSession(new SessionEndContext<TStepId, TData>()
#else
                    await SessionEndOperation.EndSession(new SessionEndContext<TStepId, TData>()
#endif
                    {
                        Session = session,
                        RunPostActions = true
                    });
                    return;
                }

#if NET35
                session.PrepareStep();
#else
                await session.PrepareStep();
#endif

                if (session.Ended)
                {
                    return;
                }

                if (session.Recovered
                    && !step.Step.Settings.RunOnRecovered())
                {
                    session.TransactionContext
                           .Logger
                           .DebugFormat(
                              "Transaction '{0}': ignoring step '{1}' with id '{2}' as the step cannot be executed on a recovered transaction.",
                              session.TransactionContext.Info.Name,
                              session.State.CurrentStepIndex,
                              session.State.CurrentStep.Step.Id);
                    session.State.Increment();
                    continue;
                }

                session.TransactionContext
                           .Logger
                           .DebugFormat(
                              "Transaction '{0}: running step '{1}' with id '{2}'.", 
                              session.TransactionContext.Info.Name,
                              session.State.CurrentStepIndex,
                              session.State.CurrentStep.Step.Id);

                IStepExecutor executor = session.State.CurrentStep.Step.StepActionExecutor;

                if (executor != null
                    && executor.ShouldRun)
                {
#if NET35
                    executor.Run(() =>
                    {
                        session.ProcessStep();

                        if (!session.Ended)
                        {
                            session.State.Increment();
                            session.RunSession();
                        }
                    });
#else
                    executor.Run(async () =>
                    {
                        await session.ProcessStep();

                        if (!session.Ended)
                        {
                            session.State.Increment();
                            await session.RunSession();
                        }
                    });
#endif
                    return;
                }
                else
                {
#if NET35
                    session.ProcessStep();
#else
                    await session.ProcessStep();
#endif

                    if (session.Ended)
                    {
                        return;
                    }

                    session.State.Increment();
                }
            }
        }
    }
}
