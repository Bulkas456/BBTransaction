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
using BBTransaction.Definition.Standard;
using BBTransaction.Definition.Standard.Context;
using BBTransaction.StateStorage;
using BBTransaction.StateStorage.Default;
using BBTransaction.Factory.Context.Part;

namespace BBTransaction.Factory
{
    public class TransactionFactory : ITransactionFactory
    {
        /// <summary>
        /// Creates a transaction.
        /// </summary>
        /// <typeparam name="TStepId">The type of the step id.</typeparam>
        /// <typeparam name="TData">The type of the transaciton data.</typeparam>
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
            ITransactionCreateInfo info = this.CreateTransactionInfo<TStepId, TData>(context);
            CreatePartContext<TStepId, TData> partContext = new CreatePartContext<TStepId, TData>()
            {
                Context = context,
                Logger = logger,
                Info = info
            };
            ITransactionDefinitionStorage<TStepId, TData> definition = this.CreateDefinition<TStepId, TData>(partContext);
            IStateStorage<TStepId, TData> stateStorage = this.CreateStateStorage<TStepId, TData>(partContext);

            TransactionContext<TStepId, TData> transactionContext = new TransactionContext<TStepId, TData>()
            {
                Logger = logger,
                Info = info,
                Definition = definition,
                StateStorage = stateStorage
            };
            return new Transaction<TStepId, TData>(transactionContext);
        }

        protected virtual ILogger CreateLogger<TStepId, TData>(ICreateTransactionContext<TStepId, TData> context)
        {
            return context.LoggerContext.Logger ?? new TransactionLogger(context.LoggerContext);
        }

        protected virtual ITransactionCreateInfo CreateTransactionInfo<TStepId, TData>(ICreateTransactionContext<TStepId, TData> context)
        {
            context.TransactionInfo.Validate();
            return new TransactionCreateInfo()
            {
                Name = context.TransactionInfo.Name,
                GetCurrentTimeFunction = context.TransactionInfo.GetCurrentTimeFunction ?? new Func<DateTime>(() => DateTime.Now)
            };
        }

        protected virtual ITransactionDefinitionStorage<TStepId, TData> CreateDefinition<TStepId, TData>(ICreatePartContext<TStepId, TData> context)
        {
            ITransactionDefinitionStorage<TStepId, TData> definition = context.Context.DefinitionCreator == null
                    ? new StandardTransactionDefinitionStorage<TStepId, TData>(new TransactionDefinitionContext()
                    {
                        Info = context.Info
                    })
                    : context.Context.DefinitionCreator(context);

            if (definition == null)
            {
                throw new InvalidOperationException(string.Format("Transaction '{0}': no definiton created from definition creator.", context.Info.Name));
            }

            return definition;
        }

        protected virtual IStateStorage<TStepId, TData> CreateStateStorage<TStepId, TData>(ICreatePartContext<TStepId, TData> context)
        {
            IStateStorage<TStepId, TData> stateStorage = context.Context.StateStorageCreator == null
                   ? DefaultStateStorage<TStepId, TData>.Instance
                   : context.Context.StateStorageCreator(context);

            if (stateStorage == null)
            {
                throw new InvalidOperationException(string.Format("Transaction '{0}': no state storage created from state storage creator.", context.Info.Name));
            }

            return stateStorage;
        }
    }
}
