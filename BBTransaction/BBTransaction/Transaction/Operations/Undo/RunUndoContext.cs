using System;
using System.Collections.Generic;
using System.Text;
#if !NET35 && !NOASYNC
using System.Threading.Tasks;
#endif
using BBTransaction.Step;
using BBTransaction.Transaction.Session;
using BBTransaction.Transaction.TransactionResult;

namespace BBTransaction.Transaction.Operations.Undo
{
    /// <summary>
    /// The context for the undo process.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    internal class RunUndoContext<TStepId, TData>
    {
        private static readonly Func<ITransactionStep<TStepId, TData>, bool> DefaultProcessStepPredicate = step => true;

        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        public ITransactionSession<TStepId, TData> Session
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the caught exception before undo.
        /// </summary>
        public Exception CaughtException
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the transaction result.
        /// </summary>
        public ResultType Result
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the process step predicate.
        /// </summary>
        public Func<ITransactionStep<TStepId, TData>, bool> ProcessStepPredicate
        {
            get;
            set;
        } = DefaultProcessStepPredicate;

        /// <summary>
        /// Gets or sets a value indicating whether the session should not end after the undo process.
        /// </summary>
        public bool NoSessionEnd
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the undo finish action.
        /// </summary>
#if NET35 || NOASYNC
        public Action UndoFinishAction
        {
            get;
            set;
        } = () => { };
#else
        public Func<Task> UndoFinishAction
        {
            get;
            set;
        } = async () => await Task.FromResult<object>(null);
#endif
    }
}
