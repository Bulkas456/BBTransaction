using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FluentAssertions;
using System.Globalization;
using BBTransaction.Step.Adapter;

namespace BBTransactionTestsWithAsync
{
    [TestClass]
    public class EqualityComparerAdapterTests
    {
        [TestMethod]
        public void WhenCreated_ShouldWorksProperly()
        {
            // Arrange
            Mock<IEqualityComparer<string>> original = new Mock<IEqualityComparer<string>>();
            original.Setup(x => x.Equals(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(true);
            original.Setup(x => x.GetHashCode(It.IsAny<string>()))
                    .Returns(1);
            Func<int, string> converter = number => number.ToString(CultureInfo.InvariantCulture);
            EqualityComparerAdapter<string, int> target = new EqualityComparerAdapter<string, int>(original.Object, converter);

            // Act
            bool equalityResult = target.Equals(123, 99);
            int hashCode = target.GetHashCode(45);

            // Assert
            equalityResult.Should().BeTrue();
            hashCode.Should().Be(1);
            original.Verify(x => x.Equals("123", "99"), Times.Once);
            original.Verify(x => x.GetHashCode("45"), Times.Once);
        }
    }
}
