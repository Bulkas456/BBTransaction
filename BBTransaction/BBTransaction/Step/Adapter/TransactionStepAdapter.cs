using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Executor;
using BBTransaction.Step.Settings;
#if !NET35 && !NOASYNC 
using System.Threading.Tasks;
#endif
using BBTransaction.Transaction.Session.Info;

namespace BBTransaction.Step.Adapter
{
    /// <summary>
    /// The adapter for a transaction step.
    /// </summary>
    /// <typeparam name="TStepIdTo">The type of the destination step id.</typeparam>
    /// <typeparam name="TDataTo">The type of the destination data.</typeparam>
    /// <typeparam name="TStepIdFrom">The type of the original step id.</typeparam>
    /// <typeparam name="TDataFrom">The type of the source data.</typeparam>
    internal class TransactionStepAdapter<TStepIdTo, TDataTo, TStepIdFrom, TDataFrom> : ITransactionStep<TStepIdTo, TDataTo>
    {
        /// <summary>
        /// The orignal step.
        /// </summary>
        private readonly ITransactionStep<TStepIdFrom, TDataFrom> original;

        /// <summary>
        /// The step converter.
        /// </summary>
        private readonly Func<TStepIdFrom, TStepIdTo> stepConverter;

        /// <summary>
        /// The revers step converter.
        /// </summary>
        private readonly Func<TStepIdTo, TStepIdFrom> reverseStepConverter;

        /// <summary>
        /// The data converter.
        /// </summary>
        private readonly Func<TDataTo, TDataFrom> dataConverter;

        /// <summary>
        /// The step action for the adapter.
        /// </summary>
        private readonly Action<TDataTo, IStepTransactionSessionInfo<TStepIdTo>> stepAction;

        /// <summary>
        /// The undo action for the adapter.
        /// </summary>
        private readonly Action<TDataTo, IUndoTransactionSessionInfo<TStepIdTo>> undoAction;

        /// <summary>
        /// The post action for the adapter.
        /// </summary>
        private readonly Action<TDataTo, IPostTransactionSessionInfo<TStepIdTo>> postAction;

#if !NET35 && !NOASYNC
        /// <summary>
        /// The async step action for the adapter.
        /// </summary>
        private readonly Func<TDataTo, IStepTransactionSessionInfo<TStepIdTo>, Task> asyncStepAction;

        /// <summary>
        /// The async undo action for the adapter.
        /// </summary>
        private readonly Func<TDataTo, IUndoTransactionSessionInfo<TStepIdTo>, Task> asyncUndoAction;

