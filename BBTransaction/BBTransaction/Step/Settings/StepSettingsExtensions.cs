using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Step.Settings
{
    /// <summary>
    /// Extensions for step settings.
    /// </summary>
    public static class StepSettingsExtensions
    {
        /// <summary>
        /// Checks if the settings contain the 'RunOnRecovered' setting.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns><c>True</c> if the settings contain the 'RunOnRecovered' setting, otherwise <c>false</c>.</returns>
        public static bool RunOnRecovered(this StepSettings settings)
        {
            return (settings & StepSettings.RunOnRecovered) != 0;
        }

        /// <summary>
        /// Checks if the settings contain the 'UndoOnRecover' setting.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns><c>True</c> if the settings contain the 'UndoOnRecover' setting, otherwise <c>false</c>.</returns>
        public static bool UndoOnRecover(this StepSettings settings)
        {
            return (settings & StepSettings.UndoOnRecover) != 0;
        }

        /// <summary>
        /// Checks if the settings contain the 'LogExecutionTime' setting.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns><c>True</c> if the settings contain the 'LogExecutionTime' setting, otherwise <c>false</c>.</returns>
        public static bool LogExecutionTime(this StepSettings settings)
        {
            return (settings & StepSettings.LogExecutionTime) != 0;
        }

        /// <summary>
        /// Checks if the settings contain the 'SameExecutorForAllActions' setting.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns><c>True</c> if the settings contain the 'SameExecutorForAllActions' setting, otherwise <c>false</c>.</returns>
        public static bool SameExecutorForAllActions(this StepSettings settings)
        {
            return (settings & StepSettings.SameExecutorForAllActions) != 0;
        }
    }
}
