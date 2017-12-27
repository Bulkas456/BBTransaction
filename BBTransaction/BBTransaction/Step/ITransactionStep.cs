﻿using System;
using System.Collections.Generic;
using System.Text;
#if !NET35 
using System.Threading.Tasks;
#endif
using BBTransaction.Info;
using BBTransaction.Step.Executor;

namespace BBTransaction.Step
{
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
        /// Gets the step description.
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
        /// Gets the undo action for the step which will be invoked during transaction rollback.
        /// </summary>
        Action<TData, ITransactionInfo<TStepId>> UndoAction
        {
            get;
        }

#if !NET35
        /// <summary>
        /// Gets the undo action for the step which will be invoked during transaction rollback.
        /// </summary>
        Func<TData, ITransactionInfo<TStepId>, Task> AsyncUndoAction
        {
            get;
        }
#endif

        /// <summary>
        /// Gets the action which will be invoked after transaction success.
        /// </summary>
        Action<TData> PostAction
        {
            get;
        }

#if !NET35
        /// <summary>
        /// Gets the action which will be invoked after transaction success.
        /// </summary>
        Func<TData, Task> AsyncPostAction
        {
            get;
        }
#endif

        /// <summary>
        /// Gets a value indicating whether the step should be invoked when the transaction was recovered.
        /// </summary>
        bool RunOnRecovered
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the undo method for the step should be invoked when the step was recovered and is the first step to run. 
        /// </summary>
        bool UndoOnRecover
        {
            get;
        }

        /// <summary>
        /// Gets an executor for the step.
        /// </summary>
        IStepExecutor<TData> Executor
        {
            get;
        }
    }
}
