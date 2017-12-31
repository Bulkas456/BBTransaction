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
    public class TransactionContext<TStepId, TData> : ITransactionContext<TStepId, TData>
    {
        /// <summary>
        /// Gets the definition for the transaction.
        /// </summary>
        public ITransactionDefinitionStorage<TStepId, TData> Definition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the state storage.
        /// </summary>
        public ITransactionStorage<TStepId, TData> StateStorage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger Logger
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the transaction info.
        /// </summary>
        public ITransactionCreateInfo<TStepId> Info
        {
            get;
            set;
        }
    }
}
