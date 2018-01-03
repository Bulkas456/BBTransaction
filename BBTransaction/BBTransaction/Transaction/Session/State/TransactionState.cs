using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Definition;
using BBTransaction.Transaction.Context;
using BBTransaction.Transaction.Settings;

namespace BBTransaction.Transaction.Session.State
{
    /// <summary>
    /// The state for the transaction.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    internal class TransactionState<TStepId, TData> : ITransactionState<TStepId, TData>
    {
        /// <summary>
        /// The transaction context.
        /// </summary>
        private ITransactionContext<TStepId, TData> transactionContext;

        /// <summary>
        /// Gets the index in the definition for the current step.
        /// </summary>
        public int CurrentStepIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the data for the transaction.
        /// </summary>
        public TData Data
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current step.
        /// </summary>
        public IStepDetails<TStepId, TData> CurrentStep
        {
            get;
            set;
        }

        /// <summary>
        /// Gets oir sets the transaction context.
        /// </summary>
        public ITransactionContext<TStepId, TData> TransactionContext
        {
            get
            {
                return this.transactionContext;
            }

            set
            {
                this.transactionContext = value;
                this.CurrentStep = this.transactionContext.Definition[this];
            }
        }

        /// <summary>
        /// Increments the state.
        /// </summary>
        public void Increment()
        {
           ++this.CurrentStepIndex;
            this.CurrentStep = this.transactionContext.Definition[this];
        }

        /// <summary>
        /// Decrements the state.
        /// </summary>
        public void Decrement()
        {
            this.CurrentStepIndex = Math.Max(0, this.CurrentStepIndex - 1);
            this.CurrentStep = this.transactionContext.Definition[this];
        }
    }
}
