using System;
using BBTransaction.Factory;
using BBTransaction.Factory.Context;
using BBTransaction.Factory.Context.Logger;
using BBTransaction.Session;
using BBTransaction.Step;
using BBTransaction;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var transaction = new TransactionFactory().Create<int, string>(options => 
            {
            });
            transaction.Definition.Add(new TransactionStep<int, string>()
            {
                Id = 1,
                StepAction = (a, b) => { },
                Settings = BBTransaction.Step.Settings.StepSettings.LogExecutionTime
            });
            transaction.Run(settings =>
            {
                settings.Mode = BBTransaction.Transaction.Settings.RunMode.Run;
                settings.Data = "data";
                settings.Settings = BBTransaction.Transaction.Settings.TransactionSettings.LogTimeExecutionForAllSteps;
                settings.TransactionResultCallback = result => 
                {
                    int c = 1;
                };
            });

        }
    }
}
