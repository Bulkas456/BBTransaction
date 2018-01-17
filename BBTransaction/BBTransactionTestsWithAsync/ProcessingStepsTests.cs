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
using BBTransaction.Executor;
using System.Threading;
using System.Linq;
using BBTransaction.Step.Settings;

namespace BBTransactionTestsWithAsync
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
            runStepActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3", "4" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[0]);
            runPostActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3", "4" });
        }

        [TestMethod]
        public async Task WhenRunTransactionWithStepExecutors_ShouldRunAllStepsProperly()
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
                { 2, null },
                { 3, new TestExecutor() { ShouldRun = true } },
                { 4, new TestExecutor() { ShouldRun = true }},
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
            int step1StepActionThreadId = stepExecutors[1].ThreadId;
            int step2PostActionThreadId = postExecutors[2].ThreadId;
            int step3StepActionThreadId = stepExecutors[3].ThreadId;
            int step3PostActionThreadId = postExecutors[3].ThreadId;
            int step4StepActionThreadId = stepExecutors[4].ThreadId;
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

            foreach (TestExecutor executor in stepExecutors.Values.Concat(postExecutors.Values).Where(x => x != null))
            {
                executor.Verify(Times.Once, Times.Once);
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
            transactionCallbackThreadId.Should().Be(callBackExecutor.ThreadId);
        }

        [TestMethod]
        public async Task WhenRunTransactionWithStepExecutorsWithSameExecutorForAllActions_ShouldRunAllStepsProperly()
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
                { 2, null },
                { 3, new TestExecutor() { ShouldRun = true } },
                { 4, new TestExecutor() { ShouldRun = true }},
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
            int step1StepActionThreadId = stepExecutors[1].ThreadId;
            int step2PostActionThreadId = postExecutors[2].ThreadId;
            int step3StepActionThreadId = stepExecutors[3].ThreadId;
            int step3PostActionThreadId = postExecutors[3].ThreadId;
            int step4StepActionThreadId = stepExecutors[4].ThreadId;
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
                                           : postExecutors[i],
                    Settings = i == 1
                                ? StepSettings.SameExecutorForAllActions
                                : StepSettings.None
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
             1: executor for other thread (step action), SameExecutorForAllActions
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
            foreach (TestExecutor executor in stepExecutors.Values.Concat(postExecutors.Values).Where(x => x != null))
            {
                executor.Dispose();
            }

            result.Should().BeSameAs(transactionCallbackResult);
            result.Data.Should().BeSameAs(transactionData);
            result.Errors.ShouldAllBeEquivalentTo(new Exception[0]);
            result.Recovered.Should().BeFalse();
            result.Result.Should().Be(ResultType.Success);
            TestExecutor sharedExecutor = stepExecutors[1];
            sharedExecutor.Verify(Times.Exactly(2), Times.Exactly(2));

            foreach (TestExecutor executor in stepExecutors.Values.Concat(postExecutors.Values).Where(x => x != null && x != sharedExecutor))
            {
                executor.Verify(Times.Once, Times.Once);
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
                /*step 1*/ step1StepActionThreadId,
                /*step 2*/ step2PostActionThreadId,
                /*step 3*/ step3PostActionThreadId,
                /*step 4*/ step3PostActionThreadId,
                /*step 5*/ step3PostActionThreadId,
                /*step 6*/ step3PostActionThreadId
            });
            transactionCallbackThreadId.Should().Be(step3PostActionThreadId);
        }
    }
}
