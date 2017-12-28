using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Factory.Context.Info
{
    /// <summary>
    /// The context for transaction info.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    public class TransactionInfoContext<TStepId> : ITransactionInfoContext<TStepId>
    {
        /// <summary>
        /// Gets or sets the name of the transaction.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the unciton which return the current time.
        /// </summary>
        public Func<DateTime> GetCurrentTimeFunction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the step id equality comparer.
        /// </summary>
        public IEqualityComparer<TStepId> StepIdComparer
        {
            get;
            set;
        }
    }
}
