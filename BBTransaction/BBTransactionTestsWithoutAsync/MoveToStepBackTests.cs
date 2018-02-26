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

namespace BBTransactionTestsWithoutAsync
{
    [TestClass]
    public class MoveToStepBackTests
    {
        [TestMethod]
        public void WhenGoBackOnceWithoutComparer_ShouldMoveToStepProperly()
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

                step.StepAction = (data, info) =>
                {
                    if (info.CurrentStepId == "3"
                        && numberOfGoBack++ == 0)
                    {
                        info.GoBack("1");
                    }

                    data.Should().BeSameAs(transactionData);
                    info.CurrentStepId.Should().Be(index);
                    runStepActions.Add(index);
                };

                step.UndoAction = (data, info) =>
                {
                    data.Should().BeSameAs(transactionData);
                    info.CurrentStepId.Should().Be(index);
                    runUndoActions.Add(index);
                };

                step.PostAction = (data, info) =>
                {
                    data.Should().BeSameAs(transactionData);
                    info.CurrentStepId.Should().Be(index);
                    runPostActions.Add(index);
                };

                target.Add(step);
            }

            /*
             0:
             1:
             2:  
             3: go back to step 1
             4:
             */

            // Act
            ITransactionResult<object> result = null;
            target.Run(settings =>
            {
                settings.Data = transactionData;
                settings.Mode = RunMode.Run;
                settings.TransactionResultCallback = callbackResult => result = callbackResult;
            });

            // Assert
            result.Data.Should().BeSameAs(transactionData);
            result.Errors.ShouldAllBeEquivalentTo(new Exception[0]);
            result.Recovered.Should().BeFalse();
            result.Result.Should().Be(ResultType.Success);
            runStepActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3", "1", "2", "3", "4" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[] { "3", "2", "1" });
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
        public void WhenGoBackToTheSameStep_ShouldMoveToStepProperly()
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

                step.StepAction = (data, info) =>
                {
                    if (info.CurrentStepId == "3"
                        && numberOfGoBack++ == 0)
                    {
                        info.GoBack("3");
                    }

                    data.Should().BeSameAs(transactionData);
                    info.CurrentStepId.Should().Be(index);
                    runStepActions.Add(index);
                };

                step.UndoAction = (data, info) =>
                {
                    data.Should().BeSameAs(transactionData);
                    info.CurrentStepId.Should().Be(index);
                    runUndoActions.Add(index);
                };

                step.PostAction = (data, info) =>
                {
                    data.Should().BeSameAs(transactionData);
                    info.CurrentStepId.Should().Be(index);
                    runPostActions.Add(index);
                };

