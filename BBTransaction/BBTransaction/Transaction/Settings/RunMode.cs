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
        /// Runs all steps from the specific step (set in settings) to the last one.
        /// </summary>
        RunFromStep,

        /// <summary>
        /// Recovers the transaction, runs undo operations for completed steps and then starts the transaction from the first step to the last one.
        /// </summary>
        RecoverAndUndoAndRun,

        /// <summary>
        /// Recovers the transaction and runs not completed steps.
        /// </summary>
        RecoverAndContinue
    }
}
