using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BBTransaction.Step;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BBTransaction.Step.Settings;
using FluentAssertions;
using System.Globalization;
using BBTransaction.Transaction.Session.Info;
using BBTransaction.Executor;

namespace BBTransactionTestsWithoutAsync
{
    [TestClass]
    public class StepAdapterTests
    {
        [TestMethod]
        public void WhenCreateAdapter_ShouldCreateProperly()
        {
            // Arrange
            byte expectedData = 200;
            string adapterExpectedData = expectedData.ToString(CultureInfo.InvariantCulture);
            bool postActionExecuted = false;
            bool stepActionExecuted = false;
            bool undoActionExecuted = false;
            Mock<ITransactionStep<int, byte>> step = new Mock<ITransactionStep<int, byte>>();
            step.SetupGet(x => x.Description)
                .Returns("description");
            step.SetupGet(x => x.Id)
                .Returns(123);
            step.SetupGet(x => x.Settings)
                .Returns((StepSettings)int.MaxValue);
            step.SetupGet(x => x.PostAction)
                 .Returns((byte data, IPostTransactionSessionInfo<int> sessionInfo) =>
                 {
                     postActionExecuted = true;
                     data.Should().Be(expectedData);
                 });
            step.SetupGet(x => x.StepAction)
                .Returns((byte data, IStepTransactionSessionInfo<int> sessionInfo) =>
                {
                    stepActionExecuted = true;
                    data.Should().Be(expectedData);
                    sessionInfo.CurrentStepId.Should().Be(step.Object.Id);
                });
            step.SetupGet(x => x.UndoAction)
                .Returns((byte data, IUndoTransactionSessionInfo<int> sessionInfo) =>
                {
                    undoActionExecuted = true;
                    data.Should().Be(expectedData);
                    sessionInfo.CurrentStepId.Should().Be(step.Object.Id);
                });
            step.SetupGet(x => x.PostActionExecutor)
                .Returns(new Mock<IExecutor>().Object);
            step.SetupGet(x => x.StepActionExecutor)
                .Returns(new Mock<IExecutor>().Object);
            step.SetupGet(x => x.UndoActionExecutor)
                .Returns(new Mock<IExecutor>().Object);
            Mock<IStepTransactionSessionInfo<string>> stepInfo = new Mock<IStepTransactionSessionInfo<string>>();
            stepInfo.SetupGet(x => x.CurrentStepId)
                .Returns(step.Object.Id.ToString(CultureInfo.InvariantCulture));
            stepInfo.SetupGet(x => x.Cancelled)
                .Returns(true);
            stepInfo.SetupGet(x => x.Recovered)
                .Returns(true);
            stepInfo.SetupGet(x => x.SessionId)
                .Returns(Guid.NewGuid());
            stepInfo.SetupGet(x => x.StartTimestamp)
                .Returns(DateTime.Now);
            Mock<IUndoTransactionSessionInfo<string>> undoInfo = stepInfo.As<IUndoTransactionSessionInfo<string>>();
            Mock<IPostTransactionSessionInfo<string>> postInfo = undoInfo.As<IPostTransactionSessionInfo<string>>();
            ITransactionStep<string, string> adapter = step.Object.Adapter<string, string, int, byte>(id => id.ToString(), idString => int.Parse(idString), data => byte.Parse(data));

            // Act
            adapter.PostAction(adapterExpectedData, postInfo.Object);

            // Assert
            postActionExecuted.Should().BeTrue();

            // Act
            adapter.StepAction(adapterExpectedData, stepInfo.Object);

            // Assert
            stepActionExecuted.Should().BeTrue();

            // Act
            adapter.UndoAction(adapterExpectedData, undoInfo.Object);

            // Assert
            undoActionExecuted.Should().BeTrue();

            // Assert
            adapter.Description.Should().Be(step.Object.Description);
            int.Parse(adapter.Id).Should().Be(step.Object.Id);
            adapter.Settings.Should().Be(step.Object.Settings);
            adapter.PostActionExecutor.Should().BeSameAs(step.Object.PostActionExecutor);
            adapter.StepActionExecutor.Should().BeSameAs(step.Object.StepActionExecutor);
            adapter.UndoActionExecutor.Should().BeSameAs(step.Object.UndoActionExecutor);
        }
    }
}
