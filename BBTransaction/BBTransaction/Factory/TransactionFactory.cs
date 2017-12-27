using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Factory.Context;

namespace BBTransaction.Factory
{
    public class TransactionFactory : ITransactionFactory
    {
        public ITransaction<TStepId, TData> Create<TStepId, TData>(ICreateTransactionContext<TStepId, TData> context)
        {
            return null;
        }
    }
}
