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
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
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
        Action<TData, ITransactionSessionInfo<TStepId>> StepAction
        {
            get;
        }

#if !NET35
        /// <summary>
        /// Gets the action which will be invoked for the step.
        /// </summary>
        Func<TData, ITransactionSessionInfo<TStepId>, Task> AsyncStepAction
        {
            get;
        }
#endif

        /// <summary>
        /// Gets an executor for the step action (optional).
        /// </summary>
        IStepExecutor StepActionExecutor
        {
            get;
        }

        /// <summary>
        /// Gets the undo action for the step which will be invoked during transaction rollback (optional).
        /// </summary>
        Action<TData, ITransactionSessionInfo<TStepId>> UndoAction
        {
            get;
        }

#if !NET35
        /// <summary>
        /// Gets the undo action for the step which will be invoked during transaction rollback (optional).
        /// </summary>
        Func<TData, ITransactionSessionInfo<TStepId>, Task> AsyncUndoAction
        {
            get;
        }
#endif

        /// <summary>
        /// Gets an executor for the undo action (optional).
        /// </summary>
        IStepExecutor UndoActionExecutor
        {
            get;
        }

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
    }
}
