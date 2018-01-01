using System;
using System.Collections.Generic;
using System.Text;
#if !NET35
using System.Threading.Tasks;
#endif

namespace BBTransaction.Step.Executor
{
    /// <summary>
    /// The step executor.
    /// </summary>
    public interface IStepExecutor
    {
        /// <summary>
        /// Gets a value indicating whether the step processing action should be invoked on the executor.
        /// </summary>
        bool ShouldRun
        {
            get;
        }

        /// <summary>
        /// Runs the step processing action.
        /// </summary>
        /// <param name="action">The action.</param>
#if NET35
        void Run(Action action);
#else
        void Run(Func<Task> action);
#endif
    }
}
