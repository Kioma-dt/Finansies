using BuisnessLogic.BudgetService;
using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace BuisnessLogic.Tests.BudgetSpecifications
{
    public class BudgetAccountSpecificationsCreatorTests
    {
        private readonly BudgetAccountSpecificationsCreator _creator = new();

        [Fact]
        public void Create_WhenAccountMatches_ShouldReturnTrue()
        {
            // Arrange
            var accountId = Guid.NewGuid();

            var filter = new BudgetFilter
            {
                Value = accountId.ToString()
            };

            var parameter = Expression.Parameter(typeof(Transaction));

            var expression = _creator.Create(parameter, filter);

            var lambda = Expression
                .Lambda<Func<Transaction, bool>>(expression, parameter)
                .Compile();

            var transaction = new Transaction
            {
                AccountId = accountId
            };

            // Act
            var result = lambda(transaction);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Create_WhenAccountDoesNotMatch_ShouldReturnFalse()
        {
            // Arrange
            var accountId = Guid.NewGuid();

            var filter = new BudgetFilter
            {
                Value = accountId.ToString()
            };

            var parameter = Expression.Parameter(typeof(Transaction));

            var expression = _creator.Create(parameter, filter);

            var lambda = Expression
                .Lambda<Func<Transaction, bool>>(expression, parameter)
                .Compile();

            var transaction = new Transaction
            {
                AccountId = Guid.NewGuid()
            };

            // Act
            var result = lambda(transaction);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Type_ShouldReturnAccount()
        {
            _creator.Type.Should().Be(BudgetFilterType.Account);
        }

        [Fact]
        public void Create_WhenGuidIsInvalid_ShouldThrowFormatException()
        {
            var filter = new BudgetFilter
            {
                Value = "invalid-guid"
            };

            var parameter = Expression.Parameter(typeof(Transaction));

            Action act = () => _creator.Create(parameter, filter);

            act.Should().Throw<FormatException>();
        }
    }
}
