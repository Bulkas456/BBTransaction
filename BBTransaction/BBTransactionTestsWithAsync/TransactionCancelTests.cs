using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BBTransaction.Factory;
using BBTransaction.Step;
using BBTransaction.Transaction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using BBTransaction.Transaction.TransactionResult;
using BBTransaction.Transaction.Settings;

namespace BBTransactionTestsWithAsync
{
    [TestClass]
    public class TransactionCancelTests
    {
        [TestMethod]
        public async Task WhenCancellTransaction_ShouldCancelProperly()
        {
            // Arrange
            object transactionData = new object();
            List<string> runStepActions = new List<string>();
            List<string> runUndoActions = new List<string>();
            List<string> runPostActions = new List<string>();
            ITransaction<string, object> target = new TransactionFactory().Create<string, object>(options =>
            {
                options.TransactionInfo.Name = "test transaction";
            });

            for (int i = 0; i < 5; ++i)
            {
                string index = i.ToString();
                TransactionStep<string, object> step = new TransactionStep<string, object>()
                {
                    Id = index
                };

                if (i % 3 == 0)
                {
                    step.AsyncStepAction = async (data, info) =>
                    {
                        data.Should().BeSameAs(transactionData);
                        info.CurrentStepId.Should().Be(index);
                        runStepActions.Add(index);

                        if (index == "3")
                        {
                            info.Cancel();
                        }

                        await Task.CompletedTask;
                    };
                }
                else
                {
                    step.StepAction = (data, info) =>
                    {
                        data.Should().BeSameAs(transactionData);
                        info.CurrentStepId.Should().Be(index);
                        runStepActions.Add(index);
                    };
                }

                if (i % 4 == 0)
                {
                    step.AsyncUndoAction = async (data, info) =>
                    {
                        data.Should().BeSameAs(transactionData);
                        info.CurrentStepId.Should().Be(index);
                        runUndoActions.Add(index);
                        await Task.CompletedTask;
                    };
                }
                else
                {
                    step.UndoAction = (data, info) =>
                    {
                        data.Should().BeSameAs(transactionData);
                        info.CurrentStepId.Should().Be(index);
                        runUndoActions.Add(index);
                    };
                }

                if (i % 2 == 0)
                {
                    step.AsyncPostAction = async (data) =>
                    {
                        data.Should().BeSameAs(transactionData);
                        runPostActions.Add(index);
                        await Task.CompletedTask;
                    };
                }
                else
                {
                    step.PostAction = (data) =>
                    {
                        data.Should().BeSameAs(transactionData);
                        runPostActions.Add(index);
                    };
                }

                target.Add(step);
            }
            ITransactionResult<object> transactionCallbackResult = null;

            /*
             0:
             1: 
             2:  
             3: cancel
             4:
             */

            // Act
            ITransactionResult<object> result = await target.Run(settings =>
            {
                settings.Data = transactionData;
                settings.Mode = RunMode.Run;
                settings.TransactionResultCallback = callbackResult => transactionCallbackResult = callbackResult;
            });

            // Assert
            result.Should().BeSameAs(transactionCallbackResult);
            result.Data.Should().BeSameAs(transactionData);
            result.Errors.ShouldAllBeEquivalentTo(new Exception[0]);
            result.Recovered.Should().BeFalse();
            result.Result.Should().Be(ResultType.Cancelled);
            runStepActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3"});
            runUndoActions.ShouldAllBeEquivalentTo(new string[] { "3", "2", "1", "0" });
            runPostActions.ShouldAllBeEquivalentTo(new string[0]);
        }
    }
}
