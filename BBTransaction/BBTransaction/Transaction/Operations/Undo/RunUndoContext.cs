using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Transaction.Session;
using BBTransaction.Transaction.TransactionResult;

namespace BBTransaction.Transaction.Operations.Undo
{
    /// <summary>
    /// The context for the undo process.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    internal struct RunUndoContext<TStepId, TData>
    {
        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        public ITransactionSession<TStepId, TData> Session
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the caught exception before undo.
        /// </summary>
        public Exception CaughtException
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the transaction result.
        /// </summary>
        public ResultType Result
        {
            get;
            set;
        }
    }
}
