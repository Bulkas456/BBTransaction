﻿using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Transaction.TransactionResult;

namespace BBTransaction.Transaction.Settings
{
    /// <summary>
    /// The transaction run settings.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaciton data.</typeparam>
    internal class RunSettings<TStepId, TData> : IRunSettings<TStepId, TData>
    {
        /// <summary>
        /// Gets or sets the run mode (required).
        /// </summary>
        public RunMode Mode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the transaction data (optional).
        /// </summary>
        public TData Data
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the first step to run (optional, ignored if mode not set to RunFromStep).
        /// </summary>
        public TStepId FirstStepId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the callback for the transaction result (optional).
        /// </summary>
        public Action<ITransactionResult<TData>> TransactionResultCallback
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the additional settings.
        /// </summary>
        public TransactionSettings Settings
        {
            get;
            set;
        }
    }
}
