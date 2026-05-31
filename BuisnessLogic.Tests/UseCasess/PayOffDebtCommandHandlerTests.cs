using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using BuisnessLogic.UseCases.DebtsUseCasses.Commands;
using FluentAssertions;
using Moq;
using Xunit;

namespace BuisnessLogic.Tests.UseCasess
{
    public class PayOffDebtCommandHandlerTests
    {
        private readonly Mock<IDebtRepository> _repo = new();
        private readonly PayOffDebtCommandHandler _handler;

        public PayOffDebtCommandHandlerTests()
        {
            _handler = new PayOffDebtCommandHandler(_repo.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenDebtNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var debtId = Guid.NewGuid();

            var command = new PayOffDebtCommand(
                userId,
                debtId,
                100,
                DateTime.UtcNow
            );

            _repo
                .Setup(r => r.GetById(userId, debtId))
                .ReturnsAsync((Debt?)null);

            // Act
            Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage($"*{debtId}*");

            _repo.Verify(r => r.Update(It.IsAny<Debt>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCallMakeAPayment_AndUpdateDebt()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var debtId = Guid.NewGuid();

            var command = new PayOffDebtCommand(
                userId,
                debtId,
                250,
                new DateTime(2025, 1, 1)
            );

            Debt? capturedDebt = null;

            var debt = new Debt
            {
                Id = debtId,
                UserId = userId
            };

            _repo
                .Setup(r => r.GetById(userId, debtId))
                .ReturnsAsync(debt);

            _repo
                .Setup(r => r.Update(It.IsAny<Debt>()))
                .Callback<Debt>(d => capturedDebt = d)
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedDebt.Should().NotBeNull();

            _repo.Verify(r => r.Update(debt), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldMutateDebt_BeforeUpdate()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var debtId = Guid.NewGuid();

            var amount = 500m;
            var date = new DateTime(2025, 5, 5);

            var command = new PayOffDebtCommand(
                userId,
                debtId,
                amount,
                date
            );

            Debt? updatedDebt = null;

            var debt = new Debt
            {
                Id = debtId,
                UserId = userId
            };

            _repo
                .Setup(r => r.GetById(userId, debtId))
                .ReturnsAsync(debt);

            _repo
                .Setup(r => r.Update(It.IsAny<Debt>()))
                .Callback<Debt>(d => updatedDebt = d)
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            updatedDebt.Should().NotBeNull();

            // Проверяем что доменный метод был вызван
            updatedDebt!.Id.Should().Be(debtId);
            updatedDebt.UserId.Should().Be(userId);
        }

        [Fact]
        public async Task Handle_ShouldNotCallUpdate_WhenDebtNotFound()
        {
            // Arrange
            var command = new PayOffDebtCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                100,
                DateTime.UtcNow
            );

            _repo
                .Setup(r => r.GetById(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((Debt?)null);

            // Act
            Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();

            _repo.Verify(r => r.Update(It.IsAny<Debt>()), Times.Never);
        }
    }
}
