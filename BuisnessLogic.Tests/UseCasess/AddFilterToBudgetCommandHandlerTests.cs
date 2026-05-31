using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using BuisnessLogic.UseCases.BudgetUseCasses.Commands;
using FluentAssertions;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BuisnessLogic.Tests.UseCasess
{


    public class AddFilterToBudgetCommandHandlerTests
    {
        private readonly Mock<IBudgetRepository> _budgetRepositoryMock;
        private readonly AddFilterToBudgetCommandHandler _handler;

        public AddFilterToBudgetCommandHandlerTests()
        {
            _budgetRepositoryMock = new Mock<IBudgetRepository>();
            _handler = new AddFilterToBudgetCommandHandler(_budgetRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenBudgetDoesNotExist()
        {
            // Arrange
            var command = new AddFilterToBudgetCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                BudgetFilterType.Account,
                "value"
            );

            _budgetRepositoryMock
                .Setup(r => r.GetById(command.UserId, command.BudgetId, It.IsAny<Expression<Func<Budget, object>>[]>()))
                .ReturnsAsync((Budget?)null);

            // Act
            Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage($"*{command.BudgetId}*");

            _budgetRepositoryMock.Verify(
                r => r.AddBudgetFilter(It.IsAny<Guid>(), It.IsAny<BudgetFilter>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCallAddBudgetFilter_WhenBudgetExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var budgetId = Guid.NewGuid();

            var command = new AddFilterToBudgetCommand(
                userId,
                budgetId,
                BudgetFilterType.Category,
                "some-value"
            );

            var budget = new Budget
            {
                Id = budgetId,
                UserId = userId,
                Filters = new List<BudgetFilter>()
            };

            _budgetRepositoryMock
                .Setup(r => r.GetById(userId, budgetId, It.IsAny<Expression<Func<Budget, object>>[]>()))
                .ReturnsAsync(budget);

            _budgetRepositoryMock
                .Setup(r => r.AddBudgetFilter(
                    userId,
                    It.IsAny<BudgetFilter>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _budgetRepositoryMock.Verify(
                r => r.AddBudgetFilter(
                    userId,
                    It.Is<BudgetFilter>(f =>
                        f.BudgetId == budgetId &&
                        f.Type == BudgetFilterType.Category &&
                        f.Value == "some-value"
                    )),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldCreateBudgetFilterWithCorrectValues()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var budgetId = Guid.NewGuid();

            BudgetFilter? capturedFilter = null;

            var command = new AddFilterToBudgetCommand(
                userId,
                budgetId,
                BudgetFilterType.TransactionType,
                "Income"
            );

            var budget = new Budget
            {
                Id = budgetId,
                UserId = userId,
                Filters = new List<BudgetFilter>()
            };

            _budgetRepositoryMock
                .Setup(r => r.GetById(userId, budgetId, It.IsAny<Expression<Func<Budget, object>>[]>()))
                .ReturnsAsync(budget);

            _budgetRepositoryMock
                .Setup(r => r.AddBudgetFilter(userId, It.IsAny<BudgetFilter>()))
                .Callback<Guid, BudgetFilter>((_, filter) =>
                {
                    capturedFilter = filter;
                })
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedFilter.Should().NotBeNull();
            capturedFilter!.BudgetId.Should().Be(budgetId);
            capturedFilter.Type.Should().Be(BudgetFilterType.TransactionType);
            capturedFilter.Value.Should().Be("Income");
        }
    }
}
