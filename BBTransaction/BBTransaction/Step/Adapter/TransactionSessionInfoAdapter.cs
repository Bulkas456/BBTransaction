﻿using System;
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
    internal struct TransactionSessionInfoAdapter<TStepIdFrom, TStepIdTo> : IStepTransactionSessionInfo<TStepIdTo>,
                                                                            IUndoTransactionSessionInfo<TStepIdTo>,
                                                                            IPostTransactionSessionInfo<TStepIdTo>
    {
        /// <summary>
        /// The main session info.
        /// </summary>
        private readonly ITransactionSessionInfo<TStepIdFrom> main;

        /// <summary>
        /// The session info for a step action.
        /// </summary>
        private readonly IStepTransactionSessionInfo<TStepIdFrom> infoForStep;

        /// <summary>
        /// The session info for an undo action.
        /// </summary>
        private readonly IUndoTransactionSessionInfo<TStepIdFrom> infoForUndo;

        /// <summary>
        /// The session info for a post action.
        /// </summary>
        private readonly IPostTransactionSessionInfo<TStepIdFrom> infoForPost;

        /// <summary>
        /// The step converter.
        /// </summary>
        private readonly Func<TStepIdTo, TStepIdFrom> stepConverter;

        /// <summary>
        /// The reverse step converter.
        /// </summary>
        private readonly Func<TStepIdFrom, TStepIdTo> revereseStepConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionSessionInfoAdapter<TStepIdFrom, TStepIdTo>"/> class.
        /// </summary>
        /// <param name="infoForStep">The session info for a step action.</param>
        /// <param name="infoForUndo">The session info for an undo action.</param>
        /// <param name="infoForPost">The session info for a post action.</param>
        /// <param name="stepConverter">The step converter.</param>
        /// <param name="reverseStepConverter">The reverse step converter.</param>
        public TransactionSessionInfoAdapter(
            IStepTransactionSessionInfo<TStepIdFrom> infoForStep,
            IUndoTransactionSessionInfo<TStepIdFrom> infoForUndo,
            IPostTransactionSessionInfo<TStepIdFrom> infoForPost,
            Func<TStepIdTo, TStepIdFrom> stepConverter,
            Func<TStepIdFrom, TStepIdTo> reverseStepConverter)
        {
            this.infoForStep = infoForStep;
            this.infoForUndo = infoForUndo;
            this.infoForPost = infoForPost;

            if (infoForStep != null)
            {
                this.main = infoForStep;
            }
            else if (infoForUndo != null)
            {
                this.main = infoForUndo;
            }
            else if (infoForPost != null)
            {
                this.main = infoForPost;
            }
            else
            {
                throw new ArgumentException(string.Format("At least one of values '{0}, '{1}', '{2}' should be not null.", nameof(infoForStep), nameof(infoForUndo), nameof(infoForPost)));
            }

            this.stepConverter = stepConverter ?? throw new ArgumentNullException(nameof(stepConverter));
            this.revereseStepConverter = reverseStepConverter ?? throw new ArgumentNullException(nameof(reverseStepConverter));
        }

        /// <summary>
        /// Gets the current step id.
        /// </summary>
        public TStepIdTo CurrentStepId => this.revereseStepConverter(this.main.CurrentStepId);

        /// <summary>
        /// Gets or sets a value indicating whether the transaction was recovered.
        /// </summary>
        public bool Recovered => this.main.Recovered;

        /// <summary>
        /// Gets the session start timestamp.
        /// </summary>
        public DateTime StartTimestamp => this.main.StartTimestamp;

        /// <summary>
        /// Gets the session id.
        /// </summary>
        public Guid SessionId => this.SessionId;

        /// <summary>
        /// Gets a value indicating whether the transaction is cancelled.
        /// </summary>
        public bool Cancelled => this.main.Cancelled;

        /// <summary>
        /// Cancels the transaction.
        /// </summary>
        public void Cancel()
        {
            this.infoForStep.Cancel();
        }

        /// <summary>
        /// Moves the transaction forward to a specific step. 
        /// </summary>
        /// <param name="id">The step id to move.</param>
        /// <param name="comparer">The equality comparer.</param>
        public void GoForward(TStepIdTo id, IEqualityComparer<TStepIdTo> comparer)
        {
            this.infoForStep.GoForward(this.stepConverter(id), comparer == null ? null : new EqualityComparerAdapter<TStepIdTo, TStepIdFrom>(comparer, this.revereseStepConverter));
        }

        /// <summary>
        /// Moves the transaction back to a specific step (all undo functions for the back steps will be executed). 
        /// </summary>
        /// <param name="id">The step id to move.</param>
        /// <param name="comparer">The equality comparer.</param>
        public void GoBack(TStepIdTo id, IEqualityComparer<TStepIdTo> comparer)
        {
            this.infoForStep.GoBack(this.stepConverter(id), comparer == null ? null : new EqualityComparerAdapter<TStepIdTo, TStepIdFrom>(comparer, this.revereseStepConverter));
        }
    }
}
