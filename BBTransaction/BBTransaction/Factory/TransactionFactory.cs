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

namespace BBTransaction.Factory
{
    public class TransactionFactory : ITransactionFactory
    {
        public ITransaction<TStepId, TData> Create<TStepId, TData>(ICreateTransactionContext<TStepId, TData> context)
        {
            ILogger logger = this.CreateLogger<TStepId, TData>(context);
            ITransactionInfo info = this.CreateTransactionInfo<TStepId, TData>(context);
            ITransactionDefinitionStorage<TStepId, TData> definition = this.CreateDefinition<TStepId, TData>(context, logger, info);

            TransactionContext<TStepId, TData> transactionContext = new TransactionContext<TStepId, TData>()
            {
                Logger = logger,
                Info = info,
                Definition = definition
            };
            return new Transaction<TStepId, TData>(transactionContext);
        }

        protected virtual ILogger CreateLogger<TStepId, TData>(ICreateTransactionContext<TStepId, TData> context)
        {
            return context.LoggerContext.Logger ?? new TransactionLogger(context.LoggerContext);
        }

        protected virtual ITransactionInfo CreateTransactionInfo<TStepId, TData>(ICreateTransactionContext<TStepId, TData> context)
        {
            return context.TransactionInfo.Validate();
        }

        protected virtual ITransactionDefinitionStorage<TStepId, TData> CreateDefinition<TStepId, TData>(ICreateTransactionContext<TStepId, TData> context, ILogger logger, ITransactionInfo info)
        {
            return context.Definition ?? new StandardTransactionDefinitionStorage<TStepId, TData>(new TransactionDefinitionContext()
            {
                Info = info
            });
        }
    }
}
