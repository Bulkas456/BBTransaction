using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Info
{
    /// <summary>
    /// The transaction info.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    public interface ITransactionCreateInfo<TStepId>
    {
        /// <summary>
        /// Gets the name of the transaction.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets the current time.
        /// </summary>
        DateTime Now
        {
            get;
        }

        /// <summary>
        /// Gets the step id equality comparer (optional).
        /// </summary>
        IEqualityComparer<TStepId> StepIdComparer
        {
            get;
        }

        /// <summary>
        /// Gets the session id creator (optional).
        /// </summary>
        Func<Guid> SessionIdCreator
        {
            get;
        }
    }
}