                target.Add(step);
            }

            /*
             0:
             1:
             2:  
             3: go back to step 3
             4:
             */

            // Act
            ITransactionResult<object> result = null;
            target.Run(settings =>
            {
                settings.Data = transactionData;
                settings.Mode = RunMode.Run;
                settings.TransactionResultCallback = callbackResult => result = callbackResult;
            });

            // Assert
            result.Data.Should().BeSameAs(transactionData);
            result.Errors.ShouldAllBeEquivalentTo(new Exception[0]);
            result.Recovered.Should().BeFalse();
            result.Result.Should().Be(ResultType.Success);
            runStepActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3", "3", "4" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[] { "3" });
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
        public void WhenGoBackMoreTimes_ShouldMoveToStepProperly()
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

                step.UndoAction = (data, info) =>
                {
                    data.Should().BeSameAs(transactionData);
                    info.CurrentStepId.Should().Be(index);
                    runUndoActions.Add(index);
                };

                step.PostAction = (data, info) =>
                {
                    data.Should().BeSameAs(transactionData);
                    info.CurrentStepId.Should().Be(index);
                    runPostActions.Add(index);
                };

                target.Add(step);
            }

            /*
             0:
             1: 
             2:  
             3: go back to step 1 (two times)
             4: go back to step 2 (one time)
             5: 
             */

            // Act
            ITransactionResult<object> result = null;
            target.Run(settings =>
            {
                settings.Data = transactionData;
                settings.Mode = RunMode.Run;
                settings.TransactionResultCallback = callbackResult => result = callbackResult;
            });

            // Assert
            result.Data.Should().BeSameAs(transactionData);
            result.Errors.ShouldAllBeEquivalentTo(new Exception[0]);
            result.Recovered.Should().BeFalse();
            result.Result.Should().Be(ResultType.Success);
            runStepActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3", "1", "2", "3", "1", "2", "3", "4", "2", "3", "4", "5" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[] { "3", "2", "1", "3", "2", "1", "4", "3", "2" });
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
        public void WhenGoBackWithComparerWithCompareSuccess_ShouldMoveToStepProperly()
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
            int goBackCounter = 0;

            for (int i = 0; i < 5; ++i)
            {
                string index = "A" + i.ToString();
                TransactionStep<string, object> step = new TransactionStep<string, object>()
                {
                    Id = index
                };

                step.StepAction = (data, info) =>
                {
                    if (info.CurrentStepId == "A3"
                        && goBackCounter++ == 0)
                    {
                        info.GoBack("a0", StringComparer.OrdinalIgnoreCase);
                    }

                    data.Should().BeSameAs(transactionData);
                    info.CurrentStepId.Should().Be(index);
                    runStepActions.Add(index);
                };

                step.UndoAction = (data, info) =>
                {
                    data.Should().BeSameAs(transactionData);
                    info.CurrentStepId.Should().Be(index);
                    runUndoActions.Add(index);
                };

                step.PostAction = (data, info) =>
                {
                    data.Should().BeSameAs(transactionData);
                    info.CurrentStepId.Should().Be(index);
                    runPostActions.Add(index);
                };

                target.Add(step);
            }

            /*
             0:
             1: 
             2:  
             3: go forward to step 0
             4:
             */

            // Act
            ITransactionResult<object> result = null;
            target.Run(settings =>
            {
                settings.Data = transactionData;
                settings.Mode = RunMode.Run;
                settings.TransactionResultCallback = callbackResult => result = callbackResult;
            });

            // Assert
            result.Data.Should().BeSameAs(transactionData);
            result.Errors.ShouldAllBeEquivalentTo(new Exception[0]);
            result.Recovered.Should().BeFalse();
            result.Result.Should().Be(ResultType.Success);
            runStepActions.ShouldAllBeEquivalentTo(new string[] { "A0", "A1", "A2", "A3", "A0", "A1", "A2", "A3", "A4" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[] { "A3", "A2", "A1", "A0" });
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
        public void WhenGoBackWithComparerWithCompareFailure_ShouldFailTransaction()
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

                step.StepAction = (data, info) =>
                {
                    if (info.CurrentStepId == "A2")
                    {
                        info.GoBack("a0", StringComparer.Ordinal);
                    }

                    data.Should().BeSameAs(transactionData);
                    info.CurrentStepId.Should().Be(index);
                    runStepActions.Add(index);
                };

                step.UndoAction = (data, info) =>
                {
                    data.Should().BeSameAs(transactionData);
                    info.CurrentStepId.Should().Be(index);
                    runUndoActions.Add(index);
                };

                step.PostAction = (data, info) =>
                {
                    data.Should().BeSameAs(transactionData);
                    info.CurrentStepId.Should().Be(index);
                    runPostActions.Add(index);
                };

                target.Add(step);
            }

            /*
             0:
             1: 
             2: go back to step 0
             3:
             4:
             */

            // Act
            ITransactionResult<object> result = null;
            target.Run(settings =>
            {
                settings.Data = transactionData;
                settings.Mode = RunMode.Run;
                settings.TransactionResultCallback = callbackResult => result = callbackResult;
            });

            // Assert
            result.Data.Should().BeSameAs(transactionData);
            result.Errors.Count().Should().Be(1);
            result.Errors.First().Message.Contains("Could not move back to a step with id 'a0' as the step does not exist.").Should().BeTrue();
            result.Recovered.Should().BeFalse();
            result.Result.Should().Be(ResultType.Failed);
            runStepActions.ShouldAllBeEquivalentTo(new string[] { "A0", "A1", "A2" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[] { "A2", "A1", "A0" });
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
        public void WhenGoBackWithoutComparerWithStepExecutors_ShouldMoveToStepProperly()
        {
            // Arrange
            object transactionData = new object();
            List<string> runStepActions = new List<string>();
            List<string> runUndoActions = new List<string>();
            List<string> runPostActions = new List<string>();
            List<int> stepActionThreadId = new List<int>();
            List<int> undoActionThreadId = new List<int>();
            List<int> postActionThreadId = new List<int>();
            int transactionCallbackThreadId = 0;
            Dictionary<int, TestExecutor> stepExecutors = new Dictionary<int, TestExecutor>()
            {
                { 0, new TestExecutor() { ShouldRun = true } },
                { 1, null },
                { 2, null },
                { 3, new TestExecutor() { ShouldRun = true } },
                { 4, new TestExecutor() { ShouldRun = true } },
                { 5, null },
                { 6, null }
            };
            Dictionary<int, TestExecutor> undoStepExecutors = new Dictionary<int, TestExecutor>()
            {
                { 0, null},
                { 1, null },
                { 2, new TestExecutor() { ShouldRun = true } },
                { 3, null },
                { 4, null },
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
            int goBackCounter = 0;

            for (int i = 0; i < 7; ++i)
            {
                string index = i.ToString();
                TransactionStep<string, object> step = new TransactionStep<string, object>()
                {
                    Id = index,
                    StepActionExecutor = stepExecutors[i],
                    PostActionExecutor = postExecutors[i],
                    UndoActionExecutor = undoStepExecutors[i]
                };

                step.StepAction = (data, info) =>
                {
                    if (info.CurrentStepId == "3"
                        && goBackCounter++ == 0)
                    {
                        info.GoBack("1");
                    }

                    data.Should().BeSameAs(transactionData);
                    info.CurrentStepId.Should().Be(index);
                    runStepActions.Add(index);
                    stepActionThreadId.Add(Thread.CurrentThread.ManagedThreadId);
                };

                step.UndoAction = (data, info) =>
                {
                    undoActionThreadId.Add(Thread.CurrentThread.ManagedThreadId);
                    data.Should().BeSameAs(transactionData);
                    info.CurrentStepId.Should().Be(index);
                    runUndoActions.Add(index);
                };

                step.PostAction = (data, info) =>
                {
                    data.Should().BeSameAs(transactionData);
                    runPostActions.Add(index);
                    postActionThreadId.Add(Thread.CurrentThread.ManagedThreadId);
                    info.CurrentStepId.Should().Be(index);
                };

                target.Add(step);
            }

            /*
             0: executor for other thread (step action)
             1: 
             2: executor for other thread (undo action), executor for other thread (post action)
             3: executor for other thread (step action), executor for other thread (post action), go back to step 1
             4: executor for other thread (step action)
             5: 
             6: 
             */

            // Act
            ITransactionResult<object> result = null;

            using (ManualResetEvent transactionEndResetEvent = new ManualResetEvent(false))
            {
                target.Run(settings =>
                {
                    settings.Data = transactionData;
                    settings.Mode = RunMode.Run;
                    settings.TransactionResultCallback = callbackResult =>
                    {
                        transactionCallbackThreadId = Thread.CurrentThread.ManagedThreadId;
                        result = callbackResult;
                        transactionEndResetEvent.Set();
                    };
                    settings.TransactionResultCallbackExecutor = callBackExecutor;
                });
                transactionEndResetEvent.WaitOne();
            }

            // Assert
            callBackExecutor.Dispose();

            foreach (TestExecutor executor in stepExecutors.Values.Concat(postExecutors.Values).Where(x => x != null))
            {
                executor.Dispose();
            }

            result.Data.Should().BeSameAs(transactionData);
            result.Errors.ShouldAllBeEquivalentTo(new Exception[0]);
            result.Recovered.Should().BeFalse();
            result.Result.Should().Be(ResultType.Success);
            callBackExecutor.Verify(Times.Once, Times.Once);

            foreach (TestExecutor executor in stepExecutors.Values.Concat(postExecutors.Values).Where(x => x != null))
            {
                executor.Verify(Times.AtLeastOnce, Times.AtLeastOnce);
            }

            runStepActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3", "1", "2", "3", "4", "5", "6" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[] { "3", "2", "1" });
            runPostActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3", "4", "5", "6" });
            stepActionThreadId.ShouldAllBeEquivalentTo(new int[]
            {
                /*step 0*/ stepExecutors[0].ThreadId,
                /*step 1*/ stepExecutors[0].ThreadId,
                /*step 2*/ stepExecutors[0].ThreadId,
                /*step 3*/ stepExecutors[3].ThreadId,
                /*step 1*/ undoStepExecutors[2].ThreadId,
                /*step 2*/ undoStepExecutors[2].ThreadId,
                /*step 3*/ stepExecutors[3].ThreadId,
                /*step 4*/ stepExecutors[4].ThreadId,
                /*step 5*/ stepExecutors[4].ThreadId,
                /*step 6*/ stepExecutors[4].ThreadId,
            });
            undoActionThreadId.ShouldAllBeEquivalentTo(new int[] 
            {
                /*step 3*/ stepExecutors[3].ThreadId,
                /*step 2*/ undoStepExecutors[2].ThreadId,
                /*step 1*/ undoStepExecutors[2].ThreadId,
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
        public void WhenGoBackWithComparerWithCompareSuccessWithStepExecutors_ShouldMoveToStepProperly()
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
            int goBackCounter = 0;

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

                step.StepAction = (data, info) =>
                {
                    if (info.CurrentStepId == "B2"
                        && goBackCounter++ == 0)
                    {
                        info.GoBack("b0", StringComparer.OrdinalIgnoreCase);
                    }

                    data.Should().BeSameAs(transactionData);
                    info.CurrentStepId.Should().Be(index);
                    runStepActions.Add(index);
                    stepActionThreadId.Add(Thread.CurrentThread.ManagedThreadId);
                };

                step.UndoAction = (data, info) =>
                {
                    data.Should().BeSameAs(transactionData);
                    info.CurrentStepId.Should().Be(index);
                    runUndoActions.Add(index);
                };

                step.PostAction = (data, info) =>
                {
                    data.Should().BeSameAs(transactionData);
                    runPostActions.Add(index);
                    postActionThreadId.Add(Thread.CurrentThread.ManagedThreadId);
                    info.CurrentStepId.Should().Be(index);
                };

                target.Add(step);
            }

            /*
             0:
             1: executor for other thread (step action)
             2: executor for other thread (step action), executor for other thread (post action), go back to step 0
             3: executor for other thread (step action), executor for other thread (post action)
             4: executor for other thread (step action)
             5: 
             6: 
             */

            // Act
            ITransactionResult<object> result = null;

            using (ManualResetEvent transactionEndResetEvent = new ManualResetEvent(false))
            {
                target.Run(settings =>
                {
                    settings.Data = transactionData;
                    settings.Mode = RunMode.Run;
                    settings.TransactionResultCallback = callbackResult =>
                    {
                        transactionCallbackThreadId = Thread.CurrentThread.ManagedThreadId;
                        result = callbackResult;
                        transactionEndResetEvent.Set();
                    };
                    settings.TransactionResultCallbackExecutor = callBackExecutor;
                });
                transactionEndResetEvent.WaitOne();
            }

            // Assert
            callBackExecutor.Dispose();

            foreach (TestExecutor executor in stepExecutors.Values.Concat(postExecutors.Values).Where(x => x != null))
            {
                executor.Dispose();
            }

            result.Data.Should().BeSameAs(transactionData);
            result.Errors.ShouldAllBeEquivalentTo(new Exception[0]);
            result.Recovered.Should().BeFalse();
            result.Result.Should().Be(ResultType.Success);
            callBackExecutor.Verify(Times.Once, Times.Once);

            foreach (TestExecutor executor in stepExecutors.Values.Concat(postExecutors.Values).Where(x => x != null))
            {
                executor.Verify(Times.AtLeastOnce, Times.AtLeastOnce);
            }
            
            runStepActions.ShouldAllBeEquivalentTo(new string[] { "B0", "B1", "B2", "B0", "B1", "B2", "B3", "B4", "B5", "B6" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[] { "B2", "B1", "B0" });
            runPostActions.ShouldAllBeEquivalentTo(new string[] { "B0", "B1", "B2", "B3", "B4", "B5", "B6" });
            stepActionThreadId.ShouldAllBeEquivalentTo(new int[]
            {
                /*step 0*/ transactionRunThreadId,
                /*step 1*/ stepExecutors[1].ThreadId,
                /*step 2*/ stepExecutors[2].ThreadId,
                /*step 0*/ stepExecutors[2].ThreadId,
                /*step 1*/ stepExecutors[1].ThreadId,
                /*step 2*/ stepExecutors[2].ThreadId,
                /*step 3*/ stepExecutors[3].ThreadId,
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
        public void WhenGoBackWithComparerWithCompareFailureWithStepExecutors_ShouldFailTransaction()
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

                step.StepAction = (data, info) =>
                {
                    if (info.CurrentStepId == "B2")
                    {
                        info.GoBack("b0", StringComparer.Ordinal);
                    }

                    data.Should().BeSameAs(transactionData);
                    info.CurrentStepId.Should().Be(index);
                    runStepActions.Add(index);
                    stepActionThreadId.Add(Thread.CurrentThread.ManagedThreadId);
                };

                step.UndoAction = (data, info) =>
                {
                    data.Should().BeSameAs(transactionData);
                    info.CurrentStepId.Should().Be(index);
                    runUndoActions.Add(index);
                };

                step.PostAction = (data, info) =>
                {
                    data.Should().BeSameAs(transactionData);
                    runPostActions.Add(index);
                    postActionThreadId.Add(Thread.CurrentThread.ManagedThreadId);
                    info.CurrentStepId.Should().Be(index);
                };

                target.Add(step);
            }

            /*
             0:
             1: executor for other thread (step action)
             2: executor for other thread (step action), executor for other thread (post action), go back to step 0
             3: executor for other thread (step action), executor for other thread (post action)
             4: executor for other thread (step action)
             5: 
             6: 
             */

            // Act
            ITransactionResult<object> result = null;

            using (ManualResetEvent transactionEndResetEvent = new ManualResetEvent(false))
            {
                target.Run(settings =>
                {
                    settings.Data = transactionData;
                    settings.Mode = RunMode.Run;
                    settings.TransactionResultCallback = callbackResult =>
                    {
                        transactionCallbackThreadId = Thread.CurrentThread.ManagedThreadId;
                        result = callbackResult;
                        transactionEndResetEvent.Set();
                    };
                    settings.TransactionResultCallbackExecutor = callBackExecutor;
                });
                transactionEndResetEvent.WaitOne();
            }

            // Assert
            callBackExecutor.Dispose();

            foreach (TestExecutor executor in stepExecutors.Values.Concat(postExecutors.Values).Where(x => x != null))
            {
                executor.Dispose();
            }

            result.Data.Should().BeSameAs(transactionData);
            result.Errors.Count().Should().Be(1);
            result.Errors.First().Message.Contains("Could not move back to a step with id 'b0' as the step does not exist.").Should().BeTrue();
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
