using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Step.Validator
{
    /// <summary>
    /// The validator for a stransaction step.
    /// </summary>
    internal static class TransactionStepValidator
    {
        /// <summary>
        /// Validates a transaction step.
        /// </summary>
        /// <typeparam name="TStepId">The type of the step id.</typeparam>
        /// <typeparam name="TData">The type of the transaciton data.</typeparam>
        /// <param name="step">The step to validate.</param>
        public static ITransactionStep<TStepId, TData> Validate<TStepId, TData>(this ITransactionStep<TStepId, TData> step)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            if (step.Description == null)
            {
                throw new NotSupportedException(string.Format("Improper step data for id '{0}': '{1}' cannot be null.", step.Id, nameof(step.Description)));
            }

            if (step.StepAction == null
#if !NET35
                && step.AsyncStepAction == null
#endif
                )
            {
                throw new NotSupportedException(string.Format("Improper step data for id '{0}': No step action.", step.Id));
            }

#if !NET35
            if (step.StepAction != null
                && step.AsyncStepAction != null)
            {
                throw new NotSupportedException(string.Format("Improper step data for id '{0}': '{1}' and '{2}' cannot be set simultaneously.", step.Id, nameof(step.StepAction), nameof(step.AsyncStepAction)));
            }

            if (step.UndoAction != null
                && step.AsyncUndoAction != null)
            {
                throw new NotSupportedException(string.Format("Improper step data for id '{0}': '{1}' and '{2}' cannot be set simultaneously.", nameof(step.UndoAction), nameof(step.AsyncUndoAction)));
            }

            if (step.PostAction != null
                && step.AsyncPostAction != null)
            {
                throw new NotSupportedException(string.Format("Improper step data for id '{0}': '{1}' and '{2}' cannot be set simultaneously.", nameof(step.PostAction), nameof(step.AsyncPostAction)));
            }
#endif

            return step;
        }
    }
}
