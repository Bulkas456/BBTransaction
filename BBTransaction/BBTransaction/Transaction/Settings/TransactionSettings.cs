using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Transaction.Settings
{
    /// <summary>
    /// The transaction settings.
    /// </summary>
    [Flags]
    public enum TransactionSettings
    {
        /// <summary>
        /// No settings.
        /// </summary>
        None = 0,

        /// <summary>
        /// Time execution for each step method will be written to log.
        /// </summary>
        LogTimeExecutionForAllSteps = 1,

        /// <summary>
        /// A transaction data wouldn't be recovered and will be taken from the run settings.
        /// </summary>
        DontRecoverTransactionData = 2
    }
}
