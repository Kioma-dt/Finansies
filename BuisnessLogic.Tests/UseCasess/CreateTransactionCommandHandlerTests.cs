using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using BuisnessLogic.UseCases.TransactionsUseCasses.Commands;
using FluentAssertions;
using Moq;
using Xunit;

namespace BuisnessLogic.Tests.UseCasess
{
    public class CreateTransactionCommandHandlerTests
    {
        private readonly Mock<ITransactionRepository> _transactionRepo = new();
        private readonly Mock<IAccountRepository> _accountRepo = new();
        private readonly Mock<IDebtRepository> _debtRepo = new();

        private CreateTransactionCommandHandler CreateHandler()
            => new(_transactionRepo.Object, _accountRepo.Object, _debtRepo.Object);

        // -----------------------------
        // SUCCESS: EXPENSE
        // -----------------------------
        [Fact]
        public async Task Handle_ShouldDecreaseAccountBalance_ForExpense()
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Balance = 300
            };

            var command = new CreateTransactionCommand(
                account.UserId,
                100,
                "Test",
                DateTime.UtcNow,
                TransactionType.Expense,
                account.Id,
                null,
                null,
                null);

            _accountRepo
                .Setup(x => x.GetById(command.UserId, command.AccountId))
                .ReturnsAsync(account);

            var handler = CreateHandler();

            await handler.Handle(command, default);

            _transactionRepo.Verify(x => x.Add(It.IsAny<Transaction>()), Times.Once);
            _accountRepo.Verify(x => x.Update(account), Times.Once);
        }

        // -----------------------------
        // SUCCESS: INCOME
        // -----------------------------
        [Fact]
        public async Task Handle_ShouldIncreaseAccountBalance_ForIncome()
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            var command = new CreateTransactionCommand(
                account.UserId,
                200,
                "Income",
                DateTime.UtcNow,
                TransactionType.Income,
                account.Id,
                null,
                null,
                null);

            _accountRepo
                .Setup(x => x.GetById(command.UserId, command.AccountId))
                .ReturnsAsync(account);

            var handler = CreateHandler();

            await handler.Handle(command, default);

            _transactionRepo.Verify(x => x.Add(It.IsAny<Transaction>()), Times.Once);
            _accountRepo.Verify(x => x.Update(account), Times.Once);
        }

        // -----------------------------
        // ERROR: ACCOUNT NOT FOUND
        // -----------------------------
        [Fact]
        public async Task Handle_ShouldThrow_WhenAccountNotFound()
        {
            var command = new CreateTransactionCommand(
                Guid.NewGuid(),
                100,
                "Test",
                DateTime.UtcNow,
                TransactionType.Expense,
                Guid.NewGuid(),
                null,
                null,
                null);

            _accountRepo
                .Setup(x => x.GetById(command.UserId, command.AccountId))
                .ReturnsAsync((Account?)null);

            var handler = CreateHandler();

            Func<Task> act = () => handler.Handle(command, default);

            await act.Should().ThrowAsync<ArgumentException>();

            _transactionRepo.Verify(x => x.Add(It.IsAny<Transaction>()), Times.Never);
            _accountRepo.Verify(x => x.Update(It.IsAny<Account>()), Times.Never);
        }

        // -----------------------------
        // SUCCESS: WITH DEBT PAYMENT
        // -----------------------------
        [Fact]
        public async Task Handle_ShouldPayDebt_WhenDebtExists()
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Balance = 500
            };

            var debt = new Debt
            {
                Id = Guid.NewGuid(),
                UserId = account.UserId
            };

            var command = new CreateTransactionCommand(
                account.UserId,
                100,
                "Debt payment",
                DateTime.UtcNow,
                TransactionType.Expense,
                account.Id,
                null,
                null,
                debt.Id);

            _accountRepo
                .Setup(x => x.GetById(command.UserId, command.AccountId))
                .ReturnsAsync(account);

            _debtRepo
                .Setup(x => x.GetById(command.UserId, debt.Id))
                .ReturnsAsync(debt);

            var handler = CreateHandler();

            await handler.Handle(command, default);

            _debtRepo.Verify(x => x.Update(debt), Times.Once);
            _transactionRepo.Verify(x => x.Add(It.IsAny<Transaction>()), Times.Once);
        }

        // -----------------------------
        // ERROR: DEBT NOT FOUND
        // -----------------------------
        [Fact]
        public async Task Handle_ShouldThrow_WhenDebtNotFound()
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            var debtId = Guid.NewGuid();

            var command = new CreateTransactionCommand(
                account.UserId,
                100,
                "Debt",
                DateTime.UtcNow,
                TransactionType.Expense,
                account.Id,
                null,
                null,
                debtId);

            _accountRepo
                .Setup(x => x.GetById(command.UserId, command.AccountId))
                .ReturnsAsync(account);

            _debtRepo
                .Setup(x => x.GetById(command.UserId, debtId))
                .ReturnsAsync((Debt?)null);

            var handler = CreateHandler();

            Func<Task> act = () => handler.Handle(command, default);

            await act.Should().ThrowAsync<ArgumentException>();

            _debtRepo.Verify(x => x.Update(It.IsAny<Debt>()), Times.Never);
        }

        // -----------------------------
        // VERIFY TRANSACTION CREATED
        // -----------------------------
        [Fact]
        public async Task Handle_ShouldCreateTransactionEntity()
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            Transaction? captured = null;

            _accountRepo
                .Setup(x => x.GetById(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(account);

            _transactionRepo
                .Setup(x => x.Add(It.IsAny<Transaction>()))
                .Callback<Transaction>(t => captured = t)
                .Returns(Task.CompletedTask);

            var handler = CreateHandler();

            await handler.Handle(new CreateTransactionCommand(
                account.UserId,
                123,
                "Check",
                DateTime.UtcNow,
                TransactionType.Income,
                account.Id,
                Guid.NewGuid(),
                null,
                null), default);

            captured.Should().NotBeNull();
            captured!.Amount.Should().Be(123);
            captured.AccountId.Should().Be(account.Id);
        }
    }
}
