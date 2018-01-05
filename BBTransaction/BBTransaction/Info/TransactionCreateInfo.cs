using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Info
{
    /// <summary>
    /// The transaction create info.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    public class TransactionCreateInfo<TStepId> : ITransactionCreateInfo<TStepId>
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
        /// Gets the current time.
        /// </summary>
        public DateTime Now => this.GetCurrentTimeFunction();

        /// <summary>
        /// Gets or sets the unciton which return the current time.
        /// </summary>
        public Func<DateTime> GetCurrentTimeFunction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the session id creator (optional).
        /// </summary>
        public Func<Guid> SessionIdCreator
        {
            get;
            set;
        }
    }
}
