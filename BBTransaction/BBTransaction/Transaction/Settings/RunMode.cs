using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Transaction.Settings
{
    /// <summary>
    /// The transaction run mode.
    /// </summary>
    public enum RunMode
    {
        /// <summary>
        /// Runs all steps from the first one to the last one.
        /// </summary>
        Run,

        /// <summary>
        /// Recovers the transaction, runs undo operations for completed steps and then starts the transaction from the first step to the last one.
        /// If there is no session to recover the transaction is ended without run.
        /// </summary>
        RecoverAndUndoAndRun,

        /// <summary>
        /// Recovers the transaction and runs not completed steps.
        /// If there is no session to recover the transaction is ended without run.
        /// </summary>
        RecoverAndContinue
    }
}
