using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic.Tests.OrderingService
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using UI.OrderingServices;
    using UI.ViewModels;
    using Xunit;

    public class TransactionsOrderingServiceFactoryTests
    {
        [Fact]
        public void Create_ShouldReturnCorrectService()
        {
            var service = new TransactionsOrderingServiceByDescription();

            var factory = new TransactionsOrderingServiceFactory(new[]
            {
            service
        });

            var result = factory.Create(TransactionsOrderBy.Description);

            result.Should().BeSameAs(service);
        }

        [Fact]
        public void Create_ShouldThrow_WhenServiceNotFound()
        {
            var factory = new TransactionsOrderingServiceFactory(new List<ITransactionsOrderingService>());

            Action act = () => factory.Create(TransactionsOrderBy.Description);

            act.Should().Throw<ArgumentException>();
        }
    }
}
