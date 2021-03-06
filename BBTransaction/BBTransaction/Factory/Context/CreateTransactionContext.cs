﻿using System;
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
    public class CreateTransactionContext<TStepId, TData> : ICreateTransactionContext<TStepId, TData>
    {
        /// <summary>
        /// Gets or sets the transaction storage creator (optional).
        /// </summary>
        public Func<ICreatePartContext<TStepId, TData>, ITransactionStorage<TData>> TransactionStorageCreator
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the logger context.
        /// </summary>
        public ILoggerContext LoggerContext
        {
            get;
        } = new LoggerContext();

        /// <summary>
        /// Gets the transaction info.
        /// </summary>
        public ITransactionInfoContext<TStepId> TransactionInfo
        {
            get;
        } = new TransactionInfoContext<TStepId>();
    }
}
