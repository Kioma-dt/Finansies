using BuisnessLogic.BudgetService;
using BuisnessLogic.Enums;
using FluentAssertions;
using Moq;
using Xunit;


namespace BuisnessLogic.Tests.BudgetSpecifications
{
    public class BudgetSpecificationsCreatorsProviderTests
    {
        [Fact]
        public void Get_WhenCreatorExists_ShouldReturnCreator()
        {
            // Arrange
            var creatorMock = new Mock<IBudgetSpecificationsCreator>();

            creatorMock
                .SetupGet(x => x.Type)
                .Returns(BudgetFilterType.Account);

            var provider = new BudgetSpecificationsCreatorsProvider(
                [creatorMock.Object]);

            // Act
            var result = provider.Get(BudgetFilterType.Account);

            // Assert
            result.Should().BeSameAs(creatorMock.Object);
        }

        [Fact]
        public void Get_WhenCreatorDoesNotExist_ShouldThrowArgumentException()
        {
            // Arrange
            var provider = new BudgetSpecificationsCreatorsProvider([]);

            // Act
            Action act = () => provider.Get(BudgetFilterType.Account);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("No Creator for filter type:*");
        }
    }
}
