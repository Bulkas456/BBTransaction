using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Factory.Context;

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
        /// <typeparam name="TData">The type of the transaciton data.</typeparam>
        /// <param name="context">The create context.</param>
        /// <returns>The transaction.</returns>
        ITransaction<TStepId, TData> Create<TStepId, TData>(ICreateTransactionContext<TStepId, TData> context);
    }
}
