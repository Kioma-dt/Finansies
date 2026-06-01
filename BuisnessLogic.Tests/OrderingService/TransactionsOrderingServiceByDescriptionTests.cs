namespace BuisnessLogic.Tests.OrderingService
{
    using FluentAssertions;
    using System.Linq;
    using UI.OrderingServices;
    using UI.ViewModels;
    using Xunit;

    public class TransactionsOrderingServiceByDescriptionTests
    {
        private static List<DisplayedTransaction> CreateData() => new()
{
    new DisplayedTransaction(Guid.NewGuid(), "B", "200", "Income", "02.01.2025", "A", "C", "F", "D"),
    new DisplayedTransaction(Guid.NewGuid(), "A", "100", "Expense", "01.01.2025", null, null, null, null),
    new DisplayedTransaction(Guid.NewGuid(), "C", "300", "Income", "03.01.2025", "Z", "Y", "X", "W")
};

        [Fact]
        public void Should_Order_Ascending()
        {
            var service = new TransactionsOrderingServiceByDescription();
            var data = CreateData();

            var result = service.Order(data, true).ToList();

            result.First().Description.Should().Be("A");
            result.Last().Description.Should().Be("C");
        }

        [Fact]
        public void Should_Order_Descending()
        {
            var service = new TransactionsOrderingServiceByDescription();
            var data = CreateData();

            var result = service.Order(data, false).ToList();

            result.First().Description.Should().Be("C");
            result.Last().Description.Should().Be("A");
        }
    }
}
