namespace BuisnessLogic.Tests.OrderingService
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using UI.OrderingServices;
    using UI.ViewModels;
    using Xunit;

    public class TransactionsOrderingServiceByDateTests
    {
        [Fact]
        public void Should_Order_By_Date()
        {
            var service = new TransactionsOrderingServiceByDate();

            var data = new List<DisplayedTransaction>
        {
            new(Guid.NewGuid(), "A", "100", "Income", "03.01.2025", null, null, null, null),
            new(Guid.NewGuid(), "B", "100", "Income", "01.01.2025", null, null, null, null),
        };

            var result = service.Order(data, true).ToList();

            result.First().Date.Should().Be("01.01.2025");
        }

        [Fact]
        public void Should_Handle_Invalid_Date()
        {
            var service = new TransactionsOrderingServiceByDate();

            var data = new List<DisplayedTransaction>
        {
            new(Guid.NewGuid(), "A", "100", "Income", "invalid", null, null, null, null),
            new(Guid.NewGuid(), "B", "100", "Income", "01.01.2025", null, null, null, null),
        };

            var result = service.Order(data, false).ToList();

            result.First().Date.Should().Be("01.01.2025");
        }
    }
}
