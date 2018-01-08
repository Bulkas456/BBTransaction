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
    internal struct TransactionSessionInfoAdapter<TStepIdFrom, TStepIdTo> : ITransactionSessionInfo<TStepIdTo>
    {
        /// <summary>
        /// The original session info.
        /// </summary>
        private readonly ITransactionSessionInfo<TStepIdFrom> original;

        /// <summary>
        /// The step converter.
        /// </summary>
        private readonly Func<TStepIdFrom, TStepIdTo> stepConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionSessionInfoAdapter<TStepIdFrom, TStepIdTo>"/> class.
        /// </summary>
        /// <param name="original">The original session info.</param>
        /// <param name="stepConverter">The step converter.</param>
        public TransactionSessionInfoAdapter(
            ITransactionSessionInfo<TStepIdFrom> original,
            Func<TStepIdFrom, TStepIdTo> stepConverter)
        {
            this.original = original ?? throw new ArgumentNullException(nameof(original));
            this.stepConverter = stepConverter ?? throw new ArgumentNullException(nameof(stepConverter));
        }

        /// <summary>
        /// Gets the current step id.
        /// </summary>
        public TStepIdTo CurrentStepId => this.stepConverter(this.original.CurrentStepId);
    }
}
