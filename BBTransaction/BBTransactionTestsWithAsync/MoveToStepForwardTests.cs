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
    public class MoveToStepForwardTests
    {
        [TestMethod]
        public async Task WhenGoForwardWithoutComparer_ShouldMoveToStepProperly()
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

            for (int i = 0; i < 5; ++i)
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
                        info.GoForward("4");
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
             1: go forward to step 4
             2:  
             3:
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
            runStepActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "4" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[0]);
            runPostActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3", "4" });
            storageMock.AssertStorageOperations(new AssertStorageOperationsContext<string, object>()
            {
                TransactionData = transactionData,
                Transaction = target,
                ExpectedStepsOrder = runStepActions,
                ExpectedUndoOrder = runUndoActions
            });
        }

        [TestMethod]
        public async Task WhenGoForwardMoreTimes_ShouldMoveToStepProperly()
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

                        if (info.CurrentStepId == "1")
                        {
                            info.GoForward("3");
                        }

                        await Task.CompletedTask;
                    };
                }
                else
                {
                    step.StepAction = (data, info) =>
                    {
                        if (info.CurrentStepId == "3")
                        {
                            info.GoForward("5");
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
             1: go forward to step 3
             2:  
             3: go dorward to step 5
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
            result.Errors.ShouldAllBeEquivalentTo(new Exception[0]);
            result.Recovered.Should().BeFalse();
            result.Result.Should().Be(ResultType.Success);
            runStepActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "3", "5" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[0]);
            runPostActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3", "4", "5" });
            storageMock.AssertStorageOperations(new AssertStorageOperationsContext<string, object>()
            {
                TransactionData = transactionData,
                Transaction = target,
                ExpectedStepsOrder = runStepActions,
                ExpectedUndoOrder = runUndoActions
            });
        }

        [TestMethod]
        public async Task WhenGoForwardWithComparerWithCompareSuccess_ShouldMoveToStepProperly()
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

            for (int i = 0; i < 5; ++i)
            {
                string index = "A" + i.ToString();
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
                        info.GoForward("a4", StringComparer.OrdinalIgnoreCase);
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
             1: go forward to step 4
             2:  
             3:
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
            runStepActions.ShouldAllBeEquivalentTo(new string[] { "A0", "A1", "A4" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[0]);
            runPostActions.ShouldAllBeEquivalentTo(new string[] { "A0", "A1", "A2", "A3", "A4" });
            storageMock.AssertStorageOperations(new AssertStorageOperationsContext<string, object>()
            {
                TransactionData = transactionData,
                Transaction = target,
                ExpectedStepsOrder = runStepActions,
                ExpectedUndoOrder = runUndoActions
            });
        }

        [TestMethod]
        public async Task WhenGoForwardWithComparerWithCompareFailure_ShouldFailTransaction()
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

            for (int i = 0; i < 5; ++i)
            {
                string index = "A" + i.ToString();
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
                        info.GoForward("a4", StringComparer.Ordinal);
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
             1: go forward to step 4
             2:  
             3:
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
            result.Errors.Count().Should().Be(1);
            result.Errors.First().Message.Contains("Could not move forward to a step with id 'a4' as the step does not exist.").Should().BeTrue();
            result.Recovered.Should().BeFalse();
            result.Result.Should().Be(ResultType.Failed);
            runStepActions.ShouldAllBeEquivalentTo(new string[] { "A0", "A1" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[] { "A1", "A0" });
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
        public async Task WhenGoForwardWithoutComparerWithStepExecutors_ShouldMoveToStepProperly()
        {
            // Arrange
            object transactionData = new object();
            List<string> runStepActions = new List<string>();
            List<string> runUndoActions = new List<string>();
            List<string> runPostActions = new List<string>();
            List<int> stepActionThreadId = new List<int>();
            List<int> postActionThreadId = new List<int>();
            int transactionCallbackThreadId = 0;
            Dictionary<int, TestExecutor> stepExecutors = new Dictionary<int, TestExecutor>()
            {
                { 0, null },
                { 1, new TestExecutor() { ShouldRun = true } },
                { 2, new TestExecutor() { ShouldRun = true } },
                { 3, new TestExecutor() { ShouldRun = true } },
                { 4, new TestExecutor() { ShouldRun = true } },
                { 5, null },
                { 6, null }
            };
            Dictionary<int, TestExecutor> postExecutors = new Dictionary<int, TestExecutor>()
            {
                { 0, null },
                { 1, null },
                { 2, new TestExecutor() { ShouldRun = true } },
                { 3, new TestExecutor() { ShouldRun = true } },
                { 4, null },
                { 5, null },
                { 6, null }
            };
            int transactionRunThreadId = Thread.CurrentThread.ManagedThreadId;
            TestExecutor callBackExecutor = new TestExecutor() { ShouldRun = true };
            ITransaction<string, object> target = new TransactionFactory().Create<string, object>(options =>
            {
                options.TransactionInfo.Name = "test transaction";
            });

            for (int i = 0; i < 7; ++i)
            {
                string index = i.ToString();
                TransactionStep<string, object> step = new TransactionStep<string, object>()
                {
                    Id = index,
                    StepActionExecutor = stepExecutors[i] == null
                                           ? null
                                           : stepExecutors[i],
                    PostActionExecutor = postExecutors[i] == null
                                           ? null
                                           : postExecutors[i]
                };

                if (i != 2 )
                {
                    step.AsyncStepAction = async (data, info) =>
                    {
                        data.Should().BeSameAs(transactionData);
                        info.CurrentStepId.Should().Be(index);
                        runStepActions.Add(index);
                        stepActionThreadId.Add(Thread.CurrentThread.ManagedThreadId);
                        await Task.CompletedTask;
                    };
                }
                else
                {
                    step.StepAction = (data, info) =>
                    {
                        info.GoForward("4");
                        data.Should().BeSameAs(transactionData);
                        info.CurrentStepId.Should().Be(index);
                        runStepActions.Add(index);
                        stepActionThreadId.Add(Thread.CurrentThread.ManagedThreadId);
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

                if (i == 1
                    || i == 2)
                {
                    step.AsyncPostAction = async (data, info) =>
                    {
                        data.Should().BeSameAs(transactionData);
                        runPostActions.Add(index);
                        postActionThreadId.Add(Thread.CurrentThread.ManagedThreadId);
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
                        postActionThreadId.Add(Thread.CurrentThread.ManagedThreadId);
                        info.CurrentStepId.Should().Be(index);
                    };
                }

                target.Add(step);
            }
            ITransactionResult<object> transactionCallbackResult = null;

            /*
             0:
             1: executor for other thread (step action)
             2: executor for other thread (step action), executor for other thread (post action), go forward to step 4
             3: executor for other thread (step action), executor for other thread (post action)
             4: executor for other thread (step action)
             5: 
             6: 
             */

            // Act
            ITransactionResult<object> result = await target.Run(settings =>
            {
                settings.Data = transactionData;
                settings.Mode = RunMode.Run;
                settings.TransactionResultCallback = callbackResult =>
                {
                    transactionCallbackThreadId = Thread.CurrentThread.ManagedThreadId;
                    transactionCallbackResult = callbackResult;
                };
                settings.TransactionResultCallbackExecutor = callBackExecutor;
            });

            // Assert
            callBackExecutor.Dispose();

            foreach (TestExecutor executor in stepExecutors.Values.Concat(postExecutors.Values).Where(x => x != null))
            {
                executor.Dispose();
            }

            result.Should().BeSameAs(transactionCallbackResult);
            result.Data.Should().BeSameAs(transactionData);
            result.Errors.ShouldAllBeEquivalentTo(new Exception[0]);
            result.Recovered.Should().BeFalse();
            result.Result.Should().Be(ResultType.Success);
            callBackExecutor.Verify(Times.Once, Times.Once);

            foreach (TestExecutor executor in stepExecutors.Values.Concat(postExecutors.Values).Where(x => x != null && x != stepExecutors[3]))
            {
                executor.Verify(Times.Once, Times.Once);
            }

            stepExecutors[3].Verify(Times.Never, Times.Never);
            runStepActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "4", "5", "6" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[0]);
            runPostActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3", "4", "5", "6" });
            stepActionThreadId.ShouldAllBeEquivalentTo(new int[]
            {
                /*step 0*/ transactionRunThreadId,
                /*step 1*/ stepExecutors[1].ThreadId,
                /*step 2*/ stepExecutors[2].ThreadId,
                /*step 4*/ stepExecutors[4].ThreadId,
                /*step 5*/ stepExecutors[4].ThreadId,
                /*step 6*/ stepExecutors[4].ThreadId
            });
            postActionThreadId.ShouldAllBeEquivalentTo(new int[]
            {
                /*step 0*/ stepExecutors[4].ThreadId,
                /*step 1*/ stepExecutors[4].ThreadId,
                /*step 2*/ postExecutors[2].ThreadId,
                /*step 3*/ postExecutors[3].ThreadId,
                /*step 4*/ postExecutors[3].ThreadId,
                /*step 5*/ postExecutors[3].ThreadId,
                /*step 6*/ postExecutors[3].ThreadId
            });
            transactionCallbackThreadId.Should().Be(callBackExecutor.ThreadId);
        }

        [TestMethod]
        public async Task WhenGoForwardWithComparerWithCompareSuccessWithStepExecutors_ShouldMoveToStepProperly()
        {
            // Arrange
            object transactionData = new object();
            List<string> runStepActions = new List<string>();
            List<string> runUndoActions = new List<string>();
            List<string> runPostActions = new List<string>();
            List<int> stepActionThreadId = new List<int>();
            List<int> postActionThreadId = new List<int>();
            int transactionCallbackThreadId = 0;
            Dictionary<int, TestExecutor> stepExecutors = new Dictionary<int, TestExecutor>()
            {
                { 0, null },
                { 1, new TestExecutor() { ShouldRun = true } },
                { 2, new TestExecutor() { ShouldRun = true } },
                { 3, new TestExecutor() { ShouldRun = true } },
                { 4, new TestExecutor() { ShouldRun = true } },
                { 5, null },
                { 6, null }
            };
            Dictionary<int, TestExecutor> postExecutors = new Dictionary<int, TestExecutor>()
            {
                { 0, null },
                { 1, null },
                { 2, new TestExecutor() { ShouldRun = true } },
                { 3, new TestExecutor() { ShouldRun = true } },
                { 4, null },
                { 5, null },
                { 6, null }
            };
            int transactionRunThreadId = Thread.CurrentThread.ManagedThreadId;
            TestExecutor callBackExecutor = new TestExecutor() { ShouldRun = true };
            ITransaction<string, object> target = new TransactionFactory().Create<string, object>(options =>
            {
                options.TransactionInfo.Name = "test transaction";
            });

            for (int i = 0; i < 7; ++i)
            {
                string index = "B" + i.ToString();
                TransactionStep<string, object> step = new TransactionStep<string, object>()
                {
                    Id = index,
                    StepActionExecutor = stepExecutors[i] == null
                                           ? null
                                           : stepExecutors[i],
                    PostActionExecutor = postExecutors[i] == null
                                           ? null
                                           : postExecutors[i]
                };

                if (i != 2)
                {
                    step.AsyncStepAction = async (data, info) =>
                    {
                        data.Should().BeSameAs(transactionData);
                        info.CurrentStepId.Should().Be(index);
                        runStepActions.Add(index);
                        stepActionThreadId.Add(Thread.CurrentThread.ManagedThreadId);
                        await Task.CompletedTask;
                    };
                }
                else
                {
                    step.StepAction = (data, info) =>
                    {
                        info.GoForward("b4", StringComparer.OrdinalIgnoreCase);
                        data.Should().BeSameAs(transactionData);
                        info.CurrentStepId.Should().Be(index);
                        runStepActions.Add(index);
                        stepActionThreadId.Add(Thread.CurrentThread.ManagedThreadId);
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

                if (i == 1
                    || i == 2)
                {
                    step.AsyncPostAction = async (data, info) =>
                    {
                        data.Should().BeSameAs(transactionData);
                        runPostActions.Add(index);
                        postActionThreadId.Add(Thread.CurrentThread.ManagedThreadId);
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
                        postActionThreadId.Add(Thread.CurrentThread.ManagedThreadId);
                        info.CurrentStepId.Should().Be(index);
                    };
                }

                target.Add(step);
            }
            ITransactionResult<object> transactionCallbackResult = null;

            /*
             0:
             1: executor for other thread (step action)
             2: executor for other thread (step action), executor for other thread (post action), go forward to step 4
             3: executor for other thread (step action), executor for other thread (post action)
             4: executor for other thread (step action)
             5: 
             6: 
             */

            // Act
            ITransactionResult<object> result = await target.Run(settings =>
            {
                settings.Data = transactionData;
                settings.Mode = RunMode.Run;
                settings.TransactionResultCallback = callbackResult =>
                {
                    transactionCallbackThreadId = Thread.CurrentThread.ManagedThreadId;
                    transactionCallbackResult = callbackResult;
                };
                settings.TransactionResultCallbackExecutor = callBackExecutor;
            });

            // Assert
            callBackExecutor.Dispose();

            foreach (TestExecutor executor in stepExecutors.Values.Concat(postExecutors.Values).Where(x => x != null))
            {
                executor.Dispose();
            }

            result.Should().BeSameAs(transactionCallbackResult);
            result.Data.Should().BeSameAs(transactionData);
            result.Errors.ShouldAllBeEquivalentTo(new Exception[0]);
            result.Recovered.Should().BeFalse();
            result.Result.Should().Be(ResultType.Success);
            callBackExecutor.Verify(Times.Once, Times.Once);

            foreach (TestExecutor executor in stepExecutors.Values.Concat(postExecutors.Values).Where(x => x != null && x != stepExecutors[3]))
            {
                executor.Verify(Times.Once, Times.Once);
            }

            stepExecutors[3].Verify(Times.Never, Times.Never);
            runStepActions.ShouldAllBeEquivalentTo(new string[] { "B0", "B1", "B2", "B4", "B5", "B6" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[0]);
            runPostActions.ShouldAllBeEquivalentTo(new string[] { "B0", "B1", "B2", "B3", "B4", "B5", "B6" });
            stepActionThreadId.ShouldAllBeEquivalentTo(new int[]
            {
                /*step 0*/ transactionRunThreadId,
                /*step 1*/ stepExecutors[1].ThreadId,
                /*step 2*/ stepExecutors[2].ThreadId,
                /*step 4*/ stepExecutors[4].ThreadId,
                /*step 5*/ stepExecutors[4].ThreadId,
                /*step 6*/ stepExecutors[4].ThreadId
            });
            postActionThreadId.ShouldAllBeEquivalentTo(new int[]
            {
                /*step 0*/ stepExecutors[4].ThreadId,
                /*step 1*/ stepExecutors[4].ThreadId,
                /*step 2*/ postExecutors[2].ThreadId,
                /*step 3*/ postExecutors[3].ThreadId,
                /*step 4*/ postExecutors[3].ThreadId,
                /*step 5*/ postExecutors[3].ThreadId,
                /*step 6*/ postExecutors[3].ThreadId
            });
            transactionCallbackThreadId.Should().Be(callBackExecutor.ThreadId);
        }

        [TestMethod]
        public async Task WhenGoForwardWithComparerWithCompareFailureWithStepExecutors_ShouldFailTransaction()
        {
            // Arrange
            object transactionData = new object();
            List<string> runStepActions = new List<string>();
            List<string> runUndoActions = new List<string>();
            List<string> runPostActions = new List<string>();
            List<int> stepActionThreadId = new List<int>();
            List<int> postActionThreadId = new List<int>();
            int transactionCallbackThreadId = 0;
            Dictionary<int, TestExecutor> stepExecutors = new Dictionary<int, TestExecutor>()
            {
                { 0, null },
                { 1, new TestExecutor() { ShouldRun = true } },
                { 2, new TestExecutor() { ShouldRun = true } },
                { 3, new TestExecutor() { ShouldRun = true } },
                { 4, new TestExecutor() { ShouldRun = true } },
                { 5, null },
                { 6, null }
            };
            Dictionary<int, TestExecutor> postExecutors = new Dictionary<int, TestExecutor>()
            {
                { 0, null },
                { 1, null },
                { 2, new TestExecutor() { ShouldRun = true } },
                { 3, new TestExecutor() { ShouldRun = true } },
                { 4, null },
                { 5, null },
                { 6, null }
            };
            int transactionRunThreadId = Thread.CurrentThread.ManagedThreadId;
            TestExecutor callBackExecutor = new TestExecutor() { ShouldRun = true };
            ITransaction<string, object> target = new TransactionFactory().Create<string, object>(options =>
            {
                options.TransactionInfo.Name = "test transaction";
            });

            for (int i = 0; i < 7; ++i)
            {
                string index = "B" + i.ToString();
                TransactionStep<string, object> step = new TransactionStep<string, object>()
                {
                    Id = index,
                    StepActionExecutor = stepExecutors[i] == null
                                           ? null
                                           : stepExecutors[i],
                    PostActionExecutor = postExecutors[i] == null
                                           ? null
                                           : postExecutors[i]
                };

                if (i != 2)
                {
                    step.AsyncStepAction = async (data, info) =>
                    {
                        data.Should().BeSameAs(transactionData);
                        info.CurrentStepId.Should().Be(index);
                        runStepActions.Add(index);
                        stepActionThreadId.Add(Thread.CurrentThread.ManagedThreadId);
                        await Task.CompletedTask;
                    };
                }
                else
                {
                    step.StepAction = (data, info) =>
                    {
                        info.GoForward("b4", StringComparer.Ordinal);
                        data.Should().BeSameAs(transactionData);
                        info.CurrentStepId.Should().Be(index);
                        runStepActions.Add(index);
                        stepActionThreadId.Add(Thread.CurrentThread.ManagedThreadId);
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

                if (i == 1
                    || i == 2)
                {
                    step.AsyncPostAction = async (data, info) =>
                    {
                        data.Should().BeSameAs(transactionData);
                        runPostActions.Add(index);
                        postActionThreadId.Add(Thread.CurrentThread.ManagedThreadId);
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
                        postActionThreadId.Add(Thread.CurrentThread.ManagedThreadId);
                        info.CurrentStepId.Should().Be(index);
                    };
                }

                target.Add(step);
            }
            ITransactionResult<object> transactionCallbackResult = null;

            /*
             0:
             1: executor for other thread (step action)
             2: executor for other thread (step action), executor for other thread (post action), go forward to step 4
             3: executor for other thread (step action), executor for other thread (post action)
             4: executor for other thread (step action)
             5: 
             6: 
             */

            // Act
            ITransactionResult<object> result = await target.Run(settings =>
            {
                settings.Data = transactionData;
                settings.Mode = RunMode.Run;
                settings.TransactionResultCallback = callbackResult =>
                {
                    transactionCallbackThreadId = Thread.CurrentThread.ManagedThreadId;
                    transactionCallbackResult = callbackResult;
                };
                settings.TransactionResultCallbackExecutor = callBackExecutor;
            });

            // Assert
            callBackExecutor.Dispose();

            foreach (TestExecutor executor in stepExecutors.Values.Concat(postExecutors.Values).Where(x => x != null))
            {
                executor.Dispose();
            }

            result.Should().BeSameAs(transactionCallbackResult);
            result.Data.Should().BeSameAs(transactionData);
            result.Errors.Count().Should().Be(1);
            result.Errors.First().Message.Contains("Could not move forward to a step with id 'b4' as the step does not exist.").Should().BeTrue();
            result.Recovered.Should().BeFalse();
            result.Result.Should().Be(ResultType.Failed);
            runStepActions.ShouldAllBeEquivalentTo(new string[] { "B0", "B1", "B2" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[] { "B2", "B1", "B0" });
            runPostActions.ShouldAllBeEquivalentTo(new string[0]);
            stepActionThreadId.ShouldAllBeEquivalentTo(new int[]
            {
                /*step 0*/ transactionRunThreadId,
                /*step 1*/ stepExecutors[1].ThreadId,
                /*step 2*/ stepExecutors[2].ThreadId
            });
            transactionCallbackThreadId.Should().Be(callBackExecutor.ThreadId);
        }
    }
}
