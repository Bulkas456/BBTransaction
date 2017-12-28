using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Definition;
using BBTransaction.Factory.Context.Logger;
using BBTransaction.Info;

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
        private readonly TransactionInfo transactionInfo = new TransactionInfo();

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
        public TransactionInfo TransactionInfo => this.transactionInfo;
    }
}
