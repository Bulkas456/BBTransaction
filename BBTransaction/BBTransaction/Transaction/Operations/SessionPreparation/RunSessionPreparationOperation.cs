using System;
using System.Collections.Generic;
using System.Text;
#if !NET35 && !NOASYNC
using System.Threading.Tasks;
#endif
using BBTransaction.Step;
using BBTransaction.Step.Settings;
using BBTransaction.Transaction.Operations.SessionEnd;
using BBTransaction.Transaction.Operations.StepAction;
using BBTransaction.Transaction.Operations.Undo;
using BBTransaction.Transaction.Session;
using BBTransaction.Transaction.Session.Storage.TransactionData;
using BBTransaction.Transaction.Settings;
using BBTransaction.Transaction.TransactionResult;
using BBTransaction.Transaction.Session.StepEnumerator;
using BBTransaction.Transaction.Session.Storage;

namespace BBTransaction.Transaction.Operations.SessionPreparation
{
    /// <summary>
    /// The run session preparation operation.
    /// </summary>
    internal static class RunSessionPreparationOperation
    {
#if NET35 || NOASYNC
        public static void RunSessionPreparation<TStepId, TData>(ITransactionSession<TStepId, TData> session)
#else
        public static async Task RunSessionPreparation<TStepId, TData>(ITransactionSession<TStepId, TData> session)
#endif
        {
            switch (session.RunSettings.Mode)
            {
                case RunMode.Run:
#if NET35 || NOASYNC
                    RunSessionPreparationOperation.StartRun(session);
#else
                    await RunSessionPreparationOperation.StartRun(session);
#endif
                    break;

                case RunMode.RecoverAndUndoAndRun:
#if NET35 || NOASYNC
                    RunSessionPreparationOperation.PrepareToRunFromScratch(session);
#else
                    await RunSessionPreparationOperation.PrepareToRunFromScratch(session);
#endif
                    break;

                case RunMode.RecoverAndContinue:
#if NET35 || NOASYNC
                    RunSessionPreparationOperation.PrepareToContinue(session);
#else
                    await RunSessionPreparationOperation.PrepareToContinue(session);
#endif
                    break;

                default:
                    throw new NotSupportedException(string.Format("RunMode '{0}' is not supported.", session.RunSettings.Mode));
            }
        }

        /// <summary>
        /// Starts the session. 
        /// </summary>
        /// <param name="session">The session.</param>
#if NET35 || NOASYNC
        private static void StartSession<TStepId, TData>(ITransactionSession<TStepId, TData> session)
#else
        private static async Task StartSession<TStepId, TData>(ITransactionSession<TStepId, TData> session)
#endif
        {
            try
            {
                session.TransactionContext.Definition.NotifyTransactionStarted();
#if NET35 || NOASYNC
                session.TransactionContext.SessionStorage.SessionStarted(session);
#else
                await session.TransactionContext.SessionStorage.SessionStarted(session);
#endif
            }
            catch (Exception e)
            {
                session.TransactionContext.Logger.ErrorFormat(e, "An error occurred during starting a session for transaction '{0}'.", session.TransactionContext.Info.Name);
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

#if NET35 || NOASYNC
        private static void PrepareToRunFromScratch<TStepId, TData>(ITransactionSession<TStepId, TData> session)
#else
        private static async Task PrepareToRunFromScratch<TStepId, TData>(ITransactionSession<TStepId, TData> session)
#endif
        {
#if NET35 || NOASYNC
            RunUndoOperation.RunUndo(new RunUndoContext<TStepId, TData>()
#else
            await RunUndoOperation.RunUndo(new RunUndoContext<TStepId, TData>()
#endif
            {
                NoSessionEnd = true,
                Session = session,
#if NET35 || NOASYNC
                UndoFinishAction = () => 
                {
                    session.StepEnumerator.MoveNext();
                    RunSessionOperation.RunSession(session);
                }
#else
                UndoFinishAction = async () =>
                {
                    session.StepEnumerator.MoveNext();
                    await RunSessionOperation.RunSession(session);
                }
#endif
            });
        }

#if NET35 || NOASYNC
        private static void PrepareToContinue<TStepId, TData>(ITransactionSession<TStepId, TData> session)
#else
        private static async Task PrepareToContinue<TStepId, TData>(ITransactionSession<TStepId, TData> session)
#endif
        {
            ITransactionStep<TStepId, TData> currentStep = session.StepEnumerator.CurrentStep;

            if (currentStep != null
                && currentStep.Settings.UndoOnRecover())
            {
#if NET35 || NOASYNC
                RunUndoOperation.RunUndo(new RunUndoContext<TStepId, TData>()
#else
                await RunUndoOperation.RunUndo(new RunUndoContext<TStepId, TData>()
#endif

                {
                    NoSessionEnd = true,
                    ProcessStepPredicate = step => step != null
                                                   && step.Settings.UndoOnRecover(),
                    Session = session,
#if NET35 || NOASYNC
                    UndoFinishAction = () => 
                    {
                        session.StepEnumerator.MoveNext();
                        RunSessionPreparationOperation.StartRun(session);
                    }
#else
                    UndoFinishAction = async () =>
                    {
                        session.StepEnumerator.MoveNext();
                        await RunSessionPreparationOperation.StartRun(session);
                    }
#endif
                });
            }
            else
            {
#if NET35 || NOASYNC
            RunSessionPreparationOperation.StartRun(session);
#else
            await RunSessionPreparationOperation.StartRun(session);
#endif
            }
        }

#if NET35 || NOASYNC
        private static void StartRun<TStepId, TData>(ITransactionSession<TStepId, TData> session)
#else
        private static async Task StartRun<TStepId, TData>(ITransactionSession<TStepId, TData> session)
#endif
        {
            if (session.Ended)
            {
                return;
            }

            if (session.StepEnumerator.IsFirstStep()
                && !session.Recovered)
            {
#if NET35 || NOASYNC
                RunSessionPreparationOperation.StartSession(session);
#else
                await RunSessionPreparationOperation.StartSession(session);
#endif
            }

#if NET35 || NOASYNC
            RunSessionOperation.RunSession(session);
#else
            await RunSessionOperation.RunSession(session);
#endif
            
        }
    }
}
