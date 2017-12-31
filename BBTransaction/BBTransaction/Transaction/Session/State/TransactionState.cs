﻿using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Transaction.Settings;

namespace BBTransaction.Transaction.Session.State
{
    /// <summary>
    /// The state for the transaction.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaciton data.</typeparam>
    internal class TransactionState<TStepId, TData> : ITransactionState<TStepId, TData>
    {
        /// <summary>
        /// Gets the index in the definition for the current step.
        /// </summary>
        public int CurrentStepIndex
        {
            get;
            set;
        }
    }
}
