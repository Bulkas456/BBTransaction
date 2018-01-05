using System;
using System.Collections.Generic;
using System.Text;
#if !NET35 && !NOASYNC
using System.Threading.Tasks;
#endif
using BBTransaction.Transaction.Session.Info;
using BBTransaction.Transaction.Session.Storage;
using BBTransaction.Transaction.Session.Storage.TransactionData;

namespace BBTransaction.Transaction.Session.Storage.Default
{
    /// <summary>
    /// The default state storage.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    internal class EmptyTransactionStorage<TData> : ITransactionStorage<TData>
    {
        public static readonly ITransactionStorage<TData> Instance = new EmptyTransactionStorage<TData>();

#if NET35 || NOASYNC
        public void SessionStarted(ITransactionData<TData> data)
        {
        }
#else
        public Task SessionStarted(ITransactionData<TData> data) => Task.FromResult<object>(null);
#endif

#if NET35 || NOASYNC
        public void StepPrepared(ITransactionData<TData> info)
        {
        }
#else
        public Task StepPrepared(ITransactionData<TData> data) => Task.FromResult<object>(null);
#endif


#if NET35 || NOASYNC
        public ITransactionData<TData> RecoverTransaction()
        {
            return null;
        }
#else
        public Task<ITransactionData<TData>> RecoverTransaction() => Task.FromResult<ITransactionData<TData>>(null);
#endif


#if NET35 || NOASYNC
        public void RemoveSession(ITransactionData<TData> data)
        {
        }
#else
        public Task RemoveSession(ITransactionData<TData> info) => Task.FromResult<object>(null);
#endif
    }
}
