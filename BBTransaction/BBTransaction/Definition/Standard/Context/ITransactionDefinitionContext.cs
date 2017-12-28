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
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    public interface ITransactionDefinitionContext<TStepId>
    {
        /// <summary>
        /// Gets the transaction info.
        /// </summary>
        ITransactionCreateInfo<TStepId> Info
        {
            get;
        }
    }
}
