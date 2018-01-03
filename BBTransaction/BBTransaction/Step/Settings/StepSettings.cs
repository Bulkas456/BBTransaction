using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Step.Settings
{
    /// <summary>
    /// Settings for a transaction step.
    /// </summary>
    [Flags]
    public enum StepSettings
    {
        /// <summary>
        /// No specific settings.
        /// </summary>
        None = 0,

        /// <summary>
        /// The step should be invoked when the transaction was recovered. 
        /// </summary>
        RunOnRecovered = 1,

        /// <summary>
        /// The undo method for the step should be invoked when the step was recovered and is the first step to run.
        /// </summary>
        UndoOnRecover = 2,

        /// <summary>
        /// Time execution for the step method will be written to log.
        /// </summary>
        LogExecutionTime = 4,

        /// <summary>
        /// A step executor for the step action will be used for the undo and post actions.
        /// </summary>
        SameExecutorForAllActions = 8
    }
}
