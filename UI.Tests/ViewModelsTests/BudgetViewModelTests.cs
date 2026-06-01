using BuisnessLogic.Entities;
using BuisnessLogic.UseCases.BudgetUseCasses.Commands;
using BuisnessLogic.UseCases.BudgetUseCasses.Queries;
using MediatR;
using UI.Messages;
using UI.Popups;
using UI.PopUps.Service;
using UI.ViewModels;

namespace UI.Tests.ViewModelsTests
{
    public class BudgetViewModelTests
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<IUserContext> _user = new();
        private readonly Mock<IPopUpService> _popup = new();

        private readonly Guid _userId = Guid.NewGuid();

        private BudgetViewModel CreateVm()
        {
            _user.Setup(x => x.UserId)
                .Returns(_userId);

            return new BudgetViewModel(
                _mediator.Object,
                _user.Object,
                _popup.Object);
        }

        [Fact]
        public void BudgetItem_UsedPercent_ShouldCalculateCorrectly()
        {
            var item = new BudgetItem
            {
                Budget = new Budget
                {
                    Limit = 200
                },
                UsedAmount = 50
            };

            item.UsedPercent.Should().Be(0.25);
        }

        [Fact]
        public void BudgetItem_UsedPercent_ShouldReturnZero_WhenLimitIsZero()
        {
            var item = new BudgetItem
            {
                Budget = new Budget
                {
                    Limit = 0
                },
                UsedAmount = 100
            };

            item.UsedPercent.Should().Be(0);
        }

        [Fact]
        public void BudgetItem_LimitText_ShouldFormatCorrectly()
        {
            var item = new BudgetItem
            {
                Budget = new Budget
                {
                    Limit = 1000
                },
                UsedAmount = 250
            };

            item.LimitText.Should().Be("250 / 1000");
        }

        [Fact]
        public async Task Receive_ShouldLoadBudgets()
        {
            var budget1 = new Budget
            {
                Id = Guid.NewGuid(),
                Limit = 100
            };

            var budget2 = new Budget
            {
                Id = Guid.NewGuid(),
                Limit = 200
            };

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllBudgetsQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[]
                {
                budget1,
                budget2
                });

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetRelevantTransactionsQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Transaction>());

            var vm = CreateVm();

            vm.Receive(
                new DataBaseChangedMessage(
                    DataBaseChangedMessageType.Budgets));

            await Task.Delay(50);

            vm.Budgets.Should().HaveCount(2);
        }

        [Fact]
        public async Task Receive_ShouldCalculateUsedAmount()
        {
            var budget = new Budget
            {
                Id = Guid.NewGuid(),
                Limit = 1000
            };

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllBudgetsQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[]
                {
                budget
                });

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetRelevantTransactionsQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Transaction>
                {
                new() { Amount = 100 },
                new() { Amount = 200 }
                });

            var vm = CreateVm();

            vm.Receive(
                new DataBaseChangedMessage(
                    DataBaseChangedMessageType.Budgets));

            await Task.Delay(50);

            vm.Budgets.Single()
                .UsedAmount
                .Should()
                .Be(300);
        }

        [Fact]
        public async Task SelectBudget_ShouldLoadTransactions()
        {
            var budget = new Budget
            {
                Id = Guid.NewGuid()
            };

            var transactions = new List<Transaction>
        {
            new()
            {
                Amount = 100,
                Date = DateTime.Today
            },
            new()
            {
                Amount = 200,
                Date = DateTime.Today
            }
        };

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetRelevantTransactionsQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactions);

            var vm = CreateVm();

            vm.SelectedBudget = new BudgetItem
            {
                Budget = budget
            };

            await vm.SelectBudget();

            vm.Transactions.Should().HaveCount(2);
        }

        [Fact]
        public async Task SelectBudget_ShouldNotLoadTransactions_WhenNoBudgetSelected()
        {
            var vm = CreateVm();

            await vm.SelectBudget();

            vm.Transactions.Should().BeEmpty();
        }

        [Fact]
        public async Task SelectBudget_ShouldFilterTransactionsByDate()
        {
            var budget = new Budget
            {
                Id = Guid.NewGuid()
            };

            var vm = CreateVm();

            vm.Receive(
                new DateRangeChangedMessage(
                    DateTime.Today.AddDays(-1),
                    DateTime.Today.AddDays(1)));

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetRelevantTransactionsQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Transaction>
                {
                new()
                {
                    Date = DateTime.Today,
                    Amount = 100
                },
                new()
                {
                    Date = DateTime.Today.AddYears(-1),
                    Amount = 200
                }
                });

            vm.SelectedBudget = new BudgetItem
            {
                Budget = budget
            };

            await vm.SelectBudget();

            vm.Transactions.Should().HaveCount(1);
            vm.Transactions.Single().Amount.Should().Be(100);
        }

        [Fact]
        public async Task CreateBudget_ShouldSendCommand()
        {
            var command = new CreateBudgetCommand(
                _userId,
                "Budget",
                1000m,
                DateTime.Today,
                DateTime.Today.AddMonths(1),
                []);

            _popup
                .Setup(x => x.ShowPopUp<CreateBudgetCommand?, BudgetCreatePopUp>())
                .ReturnsAsync(command);

            var vm = CreateVm();

            await vm.CreateBudget();

            _mediator.Verify(x =>
                x.Send(
                    command,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateBudget_ShouldNotSendCommand_WhenPopupReturnsNull()
        {
            _popup
                .Setup(x => x.ShowPopUp<CreateBudgetCommand?, BudgetCreatePopUp>())
                .ReturnsAsync((CreateBudgetCommand?)null);

            var vm = CreateVm();

            await vm.CreateBudget();

            _mediator.Verify(x =>
                x.Send(
                    It.IsAny<CreateBudgetCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Receive_DateRangeChanged_ShouldReloadSelectedBudget()
        {
            var budget = new Budget
            {
                Id = Guid.NewGuid()
            };

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetRelevantTransactionsQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Transaction>
                {
                new()
                {
                    Amount = 100,
                    Date = DateTime.Today
                }
                });

            var vm = CreateVm();

            vm.SelectedBudget = new BudgetItem
            {
                Budget = budget
            };

            vm.Receive(
                new DateRangeChangedMessage(
                    DateTime.Today.AddDays(-1),
                    DateTime.Today.AddDays(1)));

            await Task.Delay(50);

            vm.Transactions.Should().ContainSingle();
        }
    }
}
