using BuisnessLogic.DebtInterestCalculator;
using BuisnessLogic.Enums;
using FluentAssertions;
using Xunit;

namespace BuisnessLogic.Tests.DebtInterestCalculator
{

    public class ComplexInterestCalcilatorTests
    {
        private readonly ComplexInterestCalcilator _sut = new();

        [Fact]
        public void InterestType_ShouldBeComplex()
        {
            _sut.InterestType.Should().Be(InterestType.Complex);
        }

        [Fact]
        public void Calculate_WhenDatesAreEqual_ShouldReturnInitialAmount()
        {
            // Arrange
            var date = DateTime.UtcNow;

            // Act
            var result = _sut.Calculate(
                1000m,
                0.1m,
                12m,
                0m,
                date,
                date);

            // Assert
            result.Should().BeApproximately(1000m, 0.01m);
        }

        [Fact]
        public void Calculate_WhenOneYearPassed_ShouldCalculateCompoundInterest()
        {
            // Arrange
            var start = new DateTime(2024, 1, 1);
            var end = start.AddDays(365);

            // Act
            var result = _sut.Calculate(
                1000m,
                0.12m,
                12m,
                0m,
                start,
                end);

            // Assert
            result.Should().BeApproximately(
                1126.83m,
                0.1m);
        }

        [Fact]
        public void Calculate_WhenTwoYearsPassed_ShouldCalculateCorrectly()
        {
            var start = new DateTime(2024, 1, 1);
            var end = start.AddDays(730);

            var result = _sut.Calculate(
                1000m,
                0.1m,
                12m,
                0m,
                start,
                end);

            result.Should().BeGreaterThan(1200m);
        }

        [Fact]
        public void Calculate_WhenInterestRateIsZero_ShouldReturnInitialAmount()
        {
            var start = DateTime.UtcNow;
            var end = start.AddYears(5);

            var result = _sut.Calculate(
                1000m,
                0m,
                12m,
                0m,
                start,
                end);

            result.Should().BeApproximately(1000m, 0.01m);
        }

        [Fact]
        public void Calculate_WhenCurrentDateBeforeStartDate_ShouldThrowException()
        {
            var start = DateTime.UtcNow;
            var end = start.AddDays(-1);

            Action act = () => _sut.Calculate(
                1000m,
                0.1m,
                12m,
                0m,
                start,
                end);

            act.Should()
                .Throw<Exception>()
                .WithMessage("Wrong Dates");
        }
    }
}
