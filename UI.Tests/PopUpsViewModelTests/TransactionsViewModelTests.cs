using BuisnessLogic.Entities;
using UI.PopUps.ViewModels;

namespace UI.Tests.PopUpsViewModelTests
{
    using BuisnessLogic.UseCases.TransactionsUseCasses.Commands;
    using BuisnessLogic.UseCases.TransactionsUseCasses.Queries;
    using FluentAssertions;
    using MediatR;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using UI.Messages;
    using UI.OrderingServices;
    using UI.Popups;
    using UI.PopUps.Service;
    using UI.ViewModels;
    using Xunit;
    public class TransactionsViewModelTests
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<IUserContext> _user = new();
        private readonly Mock<IPopUpService> _popup = new();
        private readonly Mock<ITransactionsOrderingServiceFactory> _orderingFactory = new();
        private readonly Mock<ITransactionsOrderingService> _orderingService = new();

        private readonly Guid _userId = Guid.NewGuid();

        public TransactionsViewModelTests()
        {
            _user.Setup(x => x.UserId).Returns(_userId);

            _orderingFactory
                .Setup(x => x.Create(It.IsAny<TransactionsOrderBy>()))
                .Returns(_orderingService.Object);
        }

        private TransactionsViewModel CreateVm()
            => new TransactionsViewModel(
                _mediator.Object,
                _user.Object,
                _popup.Object,
                _orderingFactory.Object);

        private static Transaction CreateTransaction(decimal amount, DateTime date, Guid? accountId = null)
        {
            return new Transaction
            {
                Id = Guid.NewGuid(),
                Description = "Test",
                Amount = amount,
                Date = date,
                AccountId = accountId ?? Guid.NewGuid()
            };
        }

       

        [Fact]
        public async Task ChangeSorting_ShouldToggleAscending_WhenSameField()
        {
            var vm = CreateVm();

            await vm.ChangeSortingCommand.ExecuteAsync("Date");

            var firstArrow = vm.DateSortArrow;

            await vm.ChangeSortingCommand.ExecuteAsync("Date");

            var secondArrow = vm.DateSortArrow;

            firstArrow.Should().NotBe(secondArrow);
        }

    }
}
