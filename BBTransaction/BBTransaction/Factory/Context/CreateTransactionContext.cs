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
    public class CreateTransactionContext<TStepId, TData> : ICreateTransactionContext<TStepId, TData>
    {
        /// <summary>
        /// The logger context.
        /// </summary>
        private readonly LoggerContext loggerContext = new LoggerContext();

        /// <summary>
        /// The transaction info.
        /// </summary>
        private readonly TransactionInfoContext transactionInfo = new TransactionInfoContext();

        /// <summary>
        /// Gets or sets the definition for the transaction (optional).
        /// </summary>
        public ITransactionDefinitionStorage<TStepId, TData> Definition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the logger context.
        /// </summary>
        public LoggerContext LoggerContext => this.loggerContext;

        /// <summary>
        /// Gets the transaction info.
        /// </summary>
        public TransactionInfoContext TransactionInfo => this.transactionInfo;
    }
}
