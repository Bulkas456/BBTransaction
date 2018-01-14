using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Transaction.Session.Info;

namespace BBTransaction.Step.Adapter
{
    /// <summary>
    /// The adapter for a session info.
    /// </summary>
    /// <typeparam name="TStepIdFrom">The type of the original step id.</typeparam>
    /// <typeparam name="TStepIdTo">The type of the destination step id.</typeparam>
    internal struct TransactionSessionInfoAdapter<TStepIdFrom, TStepIdTo> : IStepTransactionSessionInfo<TStepIdTo>
    {
        /// <summary>
        /// The original session info.
        /// </summary>
        private readonly ITransactionSessionInfo<TStepIdFrom> original;

        /// <summary>
        /// The original session info for step.
        /// </summary>
        private readonly IStepTransactionSessionInfo<TStepIdFrom> originalForStep;

        /// <summary>
        /// The step converter.
        /// </summary>
        private readonly Func<TStepIdFrom, TStepIdTo> stepConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionSessionInfoAdapter<TStepIdFrom, TStepIdTo>"/> class.
        /// </summary>
        /// <param name="original">The original session info.</param>
        /// <param name="originalForStep">The original session info for step.</param>
        /// <param name="stepConverter">The step converter.</param>
        public TransactionSessionInfoAdapter(
            ITransactionSessionInfo<TStepIdFrom> original,
            IStepTransactionSessionInfo<TStepIdFrom> originalForStep,
            Func<TStepIdFrom, TStepIdTo> stepConverter)
        {
            this.original = original ?? throw new ArgumentNullException(nameof(original));
            this.originalForStep = originalForStep;
            this.stepConverter = stepConverter ?? throw new ArgumentNullException(nameof(stepConverter));
        }

        /// <summary>
        /// Gets the current step id.
        /// </summary>
        public TStepIdTo CurrentStepId => this.stepConverter(this.original.CurrentStepId);

        /// <summary>
        /// Gets or sets a value indicating whether the transaction was recovered.
        /// </summary>
        public bool Recovered => this.original.Recovered;

        /// <summary>
        /// Gets the session start timestamp.
        /// </summary>
        public DateTime StartTimestamp => this.original.StartTimestamp;

        /// <summary>
        /// Gets the session id.
        /// </summary>
        public Guid SessionId => this.SessionId;

        /// <summary>
        /// Cancels the transaction.
        /// </summary>
        public void Cancel()
        {
            this.originalForStep.Cancel();
        }
    }
}
