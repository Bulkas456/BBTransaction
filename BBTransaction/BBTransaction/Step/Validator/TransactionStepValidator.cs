using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Step.Validator
{
    internal static class TransactionStepValidator
    {
        public static void Validate<TStepId, TData>(this ITransactionStep<TStepId, TData> step)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            if (step.Description == null)
            {
                throw new NotSupportedException(string.Format(" '{0}' cannot be null.", nameof(step.Description)));
            }

            if (step.StepAction == null
#if !NET35
                && step.AsyncStepAction == null
#endif
                )
            {
                throw new NotSupportedException("No step action.");
            }

#if !NET35
            if (step.StepAction != null
                && step.AsyncStepAction != null)
            {
                throw new NotSupportedException(string.Format("'{0}' and '{1}' cannot be set simultaneously.", nameof(step.StepAction), nameof(step.AsyncStepAction)));
            }

            if (step.UndoAction != null
                && step.AsyncUndoAction != null)
            {
                throw new NotSupportedException(string.Format("'{0}' and '{1}' cannot be set simultaneously.", nameof(step.UndoAction), nameof(step.AsyncUndoAction)));
            }

            if (step.PostAction != null
                && step.AsyncPostAction != null)
            {
                throw new NotSupportedException(string.Format("'{0}' and '{1}' cannot be set simultaneously.", nameof(step.PostAction), nameof(step.AsyncPostAction)));
            }
#endif
        }
    }
}
