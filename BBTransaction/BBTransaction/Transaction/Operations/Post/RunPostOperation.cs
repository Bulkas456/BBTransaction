﻿using System;
using System.Collections.Generic;
using System.Text;
#if !NET35
using System.Threading.Tasks;
#endif
using BBTransaction.Transaction.Session;
using BBTransaction.Step;
using BBTransaction.Definition;
using BBTransaction.Step.Executor;
using BBTransaction.Step.Settings;
using BBTransaction.Transaction.Operations.SessionEnd;

namespace BBTransaction.Transaction.Operations.Post
{
    /// <summary>
    /// The run post actions operation.
    /// </summary>
    internal static class RunPostOperation
    {
#if NET35
        public static void RunPost<TStepId, TData>(SessionEndContext<TStepId, TData> context)
#else
        public static async Task RunPost<TStepId, TData>(SessionEndContext<TStepId, TData> context)
#endif
        {
            while (true)
            {
                ITransactionStep<TStepId, TData> step = context.Session.StepEnumerator.CurrentStep;

                if (step == null)
                {
                    SessionEndOperation.EndSession(context);
                    return;
                }

                ITransactionSession<TStepId, TData> session = context.Session;

                if (session.Recovered
                    && step.Settings.NotRunOnRecovered())
                {
                    session.TransactionContext
                           .Logger
                           .DebugFormat(
                              "Transaction '{0}': ignoring the post step action for step '{1}' with id '{2}' as the step cannot be executed on a recovered transaction.",
                              session.TransactionContext.Info.Name,
                              session.StepEnumerator.CurrentStepIndex,
                              session.StepEnumerator.CurrentStep.Id);
                    session.StepEnumerator.Increment();
                    continue;
                }
#if NET35
                if (step.PostAction == null)
#else
                if (step.PostAction == null
                    && step.AsyncPostAction == null)
#endif
                {
                    session.TransactionContext
                           .Logger
                           .DebugFormat(
                              "Transaction '{0}': no post step action for step '{1}' with id '{2}'.",
                              session.TransactionContext.Info.Name,
                              session.StepEnumerator.CurrentStepIndex,
                              session.StepEnumerator.CurrentStep.Id);
                    session.StepEnumerator.Increment();
                    continue;
                }

                IStepExecutor executor = step.PostActionExecutor != null
                                            ? step.PostActionExecutor
                                            : step.Settings.SameExecutorForAllActions()
                                                ? step.StepActionExecutor
                                                : null;

                if (executor != null
                    && executor.ShouldRun)
                {
#if NET35
                    executor.Run(() =>
                    {
                        ProcessPostOperation.ProcessPost(context);

                        if (!session.Ended)
                        {
                            session.StepEnumerator.Increment();
                            RunPostOperation.RunPost(context);
                        }
                    });
#else
                    executor.Run(async () =>
                    {
                        await ProcessPostOperation.ProcessPost(context);

                        if (!session.Ended)
                        {
                            session.StepEnumerator.Increment();
                            await RunPostOperation.RunPost(context);
                        }
                    });
#endif
                    return;
                }
                else
                { 
#if NET35
                    ProcessPostOperation.ProcessPost(context);
#else
                    await ProcessPostOperation.ProcessPost(context);
#endif
                    session.StepEnumerator.Increment();
                }
            }
        }
    }
}