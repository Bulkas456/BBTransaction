using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Threading.Tasks;
using BBTransaction.Transaction;
using BBTransaction.Factory;
using BBTransaction.Step;
using BBTransaction.Transaction.TransactionResult;
using BBTransaction.Transaction.Settings;

namespace BBTransactionTestsWithoutAsync
{
    [TestClass]
    public class ProcessingUndoTests
    {
        [TestMethod]
        public void WhenRunTransactionAndErrorOccurredStepAction_ShouldRunUndoSteps()
        {
            // Arrange
            object transactionData = new object();
            List<string> runStepActions = new List<string>();
            List<string> runUndoActions = new List<string>();
            List<string> runPostActions = new List<string>();
            Exception testException = new Exception("test exception");
            ITransaction<string, object> target = new TransactionFactory().Create<string, object>(options =>
            {
                options.TransactionInfo.Name = "test transaction";
            });

            for (int i = 0; i < 6; ++i)
            {
                string index = i.ToString();
                TransactionStep<string, object> step = new TransactionStep<string, object>()
                {
                    Id = index
                };

                step.StepAction = (data, info) =>
                {
                    data.Should().BeSameAs(transactionData);
                    info.CurrentStepId.Should().Be(index);
                    runStepActions.Add(index);

                    if (index == "3")
                    {
                        throw testException;
                    }
                };

                step.UndoAction = (data, info) =>
                {
                    data.Should().BeSameAs(transactionData);
                    info.CurrentStepId.Should().Be(index);
                    runUndoActions.Add(index);
                };

                step.PostAction = (data) =>
                {
                    data.Should().BeSameAs(transactionData);
                    runPostActions.Add(index);
                };

                target.Add(step);
            }
            ITransactionResult<object> result = null;

            /*
             0:
             1: 
             2:  
             3: error 
             4:
             5:
             */

            // Act
            target.Run(settings =>
            {
                settings.Data = transactionData;
                settings.Mode = RunMode.Run;
                settings.TransactionResultCallback = callbackResult => result = callbackResult;
            });

            // Assert
            result.Data.Should().BeSameAs(transactionData);
            result.Errors.ShouldAllBeEquivalentTo(new Exception[] { testException });
            result.Recovered.Should().BeFalse();
            result.Result.Should().Be(ResultType.Success);
            runStepActions.ShouldAllBeEquivalentTo(new string[] { "0", "1", "2", "3" });
            runUndoActions.ShouldAllBeEquivalentTo(new string[] { "3", "2", "1", "0" });
            runPostActions.ShouldAllBeEquivalentTo(new string[0]);
        }
    }
}
