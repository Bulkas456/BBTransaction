using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Info
{
    /// <summary>
    /// The transaction create info.
    /// </summary>
    public class TransactionCreateInfo : ITransactionCreateInfo
    {
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
    }
}
