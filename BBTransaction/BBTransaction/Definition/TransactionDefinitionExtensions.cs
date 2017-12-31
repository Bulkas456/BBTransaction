using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Definition;
using BBTransaction.Step;

namespace BBTransaction
{
    /// <summary>
    /// Extensions for transaction definition.
    /// </summary>
    public static class TransactionDefinitionExtensions
    {
        public static ITransactionDefinition<TStepId, TData> Add<TStepId, TData>(this ITransactionDefinition<TStepId, TData> definition, TransactionStep<TStepId, TData> step)
        {
            return definition.Add(step);
        }
    }
}
