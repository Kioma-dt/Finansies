using BuisnessLogic.DebtInterestCalculator;
using BuisnessLogic.Enums;
using FluentAssertions;
using Xunit;

namespace BuisnessLogic.Tests.DebtInterestCalculator
{
    public class NoneInterestCalculatorTests
    {
        private readonly NoneInterestCalculator _sut = new();

        [Fact]
        public void InterestType_ShouldBeNone()
        {
            _sut.InterestType.Should().Be(InterestType.None);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(100000)]
        public void Calculate_ShouldAlwaysReturnInitialAmount(
            decimal amount)
        {
            var result = _sut.Calculate(
                amount,
                0.25m,
                365m,
                100m,
                DateTime.MinValue,
                DateTime.MaxValue);

            result.Should().Be(amount);
        }

        [Fact]
        public void Calculate_ShouldIgnoreDates()
        {
            var result = _sut.Calculate(
                1000m,
                0.5m,
                12m,
                100m,
                DateTime.UtcNow,
                DateTime.UtcNow.AddYears(-5));

            result.Should().Be(1000m);
        }
    }
}
