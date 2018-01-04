using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Definition;
using BBTransaction.Transaction.Context;
using BBTransaction.Transaction.Settings;

namespace BBTransaction.Transaction.Session.StepEnumerator
{
    /// <summary>
    /// The state for the transaction.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    internal class StepEnumerator<TStepId, TData> : IStepEnumerator<TStepId, TData>
    {
        /// <summary>
        /// The minimum step index.
        /// </summary>
        private const int MinStepIndex = 0;

        /// <summary>
        /// The transaction session.
        /// </summary>
        private readonly ITransactionSession<TStepId, TData> session;

        /// <summary>
        /// Initilalizes a new instance of the <see cref="StepEnumerator<TStepId, TData>"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public StepEnumerator(ITransactionSession<TStepId, TData> session)
        {
            this.session = session;
        }

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
        /// Increments the state.
        /// </summary>
        public void Increment()
        {
            ++this.CurrentStepIndex;
            this.FillStep(); 
        }

        /// <summary>
        /// Decrements the state.
        /// </summary>
        public void Decrement()
        {
            this.CurrentStepIndex = Math.Max(MinStepIndex, this.CurrentStepIndex - 1);
            this.FillStep();
        }

        /// <summary>
        /// Restarts the enumerator.
        /// </summary>
        public void Restart()
        {
            this.CurrentStepIndex = MinStepIndex;
            this.FillStep();
        }

        /// <summary>
        /// Fills the current step.
        /// </summary>
        public void FillStep()
        {
            this.CurrentStep = this.session.TransactionContext.Definition.GetByIndex(this.CurrentStepIndex);
        }
    }
}
