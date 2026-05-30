using BuisnessLogic.BudgetService;
using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using FluentAssertions;
using System.Linq.Expressions;
using Xunit;


namespace BuisnessLogic.Tests.BudgetSpecifications
{
    public class BudgetFamilyMemberSpecificationsCreatorTests
    {
        private readonly BudgetFamilyMemberSpecificationsCreator _creator = new();

        [Fact]
        public void Type_ShouldReturnFamilyMember()
        {
            _creator.Type.Should().Be(BudgetFilterType.FamilyMember);
        }

        [Fact]
        public void Create_WhenFamilyMemberMatches_ShouldReturnTrue()
        {
            // Arrange
            var familyMemberId = Guid.NewGuid();

            var filter = new BudgetFilter
            {
                Value = familyMemberId.ToString()
            };

            var parameter = Expression.Parameter(typeof(Transaction));

            var expression = _creator.Create(parameter, filter);

            var predicate = Expression
                .Lambda<Func<Transaction, bool>>(expression, parameter)
                .Compile();

            // Act & Assert
            predicate(new Transaction
            {
                FamilyMemberId = familyMemberId
            }).Should().BeTrue();
        }

        [Fact]
        public void Create_WhenFamilyMemberDoesNotMatch_ShouldReturnFalse()
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
                FamilyMemberId = Guid.NewGuid()
            }).Should().BeFalse();
        }

        [Fact]
        public void Create_WhenValueIsInvalidGuid_ShouldThrowFormatException()
        {
            // Arrange
            var filter = new BudgetFilter
            {
                Value = "wrong-guid"
            };

            var parameter = Expression.Parameter(typeof(Transaction));

            // Act
            Action act = () => _creator.Create(parameter, filter);

            // Assert
            act.Should().Throw<FormatException>();
        }
    }
}
