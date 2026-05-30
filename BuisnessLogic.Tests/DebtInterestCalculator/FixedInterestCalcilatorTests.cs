using BuisnessLogic.DebtInterestCalculator;
using BuisnessLogic.Enums;
using FluentAssertions;
using Xunit;

namespace BuisnessLogic.Tests.DebtInterestCalculator
{
    public class FixedInterestCalcilatorTests
    {
        private readonly FixedInterestCalcilator _sut = new();

        [Fact]
        public void InterestType_ShouldBeFixed()
        {
            _sut.InterestType.Should().Be(InterestType.Fixed);
        }

        [Fact]
        public void Calculate_WhenDatesAreEqual_ShouldReturnAmount()
        {
            var now = DateTime.UtcNow;

            var result = _sut.Calculate(
                1000m,
                0m,
                12m,
                100m,
                now,
                now);

            result.Should().Be(1000m);
        }

        [Fact]
        public void Calculate_WhenOneYearPassed_ShouldAddFixedAmountForEachCapitalisation()
        {
            var start = new DateTime(2024, 1, 1);
            var end = start.AddDays(365);

            var result = _sut.Calculate(
                1000m,
                0m,
                12m,
                100m,
                start,
                end);

            result.Should().Be(2200m);
        }

        [Fact]
        public void Calculate_WhenHalfYearPassed_ShouldNotAddAnything()
        {
            var start = new DateTime(2024, 1, 1);
            var end = start.AddDays(180);

            var result = _sut.Calculate(
                1000m,
                0m,
                12m,
                100m,
                start,
                end);

            result.Should().Be(1000m);
        }

        [Fact]
        public void Calculate_WhenTwoYearsPassed_ShouldAddForTwentyFourCapitalisations()
        {
            var start = new DateTime(2024, 1, 1);
            var end = start.AddDays(730);

            var result = _sut.Calculate(
                1000m,
                0m,
                12m,
                50m,
                start,
                end);

            result.Should().Be(2200m);
        }

        [Fact]
        public void Calculate_WhenCurrentDateBeforeStartDate_ShouldThrowException()
        {
            var start = DateTime.UtcNow;
            var end = start.AddDays(-1);

            Action act = () => _sut.Calculate(
                1000m,
                0m,
                12m,
                100m,
                start,
                end);

            act.Should()
                .Throw<Exception>()
                .WithMessage("Wrong Dates");
        }
    }
}
