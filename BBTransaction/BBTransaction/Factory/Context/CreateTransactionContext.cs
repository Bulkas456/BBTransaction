﻿using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Definition;
using BBTransaction.Factory.Context.Part;
using BBTransaction.Info;
using BBTransaction.Logger;
using BBTransaction.StateStorage;

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
        /// Gets or sets the definition creator for the transaction (optional).
        /// </summary>
        public Func<ICreatePartContext<TStepId, TData>, ITransactionDefinitionStorage<TStepId, TData>> DefinitionCreator
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the state storage creator (optional).
        /// </summary>
        public Func<ICreatePartContext<TStepId, TData>, IStateStorage<TStepId, TData>> StateStorageCreator
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the logger context.
        /// </summary>
        public LoggerContext LoggerContext
        {
            get;
        } = new LoggerContext();

        /// <summary>
        /// Gets the transaction info.
        /// </summary>
        public TransactionInfoContext TransactionInfo
        {
            get;
        } = new TransactionInfoContext();
    }
}
