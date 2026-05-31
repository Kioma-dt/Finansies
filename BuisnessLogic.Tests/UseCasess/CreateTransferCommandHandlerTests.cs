using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using BuisnessLogic.UseCases.TransfersUseCasses.Commands;
using FluentAssertions;
using Moq;
using Xunit;

namespace BuisnessLogic.Tests.UseCasess
{
    public class CreateTransferCommandHandlerTests
    {
        private readonly Mock<ITransferRepository> _transferRepo = new();
        private readonly Mock<IAccountRepository> _accountRepo = new();

        private CreateTransferCommandHandler CreateHandler()
            => new(_transferRepo.Object, _accountRepo.Object);

        // -----------------------------
        // SUCCESS: BASIC TRANSFER
        // -----------------------------
        [Fact]
        public async Task Handle_ShouldTransferMoneyBetweenAccounts()
        {
            var userId = Guid.NewGuid();

            var from = new Account { Id = Guid.NewGuid(), UserId = userId, Balance = 1000 };
            var to = new Account { Id = Guid.NewGuid(), UserId = userId, Balance = 1000 };

            _accountRepo.Setup(x => x.GetById(userId, from.Id))
                .ReturnsAsync(from);

            _accountRepo.Setup(x => x.GetById(userId, to.Id))
                .ReturnsAsync(to);

            var command = new CreateTransferCommand(
                userId,
                100,
                "Transfer",
                DateTime.UtcNow,
                from.Id,
                to.Id);

            var handler = CreateHandler();

            await handler.Handle(command, default);

            _transferRepo.Verify(x => x.Add(It.IsAny<Transfer>()), Times.Once);

            _accountRepo.Verify(x => x.Update(from), Times.Once);
            _accountRepo.Verify(x => x.Update(to), Times.Once);
        }

        // -----------------------------
        // ERROR: FROM ACCOUNT NOT FOUND
        // -----------------------------
        [Fact]
        public async Task Handle_ShouldThrow_WhenFromAccountNotFound()
        {
            var userId = Guid.NewGuid();

            var fromId = Guid.NewGuid();
            var to = new Account { Id = Guid.NewGuid(), UserId = userId };

            _accountRepo.Setup(x => x.GetById(userId, fromId))
                .ReturnsAsync((Account?)null);

            _accountRepo.Setup(x => x.GetById(userId, to.Id))
                .ReturnsAsync(to);

            var command = new CreateTransferCommand(
                userId,
                100,
                "Transfer",
                DateTime.UtcNow,
                fromId,
                to.Id);

            var handler = CreateHandler();

            Func<Task> act = () => handler.Handle(command, default);

            await act.Should().ThrowAsync<ArgumentException>();

            _transferRepo.Verify(x => x.Add(It.IsAny<Transfer>()), Times.Never);
        }

        // -----------------------------
        // ERROR: TO ACCOUNT NOT FOUND
        // -----------------------------
        [Fact]
        public async Task Handle_ShouldThrow_WhenToAccountNotFound()
        {
            var userId = Guid.NewGuid();

            var from = new Account { Id = Guid.NewGuid(), UserId = userId, Balance = 1000 };
            var toId = Guid.NewGuid();

            _accountRepo.Setup(x => x.GetById(userId, from.Id))
                .ReturnsAsync(from);

            _accountRepo.Setup(x => x.GetById(userId, toId))
                .ReturnsAsync((Account?)null);

            var command = new CreateTransferCommand(
                userId,
                100,
                "Transfer",
                DateTime.UtcNow,
                from.Id,
                toId);

            var handler = CreateHandler();

            Func<Task> act = () => handler.Handle(command, default);

            await act.Should().ThrowAsync<ArgumentException>();

            _transferRepo.Verify(x => x.Add(It.IsAny<Transfer>()), Times.Never);
        }

        // -----------------------------
        // VERIFY BALANCE CHANGES
        // -----------------------------
        [Fact]
        public async Task Handle_ShouldDecreaseFromAccount_AndIncreaseToAccount()
        {
            var userId = Guid.NewGuid();

            var from = new Account { Id = Guid.NewGuid(), UserId = userId, Balance = 1000 };
            var to = new Account { Id = Guid.NewGuid(), UserId = userId , Balance = 500 };

            _accountRepo.Setup(x => x.GetById(userId, from.Id))
                .ReturnsAsync(from);

            _accountRepo.Setup(x => x.GetById(userId, to.Id))
                .ReturnsAsync(to);

            _transferRepo.Setup(x => x.Add(It.IsAny<Transfer>()))
                .Returns(Task.CompletedTask);

            var handler = CreateHandler();

            await handler.Handle(new CreateTransferCommand(
                userId,
                250,
                "Move money",
                DateTime.UtcNow,
                from.Id,
                to.Id), default);

            _accountRepo.Verify(x => x.Update(from), Times.Once);
            _accountRepo.Verify(x => x.Update(to), Times.Once);
        }

        // -----------------------------
        // VERIFY TRANSFER ENTITY
        // -----------------------------
        [Fact]
        public async Task Handle_ShouldCreateTransferEntity()
        {
            var userId = Guid.NewGuid();

            var from = new Account { Id = Guid.NewGuid(), UserId = userId, Balance = 1000 };
            var to = new Account { Id = Guid.NewGuid(), UserId = userId, Balance = 1000 };

            Transfer? captured = null;

            _accountRepo.Setup(x => x.GetById(userId, from.Id))
                .ReturnsAsync(from);

            _accountRepo.Setup(x => x.GetById(userId, to.Id))
                .ReturnsAsync(to);

            _transferRepo.Setup(x => x.Add(It.IsAny<Transfer>()))
                .Callback<Transfer>(t => captured = t)
                .Returns(Task.CompletedTask);

            var handler = CreateHandler();

            await handler.Handle(new CreateTransferCommand(
                userId,
                999,
                "Check transfer",
                DateTime.UtcNow,
                from.Id,
                to.Id), default);

            captured.Should().NotBeNull();
            captured!.Amount.Should().Be(999);
            captured.FromAccountId.Should().Be(from.Id);
            captured.ToAccountId.Should().Be(to.Id);
        }
    }
}
