using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Transaction.Session.Info;
using BBTransaction.Transaction.Session.Storage;
using BBTransaction.Transaction.Session.Storage.TransactionData;

namespace BBTransaction.Transaction.Session.Storage.Default
{
    /// <summary>
    /// The default state storage.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaciton data.</typeparam>
    internal class EmptyTransactionStorage<TStepId, TData> : ITransactionStorage<TStepId, TData>
    {
        public static readonly ITransactionStorage<TStepId, TData> Instance = new EmptyTransactionStorage<TStepId, TData>();

        public void NotifyNextStep(ITransactionData<TStepId, TData> info)
        {
        }

        public void NotifyTransactionStarted(ITransactionData<TStepId, TData> info)
        {
        }

        public ITransactionData<TStepId, TData> RecoverTransaction()
        {
            return null;
        }

        public void RemoveTransaction(ITransactionData<TStepId, TData> info)
        {
        }
    }
}
