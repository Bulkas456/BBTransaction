using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Transaction.Session.StepEnumerator
{
    /// <summary>
    /// Extensions for a step enumerator.
    /// </summary>
    public static class StepEnumeratorExtensions
    {
        /// <summary>
        /// Returns a value indicating whether the step enumerator is set to the first step.
        /// </summary>
        /// <typeparam name="TStepId">The type of the step id.</typeparam>
        /// <typeparam name="TData">The type of the transaction data.</typeparam>
        /// <param name="stepEnumerator">The step enumerator.</param>
        /// <returns><c>True</c> if the step enumerator is set to the first step, otherwise <c>false</c>.</returns>
        public static bool IsFirstStep<TStepId, TData>(this IStepEnumerator<TStepId, TData> stepEnumerator)
        {
            return stepEnumerator.CurrentStepIndex == 0;
        }
    }
}
