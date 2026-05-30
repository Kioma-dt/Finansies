using BuisnessLogic.DebtInterestCalculator;
using BuisnessLogic.Enums;
using FluentAssertions;
using Xunit;

namespace BuisnessLogic.Tests.DebtInterestCalculator
{
    public class SimpleInterestCalculatorTests
    {
        private readonly SimpleInterestCalculator _sut = new();

        [Fact]
        public void InterestType_ShouldBeSimple()
        {
            _sut.InterestType.Should().Be(InterestType.Simple);
        }

        [Fact]
        public void Calculate_WhenDatesAreEqual_ShouldReturnInitialAmount()
        {
            var now = DateTime.UtcNow;

            var result = _sut.Calculate(
                1000m,
                0.1m,
                12m,
                0m,
                now,
                now);

            result.Should().Be(1000m);
        }

        [Fact]
        public void Calculate_WhenOneYearPassed_ShouldCalculateSimpleInterest()
        {
            var start = new DateTime(2024, 1, 1);
            var end = start.AddDays(365);

            var result = _sut.Calculate(
                1000m,
                0.1m,
                12m,
                0m,
                start,
                end);

            result.Should().Be(1100m);
        }

        [Fact]
        public void Calculate_WhenTwoYearsPassed_ShouldCalculateSimpleInterest()
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

            result.Should().Be(1200m);
        }

        [Fact]
        public void Calculate_WhenHalfYearPassed_ShouldIgnoreFractionalYear()
        {
            var start = new DateTime(2024, 1, 1);
            var end = start.AddDays(180);

            var result = _sut.Calculate(
                1000m,
                0.1m,
                12m,
                0m,
                start,
                end);

            result.Should().Be(1000m);
        }

        [Fact]
        public void Calculate_WhenInterestRateIsZero_ShouldReturnAmount()
        {
            var start = DateTime.UtcNow;
            var end = start.AddYears(10);

            var result = _sut.Calculate(
                1000m,
                0m,
                12m,
                0m,
                start,
                end);

            result.Should().Be(1000m);
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
