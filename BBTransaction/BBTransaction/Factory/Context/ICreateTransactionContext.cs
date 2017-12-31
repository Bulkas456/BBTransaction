using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Definition;
using BBTransaction.Factory.Context.Info;
using BBTransaction.Factory.Context.Logger;
using BBTransaction.Factory.Context.Part;
using BBTransaction.Info;
using BBTransaction.Logger;
using BBTransaction.Transaction.Session.Storage;

namespace BBTransaction.Factory.Context
{
    /// <summary>
    /// The create transaction context.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    public interface ICreateTransactionContext<TStepId, TData>
    {
        /// <summary>
        /// Gets or sets the definition creator for the transaction (optional).
        /// </summary>
        Func<ICreatePartContext<TStepId, TData>, ITransactionDefinitionStorage<TStepId, TData>> DefinitionCreator
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the transaction storage creator (optional).
        /// </summary>
        Func<ICreatePartContext<TStepId, TData>, ITransactionStorage<TData>> TransactionStorageCreator
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the logger context.
        /// </summary>
        ILoggerContext LoggerContext
        {
            get;
        }

        /// <summary>
        /// Gets the transaction info.
        /// </summary>
        ITransactionInfoContext<TStepId> TransactionInfo
        {
            get;
        }
    }
}
