using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Factory.Context;

namespace BBTransaction.Factory
{
    public interface ITransactionFactory
    {
        ITransaction<TStepId, TData> Create<TStepId, TData>(ICreateTransactionContext<TStepId, TData> context);
    }
}
