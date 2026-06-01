namespace BuisnessLogic.Tests.OrderingService
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using UI.OrderingServices;
    using UI.ViewModels;
    using Xunit;

    public class TransactionsOrderingServiceByTypeTests
    {
        private static List<DisplayedTransaction> CreateData() => new()
{
    new DisplayedTransaction(Guid.NewGuid(), "B", "200", "Income", "02.01.2025", "A", "C", "F", "D"),
    new DisplayedTransaction(Guid.NewGuid(), "A", "100", "Expense", "01.01.2025", null, null, null, null),
    new DisplayedTransaction(Guid.NewGuid(), "C", "300", "Income", "03.01.2025", "Z", "Y", "X", "W")
};

        [Fact]
        public void Should_Order_By_Type()
        {
            var service = new TransactionsOrderingServiceByType();

            var data = new List<DisplayedTransaction>
        {
            new(Guid.NewGuid(), "A", "100", "Expense", "01.01.2025", null, null, null, null),
            new(Guid.NewGuid(), "B", "100", "Income", "01.01.2025", null, null, null, null),
        };

            var result = service.Order(data, true).ToList();

            result.First().Type.Should().Be("Expense");
        }
    }
}
