using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using BuisnessLogic.UseCases.DebtsUseCasses.Commands;
using FluentAssertions;
using Moq;
using Xunit;

namespace BuisnessLogic.Tests.UseCasess
{
    public class UpdateDebtCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldThrow_WhenDebtNotFound()
        {
            var repo = new Mock<IDebtRepository>();

            repo.Setup(x => x.GetById(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync((Debt?)null);

            var handler = new UpdateDebtCommandHandler(repo.Object);

            var cmd = new UpdateDebtCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "name",
                null,
                null
            );

            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(cmd, default));
        }

        [Fact]
        public async Task Handle_ShouldUpdateDebtFields()
        {
            var debt = new Debt();

            var repo = new Mock<IDebtRepository>();

            repo.Setup(x => x.GetById(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(debt);

            repo.Setup(x => x.Update(debt))
                .Returns(Task.CompletedTask);

            var handler = new UpdateDebtCommandHandler(repo.Object);

            var cmd = new UpdateDebtCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "updated",
                Guid.NewGuid(),
                Guid.NewGuid()
            );

            await handler.Handle(cmd, default);

            debt.Name.Should().Be("updated");

            repo.Verify(x => x.Update(debt), Times.Once);
        }
    }
}
