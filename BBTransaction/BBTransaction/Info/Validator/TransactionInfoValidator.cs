using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Info.Validator
{
    /// <summary>
    /// The validator for a transaction info.
    /// </summary>
    public static class TransactionInfoValidator
    {
        public static ITransactionInfo Validate(this ITransactionInfo info)
        {
            if (string.IsNullOrEmpty(info.Name))
            {
                throw new ArgumentException(string.Format("'{0}': '{0}' cannot be null or empty.", typeof(ITransactionInfo).Name, nameof(info.Name)));
            }

            return info;
        }
    }
}
