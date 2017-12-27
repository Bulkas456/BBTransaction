using System;
using System.Collections.Generic;
using System.Text;
#if !NET35 
using System.Threading.Tasks;
#endif
using BBTransaction.Info;
using BBTransaction.Step.Executor;

namespace BBTransaction.Step
{
    public class TransactionStep<TStepId, TData> : ITransactionStep<TStepId, TData>
    {
        public TStepId Id
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        } = string.Empty;

        public Action<TData, ITransactionInfo<TStepId>> StepAction
        {
            get;
            set;
        }

#if !NET35
        public Func<TData, ITransactionInfo<TStepId>, Task> AsyncStepAction
        {
            get;
            set;
        }
#endif

        public Action<TData, ITransactionInfo<TStepId>> UndoAction
        {
            get;
            set;
        }

#if !NET35
        public Func<TData, ITransactionInfo<TStepId>, Task> AsyncUndoAction
        {
            get;
            set;
        }
#endif

        public Action<TData> PostAction
        {
            get;
            set;
        }

#if !NET35
        public Func<TData, Task> AsyncPostAction
        {
            get;
            set;
        }
#endif

        public bool RunOnRecovered
        {
            get;
            set;
        }

        public bool UndoOnRecover
        {
            get;
            set;
        }

        public IStepExecutor<TData> Executor
        {
            get;
            set;
        }
    }
}