        /// <summary>
        /// The async post action for the adapter.
        /// </summary>
        private readonly Func<TDataTo, IPostTransactionSessionInfo<TStepIdTo>, Task> asyncPostAction;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionStepAdapter<TStepIdTo, TDataTo, TStepIdFrom, TDataFrom>"/>class.
        /// </summary>
        /// <param name="original">The original step.</param>
        /// <param name="stepConverter">The step converter.</param>
        /// <param name="reverseStepConverter">The reversed step converter.</param>
        /// <param name="dataConverter">The data converter.</param>
        public TransactionStepAdapter(
            ITransactionStep<TStepIdFrom, TDataFrom> original,
            Func<TStepIdFrom, TStepIdTo> stepConverter,
            Func<TStepIdTo, TStepIdFrom> reverseStepConverter,
            Func<TDataTo, TDataFrom> dataConverter)
        {
            this.original = original ?? throw new ArgumentNullException(nameof(original));
            this.stepConverter = stepConverter ?? throw new ArgumentNullException(nameof(stepConverter));
            this.reverseStepConverter = reverseStepConverter ?? throw new ArgumentNullException(nameof(reverseStepConverter));
            this.dataConverter = dataConverter ?? throw new ArgumentNullException(nameof(dataConverter));
            this.stepAction = this.original.StepAction == null
                               ? null
                               : new Action<TDataTo, IStepTransactionSessionInfo<TStepIdTo>>((data, info) => this.original.StepAction(this.dataConverter(data), new TransactionSessionInfoAdapter<TStepIdTo, TStepIdFrom>(info, null, null, this.reverseStepConverter)));
            this.undoAction = this.original.UndoAction == null
                               ? null
                               : new Action<TDataTo, IUndoTransactionSessionInfo<TStepIdTo>>((data, info) => this.original.UndoAction(this.dataConverter(data), new TransactionSessionInfoAdapter<TStepIdTo, TStepIdFrom>(null, info, null, this.reverseStepConverter)));
            this.postAction = this.original.PostAction == null
                              ? null
                              : new Action<TDataTo, IPostTransactionSessionInfo<TStepIdTo>>((data, info) => this.original.PostAction(this.dataConverter(data), new TransactionSessionInfoAdapter<TStepIdTo, TStepIdFrom>(null, null, info, this.reverseStepConverter)));
#if !NET35 && !NOASYNC
            this.asyncStepAction = this.original.AsyncStepAction == null
                                     ? null
                                     : new Func<TDataTo, IStepTransactionSessionInfo<TStepIdTo>, Task>(async (data, info) => await this.original.AsyncStepAction(this.dataConverter(data), new TransactionSessionInfoAdapter<TStepIdTo, TStepIdFrom>(info, null, null, this.reverseStepConverter)));
            this.asyncUndoAction = this.original.AsyncUndoAction == null
                                     ? null
                                     : new Func<TDataTo, IUndoTransactionSessionInfo<TStepIdTo>, Task>(async (data, info) => await this.original.AsyncUndoAction(this.dataConverter(data), new TransactionSessionInfoAdapter<TStepIdTo, TStepIdFrom>(null, info, null, this.reverseStepConverter)));
            this.asyncPostAction = this.original.AsyncPostAction == null
                                     ? null
                                     : new  Func<TDataTo, IPostTransactionSessionInfo<TStepIdTo>, Task>(async (data, info) => await this.original.AsyncPostAction(this.dataConverter(data), new TransactionSessionInfoAdapter<TStepIdTo, TStepIdFrom>(null, null, info, this.reverseStepConverter)));
#endif
        }

        /// <summary>
        /// Gets the step id.
        /// </summary>
        public TStepIdTo Id => this.stepConverter(this.original.Id);

        /// <summary>
        /// Gets the step description (optional).
        /// </summary>
        public string Description => this.original.Description;

        /// <summary>
        /// Gets the action which will be invoked for the step.
        /// </summary>
        public Action<TDataTo, IStepTransactionSessionInfo<TStepIdTo>> StepAction => this.stepAction;

#if !NET35 && !NOASYNC
        /// <summary>
        /// Gets the action which will be invoked for the step.
        /// </summary>
        public Func<TDataTo, IStepTransactionSessionInfo<TStepIdTo>, Task> AsyncStepAction => this.asyncStepAction;
#endif

        /// <summary>
        /// Gets an executor for the step action (optional).
        /// </summary>
        public IExecutor StepActionExecutor => this.original.StepActionExecutor;

        /// <summary>
        /// Gets the undo action for the step which will be invoked during transaction rollback (optional).
        /// </summary>
        public Action<TDataTo, IUndoTransactionSessionInfo<TStepIdTo>> UndoAction => this.undoAction;

#if !NET35 && !NOASYNC
        /// <summary>
        /// Gets the undo action for the step which will be invoked during transaction rollback (optional).
        /// </summary>
        public Func<TDataTo, IUndoTransactionSessionInfo<TStepIdTo>, Task> AsyncUndoAction => this.asyncUndoAction;
#endif

        /// <summary>
        /// Gets an executor for the undo action (optional).
        /// </summary>
        public IExecutor UndoActionExecutor => this.original.UndoActionExecutor;

        /// <summary>
        /// Gets the action which will be invoked after transaction success (optional).
        /// </summary>
        public Action<TDataTo, IPostTransactionSessionInfo<TStepIdTo>> PostAction => this.postAction;

#if !NET35 && !NOASYNC
        /// <summary>
        /// Gets the action which will be invoked after transaction success (optional).
        /// </summary>
        public Func<TDataTo, IPostTransactionSessionInfo<TStepIdTo>, Task> AsyncPostAction => this.asyncPostAction;
#endif

        /// <summary>
        /// Gets an executor for the post action (optional).
        /// </summary>
        public IExecutor PostActionExecutor => this.original.PostActionExecutor;

        /// <summary>
        /// Gets the settings for the step.
        /// </summary>
        public StepSettings Settings => this.original.Settings;
    }
}
