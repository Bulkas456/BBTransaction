using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Transaction.Session.Info
{
    /// <summary>
    /// The transaction session info for a step.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    public interface IStepTransactionSessionInfo<TStepId> : ITransactionSessionInfo<TStepId>
    {
        /// <summary>
        /// Cancels the transaction.
        /// </summary>
        void Cancel();
    }
}
