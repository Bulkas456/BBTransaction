using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Executor;
using BBTransaction.Transaction.TransactionResult;

namespace BBTransaction.Transaction.Settings
{
    /// <summary>
    /// The transaction run settings.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaction data.</typeparam>
    public interface IRunSettings<TStepId, TData>
    {
        /// <summary>
        /// Gets or sets the run mode (required).
        /// </summary>
        RunMode Mode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the transaction data (optional).
        /// </summary>
        TData Data
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the callback for the transaction result (optional).
        /// </summary>
        Action<ITransactionResult<TData>> TransactionResultCallback
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the executor for the callback for the transaction result (optional).
        /// </summary>
        IExecutor TransactionResultCallbackExecutor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the additional settings.
        /// </summary>
        TransactionSettings Settings
        {
            get;
            set;
        }
    }
}
