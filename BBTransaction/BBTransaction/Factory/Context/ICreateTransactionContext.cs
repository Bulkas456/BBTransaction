using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Definition;
using BBTransaction.Info;
using BBTransaction.Logger;

namespace BBTransaction.Factory.Context
{
    /// <summary>
    /// The create transaction context.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaciton data.</typeparam>
    public interface ICreateTransactionContext<TStepId, TData>
    {
        /// <summary>
        /// Gets the definition for the transaction (optional).
        /// </summary>
        ITransactionDefinitionStorage<TStepId, TData> Definition
        {
            get;
        }

        /// <summary>
        /// Gets the logger context.
        /// </summary>
        LoggerContext LoggerContext
        {
            get;
        }

        /// <summary>
        /// Gets the transaction info.
        /// </summary>
        TransactionInfoContext TransactionInfo
        {
            get;
        }
    }
}
