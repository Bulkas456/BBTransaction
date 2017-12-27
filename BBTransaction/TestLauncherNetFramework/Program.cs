using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBTransaction;
using BBTransaction.Factory;
using BBTransaction.Result;

namespace TestLauncherNetFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                ITransaction<Id, object> transaction = new TransactionFactory().Create<Id, object>();
                IOperationResult result = await transaction.RunAsync();
            })
            .GetAwaiter()
            .GetResult();
        }
    }
}
