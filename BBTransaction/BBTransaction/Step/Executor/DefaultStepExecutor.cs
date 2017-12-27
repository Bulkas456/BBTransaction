using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Step.Executor
{
    public class DefaultStepExecutor<TData> : IStepExecutor<TData>
    {
        public static readonly IStepExecutor<TData> Instance = new DefaultStepExecutor<TData>();
    }
}
