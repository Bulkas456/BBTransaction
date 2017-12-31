using System;
using System.Collections.Generic;
using System.Text;
#if !NET35 
using System.Threading.Tasks;
#endif
using BBTransaction.Info;
using BBTransaction.Step.Executor;
using BBTransaction.Step.Settings;
using BBTransaction.Transaction.Session.Info;

namespace BBTransaction.Step
{
    /// <summary>
    /// The step for the transaction.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaciton data.</typeparam>
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
        public Action<TData, ITransactionSessionInfo<TStepId>> StepAction
        {
            get;
            set;
        }

#if !NET35
        /// <summary>
        /// Gets or sets the action which will be invoked for the step.
        /// </summary>
        public Func<TData, ITransactionSessionInfo<TStepId>, Task> AsyncStepAction
        {
            get;
            set;
        }
#endif

        /// <summary>
        /// Gets or sets the undo action for the step which will be invoked during transaction rollback (optional).
        /// </summary>
        public Action<TData, ITransactionSessionInfo<TStepId>> UndoAction
        {
            get;
            set;
        }

#if !NET35
        /// <summary>
        /// Gets or sets the undo action for the step which will be invoked during transaction rollback (optional).
        /// </summary>
        public Func<TData, ITransactionSessionInfo<TStepId>, Task> AsyncUndoAction
        {
            get;
            set;
        }
#endif

        /// <summary>
        /// Gets or sets the action which will be invoked after transaction success (optional).
        /// </summary>
        public Action<TData> PostAction
        {
            get;
            set;
        }

#if !NET35
        /// <summary>
        /// Gets or sets the action which will be invoked after transaction success (optional).
        /// </summary>
        public Func<TData, Task> AsyncPostAction
        {
            get;
            set;
        }
#endif

        /// <summary>
        /// Gets the settings for the step.
        /// </summary>
        public StepSettings Settings
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an executor for the step (optional).
        /// </summary>
        public IStepExecutor<TData> Executor
        {
            get;
            set;
        }
    }
}
