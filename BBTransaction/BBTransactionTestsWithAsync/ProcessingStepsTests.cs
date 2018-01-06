using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BBTransaction;
using BBTransaction.Factory;
using BBTransaction.Step;
using BBTransaction.Transaction;
using BBTransaction.Transaction.TransactionResult;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using BBTransaction.Transaction.Settings;
using Moq;
using BBTransaction.Step.Executor;
using System.Threading;
using System.Linq;

namespace BBTransactionTestsNetCore
{
    [TestClass]
    public class ProcessingStepsTests
    {
        [TestMethod]
        public async Task WhenRunTransaction_ShouldRunAllStepsProperly()
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
            result.Info.Should().Be(string.Empty);
            result.Recovered.Should().BeFalse();
            result.Success.Should().BeTrue();
            runStepActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3", "4" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[0]);
            runPostActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3", "4" });
        }

        [TestMethod]
        public async Task WhenRunTransactionWithStepExecutors_ShouldRunAllStepsProperly()
        {
            // Arrange
            int transactionRunThreadId = Thread.CurrentThread.ManagedThreadId;
            int step1StepActionThreadId = 0;
            int step2PostActionThreadId = 0;
            int step3StepActionThreadId = 0;
            int step3PostActionThreadId = 0;
            int step4StepActionThreadId = 0;
            object transactionData = new object();
            List<string> runStepActions = new List<string>();
            List<string> runUndoActions = new List<string>();
            List<string> runPostActions = new List<string>();
            List<int> stepActionThreadId = new List<int>();
            List<int> postActionThreadId = new List<int>();
            int transactionCallbackThreadId = 0;
            Dictionary<int, Mock<IStepExecutor>> stepExecutors = new Dictionary<int, Mock<IStepExecutor>>()
            {
                { 0, null },
                { 1, this.CreateStepExecutorMock(id => step1StepActionThreadId = id) },
                { 2, null },
                { 3, this.CreateStepExecutorMock(id => step3StepActionThreadId = id) },
                { 4, this.CreateStepExecutorMock(id => step4StepActionThreadId = id) },
                { 5, null },
                { 6, null }
            };
            Dictionary<int, Mock<IStepExecutor>> postExecutors = new Dictionary<int, Mock<IStepExecutor>>()
            {
                { 0, null },
                { 1, null },
                { 2, this.CreateStepExecutorMock(id => step2PostActionThreadId = id) },
                { 3, this.CreateStepExecutorMock(id => step3PostActionThreadId = id) },
                { 4, null },
                { 5, null },
                { 6, null }
            };
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
                                           : stepExecutors[i].Object,
                    PostActionExecutor = postExecutors[i] == null
                                           ? null
                                           : postExecutors[i].Object
                };

                if (i == 3 
                    || i == 4)
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
                    step.AsyncPostAction = async (data) =>
                    {
                        data.Should().BeSameAs(transactionData);
                        runPostActions.Add(index);
                        postActionThreadId.Add(Thread.CurrentThread.ManagedThreadId);
                        await Task.CompletedTask;
                    };
                }
                else
                {
                    step.PostAction = (data) =>
                    {
                        data.Should().BeSameAs(transactionData);
                        runPostActions.Add(index);
                        postActionThreadId.Add(Thread.CurrentThread.ManagedThreadId);
                    };
                }

                target.Add(step);
            }
            ITransactionResult<object> transactionCallbackResult = null;

            /*
             0:
             1: executor for other thread (step action)
             2: executor for other thread (post action)
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
            });

            // Assert
            result.Should().BeSameAs(transactionCallbackResult);
            result.Data.Should().BeSameAs(transactionData);
            result.Errors.ShouldAllBeEquivalentTo(new Exception[0]);
            result.Info.Should().Be(string.Empty);
            result.Recovered.Should().BeFalse();
            result.Success.Should().BeTrue();

            foreach (Mock<IStepExecutor> executors in stepExecutors.Values.Concat(postExecutors.Values).Where(x => x != null))
            {
                executors.VerifyGet(x => x.ShouldRun, Times.Once);
                executors.Verify(x => x.Run(It.IsNotNull<Func<Task>>()), Times.Once);
            }

            runStepActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3", "4", "5", "6" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[0]);
            runPostActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3", "4", "5", "6" });
            stepActionThreadId.ShouldAllBeEquivalentTo(new int[] 
            {
                /*step 0*/ transactionRunThreadId,
                /*step 1*/ step1StepActionThreadId,
                /*step 2*/ step1StepActionThreadId,
                /*step 3*/ step3StepActionThreadId,
                /*step 4*/ step4StepActionThreadId,
                /*step 5*/ step4StepActionThreadId,
                /*step 6*/ step4StepActionThreadId
            });
            postActionThreadId.ShouldAllBeEquivalentTo(new int[]
            {
                /*step 0*/ step4StepActionThreadId,
                /*step 1*/ step4StepActionThreadId,
                /*step 2*/ step2PostActionThreadId,
                /*step 3*/ step3PostActionThreadId,
                /*step 4*/ step3PostActionThreadId,
                /*step 5*/ step3PostActionThreadId,
                /*step 6*/ step3PostActionThreadId
            });
            transactionCallbackThreadId.Should().Be(step3PostActionThreadId);
        }

        private Mock<IStepExecutor> CreateStepExecutorMock(Action<int> threadIdGetAction)
        {
            Mock<IStepExecutor> mock = new Mock<IStepExecutor>();
            mock.SetupGet(x => x.ShouldRun)
                .Returns(true);
            mock.Setup(x => x.Run(It.IsNotNull<Func<Task>>()))
                .Callback((Func<Task> method) => 
                {
                    new Thread(new ThreadStart(() =>
                    {
                        threadIdGetAction(Thread.CurrentThread.ManagedThreadId);
                        method();
                    }))
                    {
                        IsBackground = true
                    }
                    .Start();
                });
            return mock;
        }
    }
}
