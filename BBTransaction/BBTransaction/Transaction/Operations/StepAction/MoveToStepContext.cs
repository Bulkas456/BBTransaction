using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Transaction.Session;
#if !NET35 && !NOASYNC
using System.Threading.Tasks;
#endif

namespace BBTransaction.Transaction.Operations.StepAction
{
    /// <summary>
    /// The context for the move to a step operation. 
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    internal class MoveToStepContext<TStepId, TData>
    {
        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        public ITransactionSession<TStepId, TData> Session
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the finish aciton.
        /// </summary>
#if NET35 || NOASYNC
        public Action MoveToStepFinishAction
        {
            get;
            set;
        }
#else
        public Func<Task> MoveToStepFinishAction
        {
            get;
            set;
        }
#endif
    }
}
