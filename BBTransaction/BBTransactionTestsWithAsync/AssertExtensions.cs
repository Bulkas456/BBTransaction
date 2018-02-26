using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Transaction.Session.Storage;
using BBTransaction.Transaction.Session.Storage.TransactionData;
using FluentAssertions;
using Moq;
using System.Linq;
using BBTransaction.Definition;
using System.Reflection;
using BBTransaction.Transaction.Context;

namespace BBTransactionTestsWithAsync
{
    /// <summary>
    /// Extensions for assertions.
    /// </summary>
    public static class AssertExtensions
    {
        public static void AssertStorageOperations<TStepId, TData>(this Mock<ITransactionStorage<TData>> storageMock, AssertStorageOperationsContext<TStepId, TData> context)
        {
            storageMock.Verify(x => x.SessionStarted(It.Is<ITransactionData<TData>>(y => object.Equals(y.Data, context.TransactionData))), context.SessionStartedTimes);
            ITransactionDefinition<TStepId, TData> definition = ((ITransactionContext<TStepId, TData>)context.Transaction
                                                                       .GetType()
                                                                       .GetField("context", BindingFlags.Instance | BindingFlags.NonPublic)
                                                                       .GetValue(context.Transaction))
                                                                       .Definition;

            if (context.ExpectedStepsOrder == null
                || !context.ExpectedStepsOrder.Any())
            {
                storageMock.Verify(x => x.StepPrepared(It.IsAny<ITransactionData<TData>>()), Times.Never);
            }
            else
            {
                foreach (TStepId stepId in context.ExpectedStepsOrder.Distinct())
                {
                    storageMock.Verify(x => x.StepPrepared(It.Is<ITransactionData<TData>>(y => object.Equals(y.Data, context.TransactionData)
                                                                                               && object.Equals(definition.GetByIndex(y.CurrentStepIndex).Id, stepId))), 
                                       Times.Exactly(context.ExpectedStepsOrder.Where(step => object.Equals(step, stepId)).Select(step => 1).Sum()));
                }
            }

            if (context.ExpectedUndoOrder == null
                || !context.ExpectedUndoOrder.Any())
            {
                storageMock.Verify(x => x.StepReceding(It.IsAny<ITransactionData<TData>>()), Times.Never);
            }
            else
            {
                foreach (TStepId stepId in context.ExpectedUndoOrder.Distinct())
                {
                    storageMock.Verify(x => x.StepReceding(It.Is<ITransactionData<TData>>(y => object.Equals(y.Data, context.TransactionData)
                                                                                               && object.Equals(definition.GetByIndex(y.CurrentStepIndex).Id, stepId))),
                                       Times.Exactly(context.ExpectedUndoOrder.Where(step => object.Equals(step, stepId)).Select(step => 1).Sum()));
                }
            }

            storageMock.Verify(x => x.RemoveSession(It.Is<ITransactionData<TData>>(y => object.Equals(y.Data, context.TransactionData))), Times.Once);
        }
    }
}
