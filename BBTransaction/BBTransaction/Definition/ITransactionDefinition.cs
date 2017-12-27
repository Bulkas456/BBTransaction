using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Step;

namespace BBTransaction.Definition
{
    public interface ITransactionDefinition<TStepId, TData>
    {
        ITransactionDefinition<TStepId, TData> Add(ITransactionStep<TStepId, TData> step);
    }
}
