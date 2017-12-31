using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Definition;
using BBTransaction.Info;
using BBTransaction.Logger;
using BBTransaction.Transaction.Session.Storage;

namespace BBTransaction.Transaction.Context
{
    /// <summary>
    /// The context for the transaction.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaciton data.</typeparam>
    public interface ITransactionContext<TStepId, TData>
    {
        /// <summary>
        /// Gets the definition for the transaction.
        /// </summary>
        ITransactionDefinitionStorage<TStepId, TData> Definition
        {
            get;
        }

        /// <summary>
        /// Gets the state storage.
        /// </summary>
        ITransactionStorage<TStepId, TData> StateStorage
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
        ITransactionCreateInfo<TStepId> Info
        {
            get;
        }
    }
}
