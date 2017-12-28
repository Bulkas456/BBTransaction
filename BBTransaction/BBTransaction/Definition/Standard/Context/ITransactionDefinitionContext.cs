using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Info;
using BBTransaction.Logger;

namespace BBTransaction.Definition.Standard.Context
{
    /// <summary>
    /// The transaction definition context.
    /// </summary>
    public interface ITransactionDefinitionContext
    {
        /// <summary>
        /// Gets the transaction info.
        /// </summary>
        ITransactionInfo Info
        {
            get;
        }
    }
}
