using System;
using System.Threading.Tasks;
using TestLauncherNetCore;
using BBTransaction;
using BBTransaction.Factory;
using BBTransaction.Result;

namespace TestLauncher
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
