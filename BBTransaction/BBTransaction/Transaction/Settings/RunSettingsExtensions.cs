using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Transaction.Settings
{
    /// <summary>
    /// Extensions for the run settings.
    /// </summary>
    internal static class RunSettingsExtensions
    {
        /// <summary>
        /// Checks if the TransactionSettings.ProfileAllSteps setting is set.
        /// </summary>
        /// <typeparam name="TStepId">The type of the step id.</typeparam>
        /// <typeparam name="TData">The type of the transaciton data.</typeparam>
        /// <param name="settings">The run settings.</param>
        /// <returns><c>True</c> if the TransactionSettings.ProfileAllSteps is set, otherwise <c>false</c>.</returns>
        public static bool ProfileAllSteps<TStepId, TData>(this IRunSettings<TStepId, TData> settings)
        {
            return (settings.Settings & TransactionSettings.ProfileAllSteps) != 0;
        }
    }
}
