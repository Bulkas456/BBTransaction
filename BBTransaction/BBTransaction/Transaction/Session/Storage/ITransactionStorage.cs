using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Transaction.Session;
using BBTransaction.Transaction.Session.Storage.TransactionData;

namespace BBTransaction.Transaction.Session.Storage
{
    /// <summary>
    /// The storage for a transaction state.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaciton data.</typeparam>
    public interface ITransactionStorage<TStepId, TData>
    {
        void NotifyTransactionStarted(ITransactionData<TStepId, TData> info);

        void NotifyNextStep(ITransactionData<TStepId, TData> info);

        ITransactionData<TStepId, TData> RecoverTransaction();

        void RemoveTransaction(ITransactionData<TStepId, TData> info);
    }
}
