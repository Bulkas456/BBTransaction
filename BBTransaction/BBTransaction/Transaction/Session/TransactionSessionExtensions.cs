using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Transaction.Settings;
using BBTransaction.Step.Settings;

namespace BBTransaction.Transaction.Session
{
    /// <summary>
    /// Extensions for the transaction session.
    /// </summary>
    internal static class TransactionSessionExtensions
    {
        /// <summary>
        /// Checks if the execution time of the current step should be logged.
        /// </summary>
        /// <typeparam name="TStepId">The type of the step id.</typeparam>
        /// <typeparam name="TData">The type of the transaction data.</typeparam>
        /// <param name="session">The session.</param>
        /// <returns><c>True</c> if the execution time of the current step should be logged, otherwise <c>false</c>.</returns>
        public static bool ShouldLogStepExecution<TStepId, TData>(this ITransactionSession<TStepId, TData> session)
        {
            return session.RunSettings.LogTimeExecutionForAllSteps()
                   || session.StepEnumerator.CurrentStep.Settings.LogExecutionTime();
        }
    }
}
