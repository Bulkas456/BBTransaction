using System;
using System.Collections.Generic;
using System.Text;
#if !NET35 
using System.Threading.Tasks;
#endif
using BBTransaction.Info;
using BBTransaction.Step.Executor;

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
        public Action<TData, ITransactionInfo<TStepId>> StepAction
        {
            get;
            set;
        }

#if !NET35
        /// <summary>
        /// Gets or sets the action which will be invoked for the step.
        /// </summary>
        public Func<TData, ITransactionInfo<TStepId>, Task> AsyncStepAction
        {
            get;
            set;
        }
#endif

        /// <summary>
        /// Gets or sets the undo action for the step which will be invoked during transaction rollback (optional).
        /// </summary>
        public Action<TData, ITransactionInfo<TStepId>> UndoAction
        {
            get;
            set;
        }

#if !NET35
        /// <summary>
        /// Gets or sets the undo action for the step which will be invoked during transaction rollback (optional).
        /// </summary>
        public Func<TData, ITransactionInfo<TStepId>, Task> AsyncUndoAction
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
        /// Gets or sets a value indicating whether the step should be invoked when the transaction was recovered.
        /// </summary>
        public bool RunOnRecovered
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the undo method for the step should be invoked when the step was recovered and is the first step to run. 
        /// </summary>
        public bool UndoOnRecover
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
