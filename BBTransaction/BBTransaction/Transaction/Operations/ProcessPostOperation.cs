using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
#if !NET35
using System.Threading.Tasks;
#endif
using BBTransaction.Step;
using BBTransaction.Transaction.Session;

namespace BBTransaction.Transaction.Operations
{
    internal static class ProcessPostOperation
    {
#if NET35
        public static void ProcessPost<TStepId, TData>(SessionEndContext<TStepId, TData> context)
#else
        public static async Task ProcessPost<TStepId, TData>(SessionEndContext<TStepId, TData> context)
#endif
        {
            ITransactionStep<TStepId, TData> currentStep = context.Session.StepEnumerator.CurrentStep.Step;
            ITransactionSession<TStepId, TData> session = context.Session;
            Stopwatch watch = new Stopwatch();

            try
            {
                watch.Start();
#if NET35
                currentStep.PostAction(session.StepEnumerator.Data);
#else
                if (currentStep.PostAction != null)
                {
                    currentStep.PostAction(session.StepEnumerator.Data);
                }
                else
                {
                    await currentStep.AsyncPostAction(session.StepEnumerator.Data);
                }
#endif
                watch.Stop();

                if (session.ShouldLogStepExecution())
                {
                    session.TransactionContext
                           .Logger
                           .LogExecutionTime(
                               watch.Elapsed,
                               "Transaction '{0}': execution time for post step action for step '{1}' with id '{2}'.",
                               session.TransactionContext.Info.Name,
                               session.StepEnumerator.CurrentStepIndex,
                               currentStep.Id);
                }
            }
            catch (Exception e)
            {
                watch.Stop();
                string info = string.Format(
                    "Transaction '{0}': an error occurred during processing post step action for step '{1}' with id '{2}', execution time '{3}'.", 
                    session.TransactionContext.Info.Name, 
                    session.StepEnumerator.CurrentStepIndex, 
                    currentStep.Id, 
                    watch.Elapsed);
                session.TransactionContext.Logger.ErrorFormat(e, info);
                SessionEndOperation.EndSession(context.AddError(e));
            }
        }
    }
}
