using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using BuisnessLogic.UseCases.DebtsUseCasses.Queries;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace BuisnessLogic.Tests.UseCasess
{
    public class GetRelevantTransactionsForDebtQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldThrow_WhenDebtNotFound()
        {
            var debtRepo = new Mock<IDebtRepository>();

            debtRepo
                .Setup(x => x.GetById(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<Expression<Func<Debt, object>>>()))
                .ReturnsAsync((Debt?)null);

            var handler = new GetRelevantTransactionsForDebtQueryHandler(debtRepo.Object);

            var query = new GetRelevantTransactionsForDebtQuery(
                Guid.NewGuid(),
                Guid.NewGuid()
            );

            Func<Task> act = () => handler.Handle(query, default);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*No Debt With Id*");
        }

        [Fact]
        public async Task Handle_ShouldReturnTransactions_WhenDebtExists()
        {
            var userId = Guid.NewGuid();
            var debtId = Guid.NewGuid();

            var expectedTransactions = new List<Transaction>
        {
            new Transaction { Id = Guid.NewGuid(), UserId = userId },
            new Transaction { Id = Guid.NewGuid(), UserId = userId }
        };

            var debt = new Debt
            {
                Id = debtId,
                UserId = userId,
                Transactions = expectedTransactions
            };

            var debtRepo = new Mock<IDebtRepository>();

            debtRepo
                .Setup(x => x.GetById(
                    userId,
                    debtId,
                    It.IsAny<Expression<Func<Debt, object>>>()))
                .ReturnsAsync(debt);

            var handler = new GetRelevantTransactionsForDebtQueryHandler(debtRepo.Object);

            var query = new GetRelevantTransactionsForDebtQuery(userId, debtId);

            var result = await handler.Handle(query, default);

            result.Should().BeEquivalentTo(expectedTransactions);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmpty_WhenDebtHasNoTransactions()
        {
            var userId = Guid.NewGuid();
            var debtId = Guid.NewGuid();

            var debt = new Debt
            {
                Id = debtId,
                UserId = userId,
                Transactions = new List<Transaction>()
            };

            var debtRepo = new Mock<IDebtRepository>();

            debtRepo
                .Setup(x => x.GetById(
                    userId,
                    debtId,
                    It.IsAny<Expression<Func<Debt, object>>>()))
                .ReturnsAsync(debt);

            var handler = new GetRelevantTransactionsForDebtQueryHandler(debtRepo.Object);

            var query = new GetRelevantTransactionsForDebtQuery(userId, debtId);

            var result = await handler.Handle(query, default);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldCallRepository_WithIncludeTransactions()
        {
            var userId = Guid.NewGuid();
            var debtId = Guid.NewGuid();

            Debt? capturedDebt = null;

            var debtRepo = new Mock<IDebtRepository>();

            debtRepo
                .Setup(x => x.GetById(
                    userId,
                    debtId,
                    It.IsAny<Expression<Func<Debt, object>>>()))
                .Callback<Guid, Guid, Expression<Func<Debt, object>>[]>(
                    (u, d, inc) => { })
                .ReturnsAsync(new Debt
                {
                    Id = debtId,
                    UserId = userId,
                    Transactions = new List<Transaction>()
                });

            var handler = new GetRelevantTransactionsForDebtQueryHandler(debtRepo.Object);

            var query = new GetRelevantTransactionsForDebtQuery(userId, debtId);

            var result = await handler.Handle(query, default);

            result.Should().NotBeNull();
        }
    }
}
