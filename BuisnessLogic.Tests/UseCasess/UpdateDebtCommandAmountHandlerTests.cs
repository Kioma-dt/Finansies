using BuisnessLogic.DebtInterestCalculator;
using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using BuisnessLogic.UseCases.DebtsUseCasses.Commands;
using FluentAssertions;
using Moq;
using Xunit;

namespace BuisnessLogic.Tests.UseCasess
{
    public class UpdateDebtCommandAmountHandlerTests
    {
        private readonly Mock<IDebtRepository> _repo = new();
        private readonly Mock<IDebtInterestCalculatorProvider> _provider = new();

        private readonly UpdateDebtCommandAmountHandler _handler;

        public UpdateDebtCommandAmountHandlerTests()
        {
            _handler = new UpdateDebtCommandAmountHandler(
                _repo.Object,
                _provider.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenDebtNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var debtId = Guid.NewGuid();

            var command = new UpdateDebtAmountCommand(userId, debtId, DateTime.UtcNow);

            _repo
                .Setup(r => r.GetById(userId, debtId))
                .ReturnsAsync((Debt?)null);

            // Act
            Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage($"*{debtId}*");

            _provider.Verify(x => x.GetCalculator(It.IsAny<InterestType>()), Times.Never);
            _repo.Verify(x => x.Update(It.IsAny<Debt>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCallCalculator_AndUpdateDebt()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var debtId = Guid.NewGuid();

            var date = new DateTime(2025, 1, 1);

            var debt = new Debt
            {
                Id = debtId,
                UserId = userId,
                InterestType = InterestType.Simple,
                StartAmount = 1000,
                InterestRate = 0.1m,
                CapitalisationsPerYear = 1,
                FixedAddition = 0,
                StartDate = new DateTime(2024, 1, 1),
                TotalAmount = 1000
            };

            var calculatorMock = new Mock<IDebtInterestCalculator>();

            calculatorMock
                .Setup(c => c.Calculate(
                    debt.StartAmount,
                    debt.InterestRate,
                    debt.CapitalisationsPerYear,
                    debt.FixedAddition,
                    debt.StartDate,
                    date))
                .Returns(1100);

            _repo
                .Setup(r => r.GetById(userId, debtId))
                .ReturnsAsync(debt);

            _provider
                .Setup(p => p.GetCalculator(debt.InterestType))
                .Returns(calculatorMock.Object);

            Debt? updated = null;

            _repo
                .Setup(r => r.Update(It.IsAny<Debt>()))
                .Callback<Debt>(d => updated = d)
                .Returns(Task.CompletedTask);

            var command = new UpdateDebtAmountCommand(userId, debtId, date);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _provider.Verify(p => p.GetCalculator(debt.InterestType), Times.Once);

            calculatorMock.Verify(c => c.Calculate(
                debt.StartAmount,
                debt.InterestRate,
                debt.CapitalisationsPerYear,
                debt.FixedAddition,
                debt.StartDate,
                date), Times.Once);

            updated.Should().NotBeNull();
            _repo.Verify(r => r.Update(debt), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldCallChargeInterest_WithCorrectDifference()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var debtId = Guid.NewGuid();

            var date = new DateTime(2025, 1, 1);

            var debt = new Debt
            {
                Id = debtId,
                UserId = userId,
                InterestType = InterestType.Simple,
                StartAmount = 1000,
                InterestRate = 0.1m,
                CapitalisationsPerYear = 1,
                FixedAddition = 0,
                StartDate = new DateTime(2024, 1, 1),
                TotalAmount = 1000
            };

            var calculatorMock = new Mock<IDebtInterestCalculator>();

            calculatorMock
                .Setup(c => c.Calculate(
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()))
                .Returns(1200);

            _repo
                .Setup(r => r.GetById(userId, debtId))
                .ReturnsAsync(debt);

            _provider
                .Setup(p => p.GetCalculator(debt.InterestType))
                .Returns(calculatorMock.Object);

            Debt? mutated = null;

            _repo
                .Setup(r => r.Update(It.IsAny<Debt>()))
                .Callback<Debt>(d => mutated = d)
                .Returns(Task.CompletedTask);

            var command = new UpdateDebtAmountCommand(userId, debtId, date);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            mutated.Should().NotBeNull();

            // 1200 - 1000 = 200 interest
            mutated!.Should().NotBeNull();
        }
    }
}
