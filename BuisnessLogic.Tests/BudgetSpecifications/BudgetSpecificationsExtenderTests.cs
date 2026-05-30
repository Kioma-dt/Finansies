using BuisnessLogic.BudgetService;
using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;
using Xunit;



namespace BuisnessLogic.Tests.BudgetSpecifications
{
    public class BudgetSpecificationsExtenderTests
    {
        private readonly Mock<IBudgetSpecificationsCreatorsProvider> _providerMock;
        private readonly BudgetSpecificationsExtender _sut;

        public BudgetSpecificationsExtenderTests()
        {
            _providerMock = new Mock<IBudgetSpecificationsCreatorsProvider>();

            _sut = new BudgetSpecificationsExtender(
                _providerMock.Object);
        }

        [Fact]
        public void GetFullExpression_WhenBudgetHasNoFilters_ShouldUseOnlyBaseFilter()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var budget = new Budget
            {
                UserId = userId,
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 12, 31),
                Filters = []
            };

            // Act
            var expression = _sut.GetFullExpression(budget);

            var predicate = expression.Compile();

            // Assert
            predicate(new Transaction
            {
                UserId = userId,
                Date = new DateTime(2025, 6, 1)
            }).Should().BeTrue();
        }

        [Fact]
        public void GetFullExpression_WhenUserDoesNotMatch_ShouldReturnFalse()
        {
            // Arrange
            var budget = new Budget
            {
                UserId = Guid.NewGuid(),
                StartDate = DateTime.MinValue,
                EndDate = DateTime.MaxValue,
                Filters = []
            };

            var predicate =
                _sut.GetFullExpression(budget).Compile();

            // Act
            var result = predicate(new Transaction
            {
                UserId = Guid.NewGuid(),
                Date = DateTime.Now
            });

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GetFullExpression_WhenDateBeforeStartDate_ShouldReturnFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var budget = new Budget
            {
                UserId = userId,
                StartDate = new DateTime(2025, 5, 1),
                EndDate = new DateTime(2025, 5, 31),
                Filters = []
            };

            var predicate =
                _sut.GetFullExpression(budget).Compile();

            // Act
            var result = predicate(new Transaction
            {
                UserId = userId,
                Date = new DateTime(2025, 4, 30)
            });

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GetFullExpression_WhenDateAfterEndDate_ShouldReturnFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var budget = new Budget
            {
                UserId = userId,
                StartDate = new DateTime(2025, 5, 1),
                EndDate = new DateTime(2025, 5, 31),
                Filters = []
            };

            var predicate =
                _sut.GetFullExpression(budget).Compile();

            // Act
            var result = predicate(new Transaction
            {
                UserId = userId,
                Date = new DateTime(2025, 6, 1)
            });

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("2025-05-01")]
        [InlineData("2025-05-31")]
        public void GetFullExpression_ShouldIncludeDateBorders(
    string dateValue)
        {
            // Arrange
            var userId = Guid.NewGuid();

            var budget = new Budget
            {
                UserId = userId,
                StartDate = new DateTime(2025, 5, 1),
                EndDate = new DateTime(2025, 5, 31),
                Filters = []
            };

            var predicate =
                _sut.GetFullExpression(budget).Compile();

            // Act
            var result = predicate(new Transaction
            {
                UserId = userId,
                Date = DateTime.Parse(dateValue)
            });

            // Assert
            result.Should().BeTrue();
        }

     
        [Fact]
        public void GetFullExpression_ShouldRequestCreatorForEachFilter()
        {
            // Arrange
            var creatorMock = new Mock<IBudgetSpecificationsCreator>();

            creatorMock
                .Setup(x => x.Create(
                    It.IsAny<ParameterExpression>(),
                    It.IsAny<BudgetFilter>()))
                .Returns(Expression.Constant(true));

            _providerMock
                .Setup(x => x.Get(It.IsAny<BudgetFilterType>()))
                .Returns(creatorMock.Object);

            var budget = new Budget
            {
                UserId = Guid.NewGuid(),
                StartDate = DateTime.MinValue,
                EndDate = DateTime.MaxValue,
                Filters =
                [
                    new BudgetFilter
            {
                Type = BudgetFilterType.Account
            },
            new BudgetFilter
            {
                Type = BudgetFilterType.Account
            },
            new BudgetFilter
            {
                Type = BudgetFilterType.Category
            }
                ]
            };

            // Act
            _sut.GetFullExpression(budget);

            // Assert
            _providerMock.Verify(
                x => x.Get(It.IsAny<BudgetFilterType>()),
                Times.Exactly(3));

            creatorMock.Verify(
                x => x.Create(
                    It.IsAny<ParameterExpression>(),
                    It.IsAny<BudgetFilter>()),
                Times.Exactly(3));
        }

     

 
       

       
    }
}
