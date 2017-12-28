using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Factory.Context.Info;

namespace BBTransaction.Info.Validator
{
    /// <summary>
    /// The validator for a transaction info.
    /// </summary>
    public static class TransactionInfoValidator
    {
        public static void Validate<TStepId>(this ITransactionInfoContext<TStepId> info)
        {
            if (string.IsNullOrEmpty(info.Name))
            {
                throw new ArgumentException(string.Format("'{0}': '{0}' cannot be null or empty.", typeof(ITransactionInfoContext<TStepId>).Name, nameof(info.Name)));
            }
        }
    }
}
