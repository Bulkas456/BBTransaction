using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Definition;

namespace BBTransaction.Factory.Context
{
    public interface ICreateTransactionContext<TStepId, TData>
    {
        /// <summary>
        /// Gets the definition for the transaction.
        /// </summary>
        ITransactionDefinition<TStepId, TData> Definition
        {
            get;
        }
    }
}
