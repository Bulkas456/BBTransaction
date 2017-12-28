using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.State;

namespace BBTransaction.StateStorage.Default
{
    /// <summary>
    /// The default state storage.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaciton data.</typeparam>
    internal class DefaultStateStorage<TStepId, TData> : IStateStorage<TStepId, TData>
    {
        public static readonly IStateStorage<TStepId, TData> Instance = new DefaultStateStorage<TStepId, TData>();

        public void NotifyTransactionEnded()
        {
        }

        public void NotifyTransactionStarted(ITransactionState<TStepId, TData> state)
        {
        }

        public ITransactionState<TStepId, TData> RecoverState()
        {
            return null;
        }

        public void RemoveState(ITransactionState<TStepId, TData> state)
        {
        }

        public void SaveTransactionData(ITransactionState<TStepId, TData> state)
        {
        }
    }
}
