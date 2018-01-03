using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Transaction.Session;

namespace BBTransaction.Transaction.Operations
{
    /// <summary>
    /// The context for the session end operation.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    internal struct SessionEndContext<TStepId, TData>
    {
        /// <summary>
        /// Gets or sets the sesison to end.
        /// </summary>
        public ITransactionSession<TStepId, TData> Session
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to run post actions.
        /// </summary>
        public bool RunPostActions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the caught exception.
        /// </summary>
        public Exception CaughtException
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the additional info.
        /// </summary>
        public string AdditionalInfo
        {
            get;
            set;
        }
    }
}
