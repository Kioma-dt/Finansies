using BuisnessLogic.DebtInterestCalculator;
using BuisnessLogic.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace BuisnessLogic.Tests.DebtInterestCalculator
{
    public class DebtInterestCalculatorProviderTests
    {
        [Fact]
        public void GetCalculator_WhenCalculatorExists_ShouldReturnCalculator()
        {
            // Arrange
            var calculatorMock = new Mock<IDebtInterestCalculator>();

            calculatorMock
                .SetupGet(x => x.InterestType)
                .Returns(InterestType.Simple);

            var provider = new DebtInterestCalculatorProvider(
                [calculatorMock.Object]);

            // Act
            var result = provider.GetCalculator(InterestType.Simple);

            // Assert
            result.Should().BeSameAs(calculatorMock.Object);
        }

        [Fact]
        public void GetCalculator_WhenCalculatorDoesNotExist_ShouldThrowArgumentException()
        {
            // Arrange
            var provider = new DebtInterestCalculatorProvider([]);

            // Act
            Action act = () =>
                provider.GetCalculator(InterestType.Simple);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage(
                    $"No Calculator for Interest Type: {InterestType.Simple}");
        }

        [Fact]
        public void GetCalculator_WhenSeveralCalculatorsExist_ShouldReturnCorrectCalculator()
        {
            // Arrange
            var simpleCalculator = new Mock<IDebtInterestCalculator>();
            simpleCalculator
                .SetupGet(x => x.InterestType)
                .Returns(InterestType.Simple);

            var fixedCalculator = new Mock<IDebtInterestCalculator>();
            fixedCalculator
                .SetupGet(x => x.InterestType)
                .Returns(InterestType.Fixed);

            var provider = new DebtInterestCalculatorProvider(
            [
                simpleCalculator.Object,
            fixedCalculator.Object
            ]);

            // Act
            var result = provider.GetCalculator(InterestType.Fixed);

            // Assert
            result.Should().BeSameAs(fixedCalculator.Object);
        }

        [Theory]
        [InlineData(InterestType.None)]
        [InlineData(InterestType.Simple)]
        [InlineData(InterestType.Fixed)]
        [InlineData(InterestType.Complex)]
        public void GetCalculator_ShouldReturnCalculatorForCorrespondingType(
            InterestType interestType)
        {
            // Arrange
            var calculatorMock = new Mock<IDebtInterestCalculator>();

            calculatorMock
                .SetupGet(x => x.InterestType)
                .Returns(interestType);

            var provider = new DebtInterestCalculatorProvider(
                [calculatorMock.Object]);

            // Act
            var result = provider.GetCalculator(interestType);

            // Assert
            result.Should().BeSameAs(calculatorMock.Object);
        }

        [Fact]
        public void Constructor_WhenDuplicateInterestTypesExist_ShouldThrowArgumentException()
        {
            // Arrange
            var calculator1 = new Mock<IDebtInterestCalculator>();
            calculator1
                .SetupGet(x => x.InterestType)
                .Returns(InterestType.Simple);

            var calculator2 = new Mock<IDebtInterestCalculator>();
            calculator2
                .SetupGet(x => x.InterestType)
                .Returns(InterestType.Simple);

            // Act
            Action act = () =>
                new DebtInterestCalculatorProvider(
                [
                    calculator1.Object,
                calculator2.Object
                ]);

            // Assert
            act.Should()
                .Throw<ArgumentException>();
        }

        [Fact]
        public void GetCalculator_ShouldAlwaysReturnSameInstance()
        {
            // Arrange
            var calculatorMock = new Mock<IDebtInterestCalculator>();

            calculatorMock
                .SetupGet(x => x.InterestType)
                .Returns(InterestType.Complex);

            var provider = new DebtInterestCalculatorProvider(
                [calculatorMock.Object]);

            // Act
            var result1 = provider.GetCalculator(InterestType.Complex);
            var result2 = provider.GetCalculator(InterestType.Complex);

            // Assert
            result1.Should().BeSameAs(result2);
            result1.Should().BeSameAs(calculatorMock.Object);
        }

        [Fact]
        public void Constructor_WhenNoCalculatorsProvided_ShouldAllowCreation()
        {
            // Arrange & Act
            var provider = new DebtInterestCalculatorProvider([]);

            // Assert
            provider.Should().NotBeNull();
        }
    }
}
