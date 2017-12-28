using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Info;
using BBTransaction.Logger;

namespace BBTransaction.Factory.Context.Part
{
    /// <summary>
    /// The create part of transaction core context.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaciton data.</typeparam>
    public interface ICreatePartContext<TStepId, TData>
    {
        /// <summary>
        /// Gets the create transaction context.
        /// </summary>
        ICreateTransactionContext<TStepId, TData> Context
        {
            get;
        } 
        
        /// <summary>
        /// Gets the logger.
        /// </summary>
        ILogger Logger
        {
            get;
        }
        
        /// <summary>
        /// Gets the transaction info.
        /// </summary>
        ITransactionCreateInfo Info
        {
            get;
        }
    }
}
