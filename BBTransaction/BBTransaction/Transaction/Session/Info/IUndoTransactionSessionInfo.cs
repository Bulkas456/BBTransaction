using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Transaction.Session.Info
{
    /// <summary>
    /// The transaction session info for undo actions.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    public interface IUndoTransactionSessionInfo<TStepId> : ITransactionSessionInfo<TStepId>
    {
    }
}
