using BuisnessLogic.Entities;
using BuisnessLogic.UseCases.TransactionsUseCasses.Commands;
using BuisnessLogic.UseCases.TransactionsUseCasses.Queries;
using MediatR;
using UI.Messages;
using UI.OrderingServices;
using UI.Popups;
using UI.PopUps.Service;
using UI.PopUps.ViewModels;
using UI.ViewModels;

namespace UI.Tests.ViewModelsTests
{
    public class TransactionsViewModelTests
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<IUserContext> _userContext = new();
        private readonly Mock<IPopUpService> _popupService = new();
        private readonly Mock<ITransactionsOrderingServiceFactory> _orderingFactory = new();
        private readonly Mock<ITransactionsOrderingService> _orderingService = new();

        private readonly Guid _userId = Guid.NewGuid();

        public TransactionsViewModelTests()
        {
            _userContext
                .Setup(x => x.UserId)
                .Returns(_userId);

            _orderingFactory
                .Setup(x => x.Create(It.IsAny<TransactionsOrderBy>()))
                .Returns(_orderingService.Object);

            _orderingService
                .Setup(x => x.Order(
                    It.IsAny<IEnumerable<DisplayedTransaction>>(),
                    It.IsAny<bool>()))
                .Returns<IEnumerable<DisplayedTransaction>, bool>((x, _) => x);
        }

        private TransactionsViewModel CreateVm()
        {
            return new TransactionsViewModel(
                _mediator.Object,
                _userContext.Object,
                _popupService.Object,
                _orderingFactory.Object);
        }


        [Fact]
        public async Task Receive_ShouldFillDisplayedTransactions()
        {
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                Description = "Coffee",
                Amount = 100,
                Date = DateTime.Today
            };

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllTransactionsQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { transaction });

            var vm = CreateVm();

            vm.Receive(
                new DataBaseChangedMessage(
                    DataBaseChangedMessageType.Transactions));

            await Task.Delay(50);

            vm.DisplayedTransactions.Should().ContainSingle();

            vm.DisplayedTransactions[0]
                .Description
                .Should()
                .Be("Coffee");
        }

        [Fact]
        public async Task Receive_SelectedAccountChanged_ShouldFilterByAccount()
        {
            var account1 = Guid.NewGuid();
            var account2 = Guid.NewGuid();

            var transactions = new[]
            {
            new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                AccountId = account1,
                Description = "A",
                Date = DateTime.Today
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                AccountId = account2,
                Description = "B",
                Date = DateTime.Today
            }
        };

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllTransactionsQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactions);

            var vm = CreateVm();

            vm.Receive(
                new DataBaseChangedMessage(
                    DataBaseChangedMessageType.Transactions));

            await Task.Delay(50);

            vm.Receive(
                new SelectedAccountChangedMessage(account1));

            vm.DisplayedTransactions.Should().ContainSingle();

            vm.DisplayedTransactions[0]
                .Description
                .Should()
                .Be("A");
        }

        [Fact]
        public async Task ChangeSorting_ShouldUseSelectedField()
        {
            var vm = CreateVm();

            await vm.ChangeSorting("Amount");

            _orderingFactory.Verify(
                x => x.Create(TransactionsOrderBy.Amount),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task ChangeSorting_ShouldToggleAscending_WhenSameFieldSelected()
        {
            var vm = CreateVm();

            await vm.ChangeSorting("Date");

            vm.GetSortArrow(TransactionsOrderBy.Date)
                .Should()
                .Be(" ▼");

            await vm.ChangeSorting("Date");

            vm.GetSortArrow(TransactionsOrderBy.Date)
                .Should()
                .Be(" ▲");
        }

        [Fact]
        public void GetSortArrow_ShouldReturnAscendingArrow()
        {
            var vm = CreateVm();

            vm.GetSortArrow(TransactionsOrderBy.Date)
                .Should()
                .Be(" ▲");
        }

        [Fact]
        public async Task Load_ShouldNotThrow()
        {
            var vm = CreateVm();

            var act = () => vm.Load();

            act.Should().NotThrow();
        }
    }
}
