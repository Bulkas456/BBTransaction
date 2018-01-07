using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BBTransaction.Step.Executor;
using Moq;

namespace BBTransactionTestsWithoutAsync
{
    internal class TestExecutor : IStepExecutor,
                                  IDisposable
    {
        private readonly BlockingCollection<Action> actions = new BlockingCollection<Action>();

        private readonly Thread executorThread;

        private readonly ManualResetEvent threadEndedResetEvent = new ManualResetEvent(false);

        private bool disposed;

        public TestExecutor()
        {
            using (ManualResetEvent resetEvent = new ManualResetEvent(false))
            {
                this.executorThread = new Thread(new ThreadStart(() =>
                {
                    this.ThreadId = Thread.CurrentThread.ManagedThreadId;
                    resetEvent.Set();

                    foreach (Action action in this.actions.GetConsumingEnumerable())
                    {
                        this.Mock.Object.Run(action);
                        action();
                    }

                    this.threadEndedResetEvent.Set();
                }))
                {
                    IsBackground = true
                };
                this.executorThread.Start();
                resetEvent.WaitOne();
            }
        }

        ~TestExecutor()
        {
            this.Dispose(false);
        }

        public int ThreadId
        {
            get;
            private set;
        }

        public Mock<IStepExecutor> Mock
        {
            get;
        } = new Mock<IStepExecutor>();

        public bool ShouldRun
        {
            get
            {
                return this.Mock.Object.ShouldRun;
            }

            set
            {
                this.Mock
                    .SetupGet(x => x.ShouldRun)
                    .Returns(value);
            }
        }

        public void Run(Action action)
        {
            this.actions.Add(action);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed
                && disposing)
            {
                this.disposed = true;
                this.actions.CompleteAdding();
                this.threadEndedResetEvent.WaitOne();
                this.actions.Dispose();
                this.threadEndedResetEvent.Dispose();
            }
        }
    }
}
