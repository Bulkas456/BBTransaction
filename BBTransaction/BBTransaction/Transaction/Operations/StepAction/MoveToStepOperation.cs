using System;
using System.Collections.Generic;
using System.Text;
#if !NET35 && !NOASYNC
using System.Threading.Tasks;
#endif
using BBTransaction.Transaction.Session;
using BBTransaction.Transaction.Session.StepEnumerator;
using BBTransaction.Transaction.Session.StepEnumerator.StepMove;
using BBTransaction.Transaction.TransactionResult;
using BBTransaction.Transaction.Operations.Undo;
using BBTransaction.Transaction.Operations.SessionEnd;
using BBTransaction.Step;

namespace BBTransaction.Transaction.Operations.StepAction
{
    /// <summary>
    /// The move to a step operation.
    /// </summary>
    internal static class MoveToStepOperation
    {
        /// <summary>
        /// Moves the session to a step.
        /// </summary>
        /// <typeparam name="TStepId">The type of the step id.</typeparam>
        /// <typeparam name="TData">The type of the transaction data.</typeparam>
        /// <param name="context">The context.</param>
#if NET35 || NOASYNC
        public static void MoveToStep<TStepId, TData>(MoveToStepContext<TStepId, TData> context)
#else
        public static async Task MoveToStep<TStepId, TData>(MoveToStepContext<TStepId, TData> context)
#endif
        {
            switch (context.Session.MoveInfo.MoveType)
            {
                case MoveType.Forward:
#if NET35 || NOASYNC
                    MoveToStepOperation.MoveForward(context);
#else
                    await MoveToStepOperation.MoveForward(context);
#endif
                    break;

                case MoveType.Back:
#if NET35 || NOASYNC
                    MoveToStepOperation.MoveBack(context);
#else
                    await MoveToStepOperation.MoveBack(context);
#endif
                    break;

                default:
                    throw new NotSupportedException(string.Format("MoveType '{0}' is not supported.", context.Session.MoveInfo.MoveType));
            }
        }

#if NET35 || NOASYNC
        public static void MoveForward<TStepId, TData>(MoveToStepContext<TStepId, TData> context)
#else
        public static async Task MoveForward<TStepId, TData>(MoveToStepContext<TStepId, TData> context)
#endif
        {
            IStepEnumerator<TStepId, TData> stepEnumerator = context.Session.StepEnumerator;
            IMoveInfo<TStepId> moveInfo = context.Session.MoveInfo;
            int moveCount = 0;

            do
            {
                stepEnumerator.MoveNext();
                ++moveCount;
            }
            while (stepEnumerator.CurrentStep != null
                   && !moveInfo.Comparer.Equals(stepEnumerator.CurrentStep.Id, moveInfo.Id));

            context.Session.MoveInfo = null;

            if (stepEnumerator.CurrentStep == null)
            {
                for (int i = 0; i < moveCount; ++i)
                {
                    stepEnumerator.MovePrevious();
                }

#if NET35 || NOASYNC
                RunUndoOperation.RunUndo(new RunUndoContext<TStepId, TData>()
#else
                await RunUndoOperation.RunUndo(new RunUndoContext<TStepId, TData>()
#endif
                {
                    Result = ResultType.Failed,
                    CaughtException = new InvalidOperationException(string.Format("Could not move forward to a step with id '{0}' as the step does not exist.", moveInfo.Id)),
                    Session = context.Session
                });
                return;
            }

#if NET35 || NOASYNC
            context.MoveToStepFinishAction();
#else
            await context.MoveToStepFinishAction();
#endif
        }

#if NET35 || NOASYNC
        public static void MoveBack<TStepId, TData>(MoveToStepContext<TStepId, TData> context)
#else
        public static async Task MoveBack<TStepId, TData>(MoveToStepContext<TStepId, TData> context)
#endif
        {
            IMoveInfo<TStepId> moveInfo = context.Session.MoveInfo;
            bool stepFound = false;

            Func<ITransactionStep<TStepId, TData>, bool> processStepPredicate = step => 
            {
                if (step == null)
                {
                    return false;
                }

                if (stepFound)
                {
                    return false;
                }
                else
                {
                    stepFound = moveInfo.Comparer.Equals(step.Id, moveInfo.Id);
                    return true;
                }
            };

#if NET35 || NOASYNC
            RunUndoOperation.RunUndo(new RunUndoContext<TStepId, TData>()
#else
            await RunUndoOperation.RunUndo(new RunUndoContext<TStepId, TData>()
#endif
            {
                Session = context.Session,
                NoSessionEnd = true,
                ProcessStepPredicate = processStepPredicate,
#if NET35 || NOASYNC
                UndoFinishAction = () => MoveBackUndoFinishAction(context, stepFound)
#else
                UndoFinishAction = async () => await MoveBackUndoFinishAction(context, stepFound)
#endif
            });
        }

#if NET35 || NOASYNC
        public static void MoveBackUndoFinishAction<TStepId, TData>(MoveToStepContext<TStepId, TData> context, bool stepFound)
#else
        public static async Task MoveBackUndoFinishAction<TStepId, TData>(MoveToStepContext<TStepId, TData> context, bool stepFound)
#endif
        {
            if (context.Session.Ended)
            {
                context.Session.MoveInfo = null;
                return;
            }

            if (stepFound)
            {
                context.Session.MoveInfo = null;
                context.Session.StepEnumerator.MoveNext();
            }
            else
            { 
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
                .AddError(new InvalidOperationException(string.Format("Could not move back to a step with id '{0}' as the step does not exist.", context.Session.MoveInfo.Id))));
                context.Session.MoveInfo = null;
            }

#if NET35 || NOASYNC
            context.MoveToStepFinishAction();
#else
            await context.MoveToStepFinishAction();
#endif
        }
    }
}
