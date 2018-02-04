using System;
using System.Collections.Generic;
using System.Text;
#if !NET35 && !NOASYNC 
using System.Threading.Tasks;
#endif
using BBTransaction.Info;
using BBTransaction.Executor;
using BBTransaction.Step.Settings;
using BBTransaction.Transaction.Session.Info;

namespace BBTransaction.Step
{
    /// <summary>
    /// The step for the transaction.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    public class TransactionStep<TStepId, TData> : ITransactionStep<TStepId, TData>
    {
        /// <summary>
        /// Gets or sets the step id.
        /// </summary>
        public TStepId Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the step description (optional).
        /// </summary>
        public string Description
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        /// Gets or sets the action which will be invoked for the step.
        /// </summary>
        public Action<TData, IStepTransactionSessionInfo<TStepId>> StepAction
        {
            get;
            set;
        }

#if !NET35 && !NOASYNC
        /// <summary>
        /// Gets or sets the action which will be invoked for the step.
        /// </summary>
        public Func<TData, IStepTransactionSessionInfo<TStepId>, Task> AsyncStepAction
        {
            get;
            set;
        }
#endif

        /// <summary>
        /// Gets or sets an executor for the step action (optional).
        /// </summary>
        public IExecutor StepActionExecutor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the undo action for the step which will be invoked during transaction rollback (optional).
        /// </summary>
        public Action<TData, IUndoTransactionSessionInfo<TStepId>> UndoAction
        {
            get;
            set;
        }

#if !NET35 && !NOASYNC
        /// <summary>
        /// Gets or sets the undo action for the step which will be invoked during transaction rollback (optional).
        /// </summary>
        public Func<TData, IUndoTransactionSessionInfo<TStepId>, Task> AsyncUndoAction
        {
            get;
            set;
        }
#endif

        /// <summary>
        /// Gets an executor for the undo action (optional).
        /// </summary>
        public IExecutor UndoActionExecutor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the action which will be invoked after transaction success (optional).
        /// </summary>
        public Action<TData, IPostTransactionSessionInfo<TStepId>> PostAction
        {
            get;
            set;
        }

#if !NET35 && !NOASYNC
        /// <summary>
        /// Gets or sets the action which will be invoked after transaction success (optional).
        /// </summary>
        public Func<TData, IPostTransactionSessionInfo<TStepId>, Task> AsyncPostAction
        {
            get;
            set;
        }
#endif

        /// <summary>
        /// Gets an executor for the post action (optional).
        /// </summary>
        public IExecutor PostActionExecutor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the settings for the step.
        /// </summary>
        public StepSettings Settings
        {
            get;
            set;
        }
    }
}
