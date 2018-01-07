using System;
using System.Collections.Generic;
using System.Text;
#if !NET35 && !NOASYNC
using System.Threading.Tasks;
#endif

namespace BBTransaction.Executor
{
    /// <summary>
    /// The executor.
    /// </summary>
    public interface IExecutor
    {
        /// <summary>
        /// Gets a value indicating whether the action should be invoked on the executor.
        /// </summary>
        bool ShouldRun
        {
            get;
        }

        /// <summary>
        /// Runs the action.
        /// </summary>
        /// <param name="action">The action.</param>
#if NET35 || NOASYNC
        void Run(Action action);
#else
        void Run(Func<Task> action);
#endif
    }
}
