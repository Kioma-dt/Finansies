using BuisnessLogic.BudgetService;
using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using FluentAssertions;
using System.Linq.Expressions;
using Xunit;


namespace BuisnessLogic.Tests.BudgetSpecifications
{
    public class BudgetCategorySpecificationsCreatorTests
    {
        private readonly BudgetCategorySpecificationsCreator _creator = new();

        [Fact]
        public void Type_ShouldReturnCategory()
        {
            _creator.Type.Should().Be(BudgetFilterType.Category);
        }

        [Fact]
        public void Create_WhenCategoryMatches_ShouldReturnTrue()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            var filter = new BudgetFilter
            {
                Type = BudgetFilterType.Category,
                Value = categoryId.ToString()
            };

            var parameter = Expression.Parameter(typeof(Transaction));

            // Act
            var expression = _creator.Create(parameter, filter);

            var predicate = Expression
                .Lambda<Func<Transaction, bool>>(expression, parameter)
                .Compile();

            // Assert
            predicate(new Transaction
            {
                CategoryId = categoryId
            }).Should().BeTrue();
        }

        [Fact]
        public void Create_WhenCategoryDoesNotMatch_ShouldReturnFalse()
        {
            // Arrange
            var filter = new BudgetFilter
            {
                Value = Guid.NewGuid().ToString()
            };

            var parameter = Expression.Parameter(typeof(Transaction));

            var expression = _creator.Create(parameter, filter);

            var predicate = Expression
                .Lambda<Func<Transaction, bool>>(expression, parameter)
                .Compile();

            // Act & Assert
            predicate(new Transaction
            {
                CategoryId = Guid.NewGuid()
            }).Should().BeFalse();
        }

        [Fact]
        public void Create_WhenValueIsNotGuid_ShouldThrowFormatException()
        {
            // Arrange
            var filter = new BudgetFilter
            {
                Value = "invalid-guid"
            };

            var parameter = Expression.Parameter(typeof(Transaction));

            // Act
            Action act = () => _creator.Create(parameter, filter);

            // Assert
            act.Should().Throw<FormatException>();
        }
    }
}
