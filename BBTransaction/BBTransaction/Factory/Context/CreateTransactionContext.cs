using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Definition;

namespace BBTransaction.Factory.Context
{
    public class CreateTransactionContext<TStepId, TData> : ICreateTransactionContext<TStepId, TData>
    {
        /// <summary>
        /// Gets or sets the definition for the transaction.
        /// </summary>
        public ITransactionDefinition<TStepId, TData> Definition
        {
            get;
            set;
        }
    }
}
