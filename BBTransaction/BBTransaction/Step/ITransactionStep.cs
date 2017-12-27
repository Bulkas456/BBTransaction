using System;
using System.Collections.Generic;
using System.Text;
#if !NET35 
using System.Threading.Tasks;
#endif
using BBTransaction.Info;
using BBTransaction.Step.Executor;
using BBTransaction.Step.Settings;

namespace BBTransaction.Step
{
    /// <summary>
    /// The step for the transaction.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaciton data.</typeparam>
    public interface ITransactionStep<TStepId, TData>
    {
        /// <summary>
        /// Gets the step id.
        /// </summary>
        TStepId Id
        {
            get;
        }

        /// <summary>
        /// Gets the step description (optional).
        /// </summary>
        string Description
        {
            get;
        }

        /// <summary>
        /// Gets the action which will be invoked for the step.
        /// </summary>
        Action<TData, ITransactionInfo<TStepId>> StepAction
        {
            get;
        }

#if !NET35
        /// <summary>
        /// Gets the action which will be invoked for the step.
        /// </summary>
        Func<TData, ITransactionInfo<TStepId>, Task> AsyncStepAction
        {
            get;
        }
#endif

        /// <summary>
        /// Gets the undo action for the step which will be invoked during transaction rollback (optional).
        /// </summary>
        Action<TData, ITransactionInfo<TStepId>> UndoAction
        {
            get;
        }

#if !NET35
        /// <summary>
        /// Gets the undo action for the step which will be invoked during transaction rollback (optional).
        /// </summary>
        Func<TData, ITransactionInfo<TStepId>, Task> AsyncUndoAction
        {
            get;
        }
#endif

        /// <summary>
        /// Gets the action which will be invoked after transaction success (optional).
        /// </summary>
        Action<TData> PostAction
        {
            get;
        }

#if !NET35
        /// <summary>
        /// Gets the action which will be invoked after transaction success (optional).
        /// </summary>
        Func<TData, Task> AsyncPostAction
        {
            get;
        }
#endif

        /// <summary>
        /// Gets the settings for the step.
        /// </summary>
        StepSettings Settings
        {
            get;
        }

        /// <summary>
        /// Gets an executor for the step (optional).
        /// </summary>
        IStepExecutor<TData> Executor
        {
            get;
        }
    }
}
