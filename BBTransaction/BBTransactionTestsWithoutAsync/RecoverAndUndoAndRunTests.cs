﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BBTransaction.Factory;
using BBTransaction.Step;
using BBTransaction.Step.Settings;
using BBTransaction.Transaction;
using BBTransaction.Transaction.Session.Storage;
using BBTransaction.Transaction.Session.Storage.TransactionData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FluentAssertions;
using BBTransaction.Transaction.TransactionResult;
using BBTransaction.Transaction.Settings;
using System.Threading;
using System.Linq;
using BBTransactionTestsWithoutAsync;

namespace BBTransactionTestsWithAsync
{
    [TestClass]
    public class RecoverAndUndoAndRunTests
    {
        [TestMethod]
        public void WhenRunRecoveredTransaction_ShouldProcessStepsProperly()
        {
            // Arrange
            object transactionData = new object();
            List<string> runStepActions = new List<string>();
            List<string> runUndoActions = new List<string>();
            List<string> runPostActions = new List<string>();
            Mock<ITransactionStorage<object>> storageMock = new Mock<ITransactionStorage<object>>();
            Mock<ITransactionData<object>> recoveredData = new Mock<ITransactionData<object>>();
            recoveredData.SetupGet(x => x.CurrentStepIndex)
                         .Returns(2);
            recoveredData.SetupGet(x => x.Data)
                         .Returns(transactionData);
            recoveredData.SetupGet(x => x.SessionId)
                         .Returns(Guid.NewGuid());
            recoveredData.SetupGet(x => x.StartTimestamp)
                         .Returns(DateTime.Now);
            storageMock.Setup(x => x.RecoverTransaction())
                       .Returns(recoveredData.Object);
            ITransaction<string, object> target = new TransactionFactory().Create<string, object>(options =>
            {
                options.TransactionInfo.Name = "test transaction";
                options.TransactionStorageCreator = context => storageMock.Object;
            });

            for (int i = 0; i < 6; ++i)
            {
                string index = i.ToString();
                TransactionStep<string, object> step = new TransactionStep<string, object>()
                {
                    Id = index,
                    Settings = StepSettings.None
                };

                step.StepAction = (data, info) =>
                {
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
                    runPostActions.Add(index);
                    info.CurrentStepId.Should().Be(index);
                };

                target.Add(step);
            }
            ITransactionResult<object> result = null;

            /*
             0:
             1: 
             2: recovered step
             3:  
             4:
             5:
             */

            // Act
            target.Run(settings =>
            {
                settings.Data = transactionData;
                settings.Mode = RunMode.RecoverAndUndoAndRun;
                settings.TransactionResultCallback = callbackResult => result = callbackResult;
            });

            // Assert
            result.Data.Should().BeSameAs(transactionData);
            result.Errors.ShouldAllBeEquivalentTo(new Exception[0]);
            result.Recovered.Should().BeTrue();
            result.Result.Should().Be(ResultType.Success);
            runStepActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3", "4", "5" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[] { "2", "1", "0" });
            runPostActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3", "4", "5" });
            storageMock.AssertStorageOperations(new AssertStorageOperationsContext<string, object>()
            {
                SessionStartedTimes = Times.Never(),
                TransactionData = transactionData,
                Transaction = target,
                ExpectedStepsOrder = runStepActions,
                ExpectedUndoOrder = runUndoActions
            });
        }

