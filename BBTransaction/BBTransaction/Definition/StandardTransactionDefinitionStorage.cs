using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Step;
using BBTransaction.Step.Validator;

namespace BBTransaction.Definition
{
    internal class StandardTransactionDefinitionStorage<TStepId, TData> : ITransactionDefinition<TStepId, TData>
    {
        public ITransactionDefinition<TStepId, TData> Add(ITransactionStep<TStepId, TData> step)
        {
            step.Validate();
            throw new NotImplementedException();
        }
    }
}
