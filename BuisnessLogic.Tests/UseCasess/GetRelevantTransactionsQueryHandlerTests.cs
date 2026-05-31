using BuisnessLogic.BudgetService;
using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using BuisnessLogic.UseCases.BudgetUseCasses.Queries;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace BuisnessLogic.Tests.UseCasess
{
    public class GetRelevantTransactionsQueryHandlerTests
    {
        private readonly Mock<IBudgetRepository> _budgetRepo = new();
        private readonly Mock<ITransactionRepository> _transactionRepo = new();
        private readonly Mock<IBudgetSpecificationsExtender> _extender = new();

        private readonly GetRelevantTransactionsQueryHandler _handler;

        public GetRelevantTransactionsQueryHandlerTests()
        {
            _handler = new GetRelevantTransactionsQueryHandler(
                _budgetRepo.Object,
                _transactionRepo.Object,
                _extender.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenBudgetNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var budgetId = Guid.NewGuid();

            var query = new GetRelevantTransactionsQuery(userId, budgetId);

            _budgetRepo
                .Setup(r => r.GetById(userId, budgetId, It.IsAny<Expression<Func<Budget, object>>[]>()))
                .ReturnsAsync((Budget?)null);

            // Act
            Func<Task> act = () => _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage($"*{budgetId}*");

            _extender.Verify(x => x.GetFullExpression(It.IsAny<Budget>()), Times.Never);
            _transactionRepo.Verify(x => x.GetWithSpecification(It.IsAny<Guid>(), It.IsAny<Expression<Func<Transaction, bool>>>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCallExtenderAndRepository_WhenBudgetExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var budgetId = Guid.NewGuid();

            var budget = new Budget
            {
                Id = budgetId,
                UserId = userId,
                Filters = new List<BudgetFilter>()
            };

            var query = new GetRelevantTransactionsQuery(userId, budgetId);

            Expression<Func<Transaction, bool>> fakeExpression = t => t.Amount > 100;

            _budgetRepo
                .Setup(r => r.GetById(userId, budgetId, It.IsAny<Expression<Func<Budget, object>>[]>()))
                .ReturnsAsync(budget);

            _extender
                .Setup(x => x.GetFullExpression(budget))
                .Returns(fakeExpression);

            _transactionRepo
                .Setup(r => r.GetWithSpecification(userId, fakeExpression))
                .ReturnsAsync(new List<Transaction>());

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _extender.Verify(x => x.GetFullExpression(budget), Times.Once);

            _transactionRepo.Verify(x =>
                x.GetWithSpecification(userId, fakeExpression),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnTransactions_FromRepository()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var budgetId = Guid.NewGuid();

            var budget = new Budget
            {
                Id = budgetId,
                UserId = userId,
                Filters = new List<BudgetFilter>()
            };

            var query = new GetRelevantTransactionsQuery(userId, budgetId);

            var expected = new List<Transaction>
        {
            new Transaction { Id = Guid.NewGuid(), UserId = userId },
            new Transaction { Id = Guid.NewGuid(), UserId = userId }
        };

            Expression<Func<Transaction, bool>> expression = t => true;

            _budgetRepo
                .Setup(r => r.GetById(userId, budgetId, It.IsAny<Expression<Func<Budget, object>>[]>()))
                .ReturnsAsync(budget);

            _extender
                .Setup(x => x.GetFullExpression(budget))
                .Returns(expression);

            _transactionRepo
                .Setup(r => r.GetWithSpecification(userId, expression))
                .ReturnsAsync(expected);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Handle_ShouldPassCorrectUserId_ToTransactionRepository()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var budgetId = Guid.NewGuid();

            var budget = new Budget
            {
                Id = budgetId,
                UserId = userId,
                Filters = new List<BudgetFilter>()
            };

            var query = new GetRelevantTransactionsQuery(userId, budgetId);

            Expression<Func<Transaction, bool>> expression = t => true;

            _budgetRepo
                .Setup(r => r.GetById(userId, budgetId, It.IsAny<Expression<Func<Budget, object>>[]>()))
                .ReturnsAsync(budget);

            _extender
                .Setup(x => x.GetFullExpression(It.IsAny<Budget>()))
                .Returns(expression);

            _transactionRepo
                .Setup(r => r.GetWithSpecification(userId, expression))
                .ReturnsAsync(new List<Transaction>());

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _transactionRepo.Verify(x =>
                x.GetWithSpecification(
                    It.Is<Guid>(u => u == userId),
                    expression),
                Times.Once);
        }
    }
}
