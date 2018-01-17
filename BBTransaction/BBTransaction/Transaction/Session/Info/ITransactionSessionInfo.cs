using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Transaction.Session.Info
{
    /// <summary>
    /// The transaction session info.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    public interface ITransactionSessionInfo<TStepId>
    {
        /// <summary>
        /// Gets the current step id.
        /// </summary>
        TStepId CurrentStepId
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the transaction was recovered.
        /// </summary>
        bool Recovered
        {
            get;
        }

        /// <summary>
        /// Gets the session start timestamp.
        /// </summary>
        DateTime StartTimestamp
        {
            get;
        }

        /// <summary>
        /// Gets the session id.
        /// </summary>
        Guid SessionId
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the transaction is cancelled.
        /// </summary>
        bool Cancelled
        {
            get;
        }
    }
}
