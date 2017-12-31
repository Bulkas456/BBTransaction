using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Factory.Context;
using BBTransaction.Transaction;

namespace BBTransaction.Factory
{
    /// <summary>
    /// The transaction factory.
    /// </summary>
    public interface ITransactionFactory
    {
        /// <summary>
        /// Creates a transaction.
        /// </summary>
        /// <typeparam name="TStepId">The type of the step id.</typeparam>
        /// <typeparam name="TData">The type of the transaction data.</typeparam>
        /// <param name="options">The action to set options.</param>
        /// <returns>The transaction.</returns>
        ITransaction<TStepId, TData> Create<TStepId, TData>(Action<ICreateTransactionContext<TStepId, TData>> options);
    }
}
