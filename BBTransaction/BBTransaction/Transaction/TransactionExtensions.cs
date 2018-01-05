using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Step;

namespace BBTransaction.Transaction
{
    /// <summary>
    /// Extensions for transactions.
    /// </summary>
    public static class TransactionExtensions
    {
        /// <summary>
        /// Adds a collection of steps to the transaction.
        /// </summary>
        /// <param name="steps">The collection of steps to add.</param>
        /// <returns>The transaction.</returns>
        public static ITransaction<TStepId, TData> Add<TStepId, TData>(this ITransaction<TStepId, TData> transaction, IEnumerable<ITransactionStep<TStepId, TData>> steps)
        {
            foreach (ITransactionStep<TStepId, TData> step in steps)
            {
                transaction.Add(step);
            }

            return transaction;
        }
    }
}
