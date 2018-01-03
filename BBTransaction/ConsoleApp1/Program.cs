using System;
using BBTransaction.Factory;
using BBTransaction.Factory.Context;
using BBTransaction.Factory.Context.Logger;
using BBTransaction.Step;
using BBTransaction;
using BBTransaction.Transaction.Session.Info;
using System.Threading.Tasks;
using BBTransaction.Step.Executor;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Factory.StartNew(async () =>
            {
                var transaction = new TransactionFactory().Create<int, string>(options =>
                {
                    options.TransactionInfo.Name = "elo";
                });
                transaction.Definition.Add(new TransactionStep<int, string>()
                {
                    Id = 1,
                    Settings = BBTransaction.Step.Settings.StepSettings.LogExecutionTime,
                    StepAction = Step1
                });
                transaction.Definition.Add(new TransactionStep<int, string>()
                {
                    Id = 2,
                    Settings = BBTransaction.Step.Settings.StepSettings.LogExecutionTime,
                    AsyncStepAction = Step2Async,
                    StepActionExecutor = new TestExecutor()
                });
                transaction.Definition.Add(new TransactionStep<int, string>()
                {
                    Id = 3,
                    Settings = BBTransaction.Step.Settings.StepSettings.LogExecutionTime,
                    StepAction = Step3
                });
                var result = await transaction.Run(settings =>
                {
                    settings.Mode = BBTransaction.Transaction.Settings.RunMode.Run;
                    settings.Data = "data";
                    settings.Settings = BBTransaction.Transaction.Settings.TransactionSettings.LogTimeExecutionForAllSteps;
                    settings.TransactionResultCallback = resultCallback =>
                    {
                    };
                });

                int a = 1;
            });

            Console.ReadKey();
        }

        private static void Step1(string data, ITransactionSessionInfo<int> info)
        {
        }

        private static async Task Step2Async(string data, ITransactionSessionInfo<int> info)
        {
        }

        private static void Step2(string data, ITransactionSessionInfo<int> info)
        {
        }

        private static void Step3(string data, ITransactionSessionInfo<int> info)
        {
        }

        internal class TestExecutor : IStepExecutor
        {
            public bool ShouldRun => true;

            public void Run(Func<Task> action)
            {
                ThreadPool.QueueUserWorkItem(async x =>
                {
                    Thread.Sleep(1000);
                    await action();
                    Thread.Sleep(2000);
                });
            }
        }
    }
}
