using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Info;

namespace BBTransaction.Definition.Standard.Context
{
    /// <summary>
    /// The transaction definition context.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    public class TransactionDefinitionContext<TStepId> : ITransactionDefinitionContext<TStepId>
    {
        /// <summary>
        /// Gets or sets the transaction info (required).
        /// </summary>
        public ITransactionCreateInfo<TStepId> Info
        {
            get;
            set;
        }
    }
}
