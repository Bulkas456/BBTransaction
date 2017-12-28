using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Info;

namespace BBTransaction.Definition.Standard.Context
{
    /// <summary>
    /// The transaction definition context.
    /// </summary>
    public class TransactionDefinitionContext : ITransactionDefinitionContext
    {
        /// <summary>
        /// Gets or sets the transaction info (required).
        /// </summary>
        public ITransactionCreateInfo Info
        {
            get;
            set;
        }
    }
}
