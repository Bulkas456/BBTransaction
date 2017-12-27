using System;
using System.Collections.Generic;
using System.Text;
#if !NET35 
using System.Threading.Tasks;
#endif
using BBTransaction.Definition;
using BBTransaction.Result;

namespace BBTransaction
{
    public interface ITransaction<TStepId, TData>
    {
        /// <summary>
        /// Gets the definition for the transaciton.
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
