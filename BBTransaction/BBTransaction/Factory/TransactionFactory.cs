using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Factory.Context;
using BBTransaction.Info;
using BBTransaction.Logger;
using BBTransaction.Transaction;
using BBTransaction.Transaction.Context;
using BBTransaction.Info.Validator;
using BBTransaction.Definition;
using BBTransaction.Factory.Context.Part;
using BBTransaction.Transaction.Session.Storage;

namespace BBTransaction.Factory
{
    public class TransactionFactory : ITransactionFactory
    {
        /// <summary>
        /// Creates a transaction.
        /// </summary>
        /// <typeparam name="TStepId">The type of the step id.</typeparam>
        /// <typeparam name="TData">The type of the transaction data.</typeparam>
        /// <param name="options">The action to set options.</param>
        /// <returns>The transaction.</returns>
        public ITransaction<TStepId, TData> Create<TStepId, TData>(Action<ICreateTransactionContext<TStepId, TData>> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            ICreateTransactionContext<TStepId, TData> context = new CreateTransactionContext<TStepId, TData>();
            options(context);
            ILogger logger = this.CreateLogger<TStepId, TData>(context);
            ITransactionCreateInfo<TStepId> info = this.CreateTransactionInfo<TStepId, TData>(context);
            CreatePartContext<TStepId, TData> partContext = new CreatePartContext<TStepId, TData>()
            {
                Context = context,
                Logger = logger,
                Info = info
            };
            ITransactionDefinition<TStepId, TData> definition = this.CreateDefinition<TStepId, TData>(partContext);
            ITransactionStorage<TData> stateStorage = this.CreateStateStorage<TStepId, TData>(partContext);

            TransactionContext<TStepId, TData> transactionContext = new TransactionContext<TStepId, TData>()
            {
                Logger = logger,
                Info = info,
                Definition = definition,
                SessionStorage = stateStorage
            };
            return new Transaction<TStepId, TData>(transactionContext);
        }

        protected virtual ILogger CreateLogger<TStepId, TData>(ICreateTransactionContext<TStepId, TData> context)
        {
            return context.LoggerContext.Logger ?? new TransactionLogger(context.LoggerContext);
        }

        protected virtual ITransactionCreateInfo<TStepId> CreateTransactionInfo<TStepId, TData>(ICreateTransactionContext<TStepId, TData> context)
        {
            context.TransactionInfo.Validate();
            return new TransactionCreateInfo<TStepId>()
            {
                Name = context.TransactionInfo.Name,
                GetCurrentTimeFunction = context.TransactionInfo.GetCurrentTimeFunction ?? new Func<DateTime>(() => DateTime.Now),
                SessionIdCreator = context.TransactionInfo.SessionIdCreator
            };
        }

        protected virtual ITransactionDefinition<TStepId, TData> CreateDefinition<TStepId, TData>(ICreatePartContext<TStepId, TData> context)
        {
            return new TransactionDefinition<TStepId, TData>(context.Info);
        }

        protected virtual ITransactionStorage<TData> CreateStateStorage<TStepId, TData>(ICreatePartContext<TStepId, TData> context)
        {
            return context.Context.TransactionStorageCreator == null
                     ? null
                     : context.Context.TransactionStorageCreator(context);
        }
    }
}
