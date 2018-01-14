using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Transaction.Session;
using BBTransaction.Transaction.TransactionResult;

namespace BBTransaction.Transaction.Operations.SessionEnd
{
    /// <summary>
    /// The context for the session end operation.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    internal class SessionEndContext<TStepId, TData>
    {
        /// <summary>
        /// The collection of caught exceptions.
        /// </summary>
        private readonly List<Exception> caughtExceptions = new List<Exception>();

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
        /// Gets the list of caught exceptions.
        /// </summary>
        public IEnumerable<Exception> CaughtExceptions => this.caughtExceptions;

        /// <summary>
        /// Gets or sets the transaction result.
        /// </summary>
        public ResultType Result
        {
            get;
            set;
        }

        public SessionEndContext<TStepId, TData> AddError(Exception error)
        {
            this.caughtExceptions.Add(error);
            return this;
        }
    }
}
