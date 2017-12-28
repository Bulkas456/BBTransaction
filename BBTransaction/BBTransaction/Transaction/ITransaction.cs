using System;
using System.Collections.Generic;
using System.Text;
#if !NET35 
using System.Threading.Tasks;
#endif
using BBTransaction.Definition;
using BBTransaction.Result;

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

        IOperationResult Run();

#if !NET35 
        Task<IOperationResult> RunAsync();
#endif
    }
}
