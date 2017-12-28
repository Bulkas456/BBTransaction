using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.State;

namespace BBTransaction.StateStorage
{
    /// <summary>
    /// The storage for a transaction state.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaciton data.</typeparam>
    public interface IStateStorage<TStepId, TData>
    {
        void NotifyTransactionStarted(ITransactionState<TStepId, TData> state);

        void NotifyTransactionEnded();

        void SaveTransactionData(ITransactionState<TStepId, TData> state);

        ITransactionState<TStepId, TData> RecoverState();

        void RemoveState(ITransactionState<TStepId, TData> state);
    }
}
