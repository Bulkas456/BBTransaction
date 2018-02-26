using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Threading.Tasks;
using BBTransaction.Transaction;
using BBTransaction.Factory;
using BBTransaction.Step;
using BBTransaction.Transaction.TransactionResult;
using BBTransaction.Transaction.Settings;
using BBTransaction.Transaction.Session.Storage;
using Moq;
using BBTransaction.Transaction.Session.Storage.TransactionData;

namespace BBTransactionTestsWithAsync
{
    [TestClass]
    public class ProcessingUndoTests
    {
        [TestMethod]
        public async Task WhenRunTransactionAndErrorOccurredInNonAsyncStepAction_ShouldRunUndoSteps()
        {
            // Arrange
            object transactionData = new object();
            List<string> runStepActions = new List<string>();
            List<string> runUndoActions = new List<string>();
            List<string> runPostActions = new List<string>();
            Exception testException = new Exception("test exception");
            Mock<ITransactionStorage<object>> storageMock = new Mock<ITransactionStorage<object>>();
            ITransaction<string, object> target = new TransactionFactory().Create<string, object>(options =>
            {
                options.TransactionInfo.Name = "test transaction";
                options.TransactionStorageCreator = partContext => storageMock.Object;
            });

            for (int i = 0; i < 6; ++i)
            {
                string index = i.ToString();
                TransactionStep<string, object> step = new TransactionStep<string, object>()
                {
                    Id = index
                };

                if (i % 2 == 0)
                {
                    step.AsyncStepAction = async (data, info) =>
                    {
                        data.Should().BeSameAs(transactionData);
                        info.CurrentStepId.Should().Be(index);
                        runStepActions.Add(index);
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

                        if (index == "3")
                        {
                            throw testException; 
                        }
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

                if (i % 3 == 0)
                {
                    step.AsyncPostAction = async (data, info) =>
                    {
                        data.Should().BeSameAs(transactionData);
                        runPostActions.Add(index);
                        info.CurrentStepId.Should().Be(index);
                        await Task.CompletedTask;
                    };
                }
                else
                {
                    step.PostAction = (data, info) =>
                    {
                        data.Should().BeSameAs(transactionData);
                        runPostActions.Add(index);
                        info.CurrentStepId.Should().Be(index);
                    };
                }

                target.Add(step);
            }
            ITransactionResult<object> transactionCallbackResult = null;

            /*
             0:
             1: 
             2:  
             3: error 
             4:
             5:
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
            result.Errors.ShouldAllBeEquivalentTo(new Exception[] { testException });
            result.Recovered.Should().BeFalse();
            result.Result.Should().Be(ResultType.Failed);
            runStepActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[] { "3", "2", "1", "0" });
            runPostActions.ShouldAllBeEquivalentTo(new string[0]);
            storageMock.AssertStorageOperations(new AssertStorageOperationsContext<string, object>()
            {
                TransactionData = transactionData,
                Transaction = target,
                ExpectedStepsOrder = runStepActions,
                ExpectedUndoOrder = runUndoActions
            });
        }

        [TestMethod]
        public async Task WhenRunTransactionAndErrorOccurredInAsyncStepAction_ShouldRunUndoSteps()
        {
            // Arrange
            object transactionData = new object();
            List<string> runStepActions = new List<string>();
            List<string> runUndoActions = new List<string>();
            List<string> runPostActions = new List<string>();
            Exception testException = new Exception("test exception");
            ITransaction<string, object> target = new TransactionFactory().Create<string, object>(options =>
            {
                options.TransactionInfo.Name = "test transaction";
            });

            for (int i = 0; i < 6; ++i)
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
                            throw testException;
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
                    step.AsyncPostAction = async (data, info) =>
                    {
                        data.Should().BeSameAs(transactionData);
                        runPostActions.Add(index);
                        info.CurrentStepId.Should().Be(index);
                        await Task.CompletedTask;
                    };
                }
                else
                {
                    step.PostAction = (data, info) =>
                    {
                        data.Should().BeSameAs(transactionData);
                        runPostActions.Add(index);
                        info.CurrentStepId.Should().Be(index);
                    };
                }

                target.Add(step);
            }
            ITransactionResult<object> transactionCallbackResult = null;

            /*
             0:
             1: 
             2:  
             3: error 
             4:
             5:
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
            result.Errors.ShouldAllBeEquivalentTo(new Exception[] { testException });
            result.Recovered.Should().BeFalse();
            result.Result.Should().Be(ResultType.Failed);
            runStepActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[] { "3", "2", "1", "0" });
            runPostActions.ShouldAllBeEquivalentTo(new string[0]);
        }
    }
}
