using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Transaction.Session.Info
{
    /// <summary>
    /// The transaciton session info.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    public interface ITransactionSessionInfo<TStepId>
    {
        /// <summary>
        /// Gets the current step id.
        /// </summary>
        TStepId CurrentStepId
        {
            get;
        }
    }
}
