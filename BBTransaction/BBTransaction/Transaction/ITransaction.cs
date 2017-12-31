using System;
using System.Collections.Generic;
using System.Text;
#if !NET35
using System.Threading.Tasks;
#endif
using BBTransaction.Definition;
using BBTransaction.Result;
using BBTransaction.Transaction.Settings;
using BBTransaction.Transaction.TransactionResult;

namespace BBTransaction.Transaction
{
    /// <summary>
    /// The transaction.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaciton data.</typeparam>
    public interface ITransaction<TStepId, TData>
    {
        /// <summary>
        /// Gets the definition for the transaction.
        /// </summary>
        ITransactionDefinition<TStepId, TData> Definition
        {
            get;
        }

#if NET35
        /// <summary>
        /// Runs the transaction.
        /// </summary>
        /// <param name="settings">The action to set settings.</param>
        void Run(Action<IRunSettings<TStepId, TData>> settings);
#else
        Task<ITransactionResult<TData>> Run(Action<IRunSettings<TStepId, TData>> settings);
#endif
    }
}
