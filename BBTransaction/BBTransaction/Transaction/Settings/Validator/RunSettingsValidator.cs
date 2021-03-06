﻿using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Transaction.Context;

namespace BBTransaction.Transaction.Settings.Validator
{
    /// <summary>
    /// The validator for transaction run settings.
    /// </summary>
    public static class RunSettingsValidator
    {
        public static void Validate<TStepId, TData>(this IRunSettings<TStepId, TData> settings, ITransactionContext<TStepId, TData> transactionContext)
        {
            if (!Enum.IsDefined(typeof(RunMode), settings.Mode))
            {
                throw new ArgumentException(string.Format(
                    "Transaction '{0}': unknown run mode '{1}'.", 
                    transactionContext.Info.Name, 
                    settings.Mode));
            }
        }
    }
}
