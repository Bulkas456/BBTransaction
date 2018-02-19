using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Transaction.TransactionResult;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;
using BBTransaction.Transaction.Session.Storage;
using BBTransaction.Factory;
using BBTransaction.Step;
using BBTransaction.Transaction;
using BBTransaction.Transaction.Settings;
using BBTransaction.Transaction.Session.Storage.TransactionData;
using System.Threading;
using System.Linq;

namespace BBTransactionTestsWithAsync
{
    [TestClass]
    public class MoveToStepBackTests
    {
        [TestMethod]
        public async Task WhenGoBackOnceWithoutComparer_ShouldMoveToStepProperly()
        {
            // Arrange
            object transactionData = new object();
            List<string> runStepActions = new List<string>();
            List<string> runUndoActions = new List<string>();
            List<string> runPostActions = new List<string>();
            Mock<ITransactionStorage<object>> storageMock = new Mock<ITransactionStorage<object>>();
            ITransaction<string, object> target = new TransactionFactory().Create<string, object>(options =>
            {
                options.TransactionInfo.Name = "test transaction";
                options.TransactionStorageCreator = partContext => storageMock.Object;
            });
            int numberOfGoBack = 0;

            for (int i = 0; i < 5; ++i)
            {
                string index = i.ToString();
                TransactionStep<string, object> step = new TransactionStep<string, object>()
                {
                    Id = index
                };

                if (i == 3)
                {
                    step.AsyncStepAction = async (data, info) =>
                    {
                        if (numberOfGoBack++ == 0)
                        {
                            info.GoBack("1");
                        }

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
                    };
                }

                if (i % 3 == 0)
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
                        info.CurrentStepId.Should().Be(index);
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
             3: go back to step 1
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
            result.Result.Should().Be(ResultType.Success);
            runStepActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3", "1", "2", "3", "4" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[] { "3", "2", "1" });
            runPostActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3", "4" });
            storageMock.Verify(x => x.SessionStarted(It.Is<ITransactionData<object>>(y => y.Data == transactionData)), Times.Once);
            storageMock.Verify(x => x.StepPrepared(It.Is<ITransactionData<object>>(y => y.Data == transactionData)), Times.Exactly(8));
            storageMock.Verify(x => x.StepReceding(It.IsAny<ITransactionData<object>>()), Times.Exactly(3));
            storageMock.Verify(x => x.RemoveSession(It.Is<ITransactionData<object>>(y => y.Data == transactionData)), Times.Once);
        }

        [TestMethod]
        public async Task WhenGoBackMoreTimes_ShouldMoveToStepProperly()
        {
            // Arrange
            object transactionData = new object();
            List<string> runStepActions = new List<string>();
            List<string> runUndoActions = new List<string>();
            List<string> runPostActions = new List<string>();
            Mock<ITransactionStorage<object>> storageMock = new Mock<ITransactionStorage<object>>();
            ITransaction<string, object> target = new TransactionFactory().Create<string, object>(options =>
            {
                options.TransactionInfo.Name = "test transaction";
                options.TransactionStorageCreator = partContext => storageMock.Object;
            });
            int step3BackCount = 0;
            int step4BackCount = 0;

            for (int i = 0; i < 6; ++i)
            {
                string index = i.ToString();
                TransactionStep<string, object> step = new TransactionStep<string, object>()
                {
                    Id = index
                };

                if (i == 1)
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
                        if (info.CurrentStepId == "3"
                            && step3BackCount++ < 2)
                        {
                            info.GoBack("1");
                        }

                        if (info.CurrentStepId == "4"
                            && step4BackCount++ < 1)
                        {
                            info.GoBack("2");
                        }

                        data.Should().BeSameAs(transactionData);
                        info.CurrentStepId.Should().Be(index);
                        runStepActions.Add(index);
                    };
                }

                if (i % 3 == 0)
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
                        info.CurrentStepId.Should().Be(index);
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
             3: go back to step 1 (two times)
             4: go back to step 2 (one time)
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
            result.Errors.ShouldAllBeEquivalentTo(new Exception[0]);
            result.Recovered.Should().BeFalse();
            result.Result.Should().Be(ResultType.Success);
            runStepActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3", "1", "2", "3", "1", "2", "3", "4", "2", "3", "4", "5" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[] { "3", "2", "1", "3", "2", "1", "4", "3", "2" });
            runPostActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3", "4", "5" });
            storageMock.Verify(x => x.SessionStarted(It.Is<ITransactionData<object>>(y => y.Data == transactionData)), Times.Once);
            storageMock.Verify(x => x.StepPrepared(It.Is<ITransactionData<object>>(y => y.Data == transactionData)), Times.Exactly(15));
            storageMock.Verify(x => x.StepReceding(It.IsAny<ITransactionData<object>>()), Times.Exactly(9));
            storageMock.Verify(x => x.RemoveSession(It.Is<ITransactionData<object>>(y => y.Data == transactionData)), Times.Once);
        }
    }
}