        [TestMethod]
        public void WhenRunRecoveredTransactionWithCurrentStepAsFirstStepWithExecutor_ShouldProcessStepsProperly()
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
            int transactionRunThreadId = Thread.CurrentThread.ManagedThreadId;
            Dictionary<int, TestExecutor> stepExecutors = new Dictionary<int, TestExecutor>()
            {
                { 0, null },
                { 1, null },
                { 2, new TestExecutor() { ShouldRun = true } },
                { 3, new TestExecutor() { ShouldRun = true } },
                { 4, new TestExecutor() { ShouldRun = true } },
                { 5, null },
                { 6, null }
            };
            Dictionary<int, TestExecutor> undoExecutors = new Dictionary<int, TestExecutor>()
            {
                { 0, new TestExecutor() { ShouldRun = true } },
                { 1, null },
                { 2, new TestExecutor() { ShouldRun = true } },
                { 3, null },
                { 4, new TestExecutor() { ShouldRun = true } },
                { 5, null },
                { 6, null }
            };
            Dictionary<int, TestExecutor> postExecutors = new Dictionary<int, TestExecutor>()
            {
                { 0, null },
                { 1, null },
                { 2, null },
                { 3, new TestExecutor() { ShouldRun = true } },
                { 4, null },
                { 5, null },
                { 6, null }
            };
            TestExecutor callBackExecutor = new TestExecutor() { ShouldRun = true };
            Mock<ITransactionStorage<object>> storageMock = new Mock<ITransactionStorage<object>>();
            Mock<ITransactionData<object>> recoveredData = new Mock<ITransactionData<object>>();
            recoveredData.SetupGet(x => x.CurrentStepIndex)
                         .Returns(0);
            recoveredData.SetupGet(x => x.Data)
                         .Returns(transactionData);
            recoveredData.SetupGet(x => x.SessionId)
                         .Returns(Guid.NewGuid());
            recoveredData.SetupGet(x => x.StartTimestamp)
                         .Returns(DateTime.Now);
            storageMock.Setup(x => x.RecoverTransaction())
                       .Returns(recoveredData.Object);
            ITransaction<string, object> target = new TransactionFactory().Create<string, object>(options =>
            {
                options.TransactionInfo.Name = "test transaction";
                options.TransactionStorageCreator = context => storageMock.Object;
            });

            for (int i = 0; i < 7; ++i)
            {
                string index = i.ToString();
                TransactionStep<string, object> step = new TransactionStep<string, object>()
                {
                    Id = index,
                    StepActionExecutor = stepExecutors[i],
                    UndoActionExecutor = undoExecutors[i],
                    PostActionExecutor = postExecutors[i],
                    Settings = StepSettings.None
                };

                step.StepAction = (data, info) =>
                {
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
                    undoActionThreadId.Add(Thread.CurrentThread.ManagedThreadId);
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
            ITransactionResult<object> result = null;

            /*
             0: recovered step, executor for other thread (undo action)
             1: 
             2: executor for other thread (step action), executor for other thread (undo action)
             3: executor for other thread (step action), executor for other thread (post action)
             4: executor for other thread (step action), executor for other thread (undo action)
             5: 
             6: 
             */

            // Act
            using (ManualResetEvent transactionEndResetEvent = new ManualResetEvent(false))
            {
                target.Run(settings =>
                {
                    settings.Data = transactionData;
                    settings.Mode = RunMode.RecoverAndUndoAndRun;
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
            result.Recovered.Should().BeTrue();
            result.Result.Should().Be(ResultType.Success);
            callBackExecutor.Verify(Times.Once, Times.Once);

            foreach (TestExecutor executor in stepExecutors.Values.Concat(postExecutors.Values).Where(x => x != null))
            {
                executor.Verify(Times.Once, Times.Once);
            }

            runStepActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3", "4", "5", "6" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[] { "0" });
            runPostActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3", "4", "5", "6" });
            stepActionThreadId.ShouldAllBeEquivalentTo(new int[]
            {
                /*step 0*/ undoExecutors[0].ThreadId,
                /*step 1*/ undoExecutors[0].ThreadId,
                /*step 2*/ stepExecutors[2].ThreadId,
                /*step 3*/ stepExecutors[3].ThreadId,
                /*step 4*/ stepExecutors[4].ThreadId,
                /*step 5*/ stepExecutors[4].ThreadId,
                /*step 6*/ stepExecutors[4].ThreadId
            });
            undoActionThreadId.ShouldAllBeEquivalentTo(new int[] { undoExecutors[0].ThreadId });
            postActionThreadId.ShouldAllBeEquivalentTo(new int[]
            {
                /*step 0*/ stepExecutors[4].ThreadId,
                /*step 1*/ stepExecutors[4].ThreadId,
                /*step 2*/ stepExecutors[4].ThreadId,
                /*step 3*/ postExecutors[3].ThreadId,
                /*step 4*/ postExecutors[3].ThreadId,
                /*step 5*/ postExecutors[3].ThreadId,
                /*step 6*/ postExecutors[3].ThreadId
            });
            transactionCallbackThreadId.Should().Be(callBackExecutor.ThreadId);
            storageMock.AssertStorageOperations(new AssertStorageOperationsContext<string, object>()
            {
                SessionStartedTimes = Times.Never(),
                TransactionData = transactionData,
                Transaction = target,
                ExpectedStepsOrder = runStepActions,
                ExpectedUndoOrder = runUndoActions
            });
        }
    }
}
