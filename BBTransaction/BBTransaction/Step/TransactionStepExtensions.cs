using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Step.Adapter;

namespace BBTransaction.Step
{
    /// <summary>
    /// Extensions for the transaction step.
    /// </summary>
    public static class TransactionStepExtensions
    {
        /// <summary>
        /// Creates an adapter for the step.
        /// </summary>
        /// <typeparam name="TStepIdTo">The type of the destination step id.</typeparam>
        /// <typeparam name="TDataTo">The type of the destination data.</typeparam>
        /// <typeparam name="TStepIdFrom">The type of the original step id.</typeparam>
        /// <typeparam name="TDataFrom">The type of the source data.</typeparam>
        /// <param name="original">The original step.</param>
        /// <param name="stepConverter">The step converter.</param>
        /// <param name="reverseStepConverter">The reversed step converter.</param>
        /// <param name="dataConverter">The data converter.</param>
        /// <returns>The adapter for the step.</returns>
        public static ITransactionStep<TStepIdTo, TDataTo> Adapter<TStepIdTo, TDataTo, TStepIdFrom, TDataFrom>(
            this ITransactionStep<TStepIdFrom, TDataFrom> step,
            Func<TStepIdFrom, TStepIdTo> stepConverter,
            Func<TStepIdTo, TStepIdFrom> reverseStepConverter,
            Func<TDataTo, TDataFrom> dataConverter)
        {
            return new TransactionStepAdapter<TStepIdTo, TDataTo, TStepIdFrom, TDataFrom>(step, stepConverter, reverseStepConverter, dataConverter);
        }
    }
}
