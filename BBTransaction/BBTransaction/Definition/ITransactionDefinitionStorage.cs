using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Definition
{
    public interface ITransactionDefinitionStorage<TStepId, TData> : ITransactionDefinition<TStepId, TData>
    {

    }
}
