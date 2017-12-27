using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Step;

namespace BBTransaction.Definition
{
    public static class TransactionDefinitionExtensions
    {
        public static ITransactionDefinition<TStepId, TData> Add<TStepId, TData>(this ITransactionDefinition<TStepId, TData> definition, IEnumerable<ITransactionStep<TStepId, TData>> steps)
        {
            foreach (ITransactionStep<TStepId, TData> step in steps)
            {
                definition.Add(step);
            }

            return definition;
        }
    }
}
