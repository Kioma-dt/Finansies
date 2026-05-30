using BuisnessLogic.BudgetService;
using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using FluentAssertions;
using System.Linq.Expressions;
using Xunit;


namespace BuisnessLogic.Tests.BudgetSpecifications
{
    public class BudgetTransactionTypeSpecificationsCreatorTests
    {
        private readonly BudgetTransactionTypeSpecificationsCreator _creator = new();

        [Fact]
        public void Type_ShouldReturnTransactionType()
        {
            _creator.Type.Should().Be(BudgetFilterType.TransactionType);
        }

        [Theory]
        [InlineData(TransactionType.Income)]
        [InlineData(TransactionType.Expense)]
        public void Create_WhenTransactionTypeMatches_ShouldReturnTrue(
            TransactionType type)
        {
            // Arrange
            var filter = new BudgetFilter
            {
                Value = type.ToString()
            };

            var parameter = Expression.Parameter(typeof(Transaction));

            var expression = _creator.Create(parameter, filter);

            var predicate = Expression
                .Lambda<Func<Transaction, bool>>(expression, parameter)
                .Compile();

            // Act & Assert
            predicate(new Transaction
            {
                Type = type
            }).Should().BeTrue();
        }

        [Fact]
        public void Create_WhenTransactionTypeDoesNotMatch_ShouldReturnFalse()
        {
            // Arrange
            var filter = new BudgetFilter
            {
                Value = TransactionType.Income.ToString()
            };

            var parameter = Expression.Parameter(typeof(Transaction));

            var expression = _creator.Create(parameter, filter);

            var predicate = Expression
                .Lambda<Func<Transaction, bool>>(expression, parameter)
                .Compile();

            // Act & Assert
            predicate(new Transaction
            {
                Type = TransactionType.Expense
            }).Should().BeFalse();
        }

        [Fact]
        public void Create_WhenEnumValueIsInvalid_ShouldThrowArgumentException()
        {
            // Arrange
            var filter = new BudgetFilter
            {
                Value = "INVALID_ENUM"
            };

            var parameter = Expression.Parameter(typeof(Transaction));

            // Act
            Action act = () => _creator.Create(parameter, filter);

            // Assert
            act.Should().Throw<ArgumentException>();
        }
    }
}
