using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Factory.Context;
using BBTransaction.Transaction;

namespace BBTransaction.Factory
{
    public static class TransactionFactoryExtensions
    {
        public static ITransaction<TStepId, TData> Create<TStepId, TData>(this ITransactionFactory factory)
        {
            return factory.Create<TStepId, TData>(new CreateTransactionContext<TStepId, TData>()
            {
            });
        }
    }
}
