using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Definition;
using BBTransaction.Transaction;
using Moq;

namespace BBTransactionTestsWithoutAsync
{
    public class AssertStorageOperationsContext<TStepId, TData>
    {
        public Times SessionStartedTimes
        {
            get;
            set;
        } = Times.Once();

        public TData TransactionData
        {
            get;
            set;
        }

        public IEnumerable<TStepId> ExpectedStepsOrder
        {
            get;
            set;
        }

        public IEnumerable<TStepId> ExpectedUndoOrder
        {
            get;
            set;
        }

        public ITransaction<TStepId, TData> Transaction
        {
            get;
            set;
        }
    }
}
