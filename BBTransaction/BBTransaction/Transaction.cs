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
    public class Transaction<TStepId, TData> : ITransaction<TStepId, TData>
    {
        public ITransactionDefinition<TStepId, TData> Definition => throw new NotImplementedException();

        public IOperationResult Run()
        {
            throw new NotImplementedException();
        }

#if !NET35
        public Task<IOperationResult> RunAsync()
        {
            throw new NotImplementedException();
        }
#endif
    }
}
