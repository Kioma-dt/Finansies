using BuisnessLogic.Entities;
using BuisnessLogic.UseCases.TransfersUseCasses.Commands;
using BuisnessLogic.UseCases.TransfersUseCasses.Queries;
using MediatR;
using UI.Messages;
using UI.Popups;
using UI.PopUps.Service;
using UI.ViewModels;

namespace UI.Tests.ViewModelsTests
{
    public class TransfersViewModelTests
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<IUserContext> _userContext = new();
        private readonly Mock<IPopUpService> _popup = new();

        private readonly Guid _userId = Guid.NewGuid();

        public TransfersViewModelTests()
        {
            _userContext
                .Setup(x => x.UserId)
                .Returns(_userId);
        }

        private TransfersViewModel CreateVm()
        {
            return new TransfersViewModel(
                _mediator.Object,
                _userContext.Object,
                _popup.Object);
        }

        [Fact]
        public async Task Receive_ShouldLoadTransfers()
        {
            var transfer = new Transfer
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                Description = "T1",
                Amount = 100,
                Date = DateTime.Today
            };

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllTransfersQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { transfer });

            var vm = CreateVm();

            vm.Receive(new DataBaseChangedMessage(DataBaseChangedMessageType.Transfers));

            await Task.Delay(50);

            vm.DisplayedTransfers.Should().ContainSingle();
            vm.DisplayedTransfers[0].Desciption.Should().Be("T1");
        }

        [Fact]
        public async Task Receive_ShouldFilterByDateRange()
        {
            var oldTransfer = new Transfer
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                Amount = 100,
                Date = DateTime.Today.AddMonths(-2)
            };

            var newTransfer = new Transfer
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                Amount = 200,
                Date = DateTime.Today
            };

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllTransfersQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { oldTransfer, newTransfer });

            var vm = CreateVm();

            vm.Receive(new DataBaseChangedMessage(DataBaseChangedMessageType.Transfers));

            await Task.Delay(50);

            vm.Receive(new DateRangeChangedMessage(
                DateTime.Today.AddDays(-1),
                DateTime.Today.AddDays(1)));

            vm.DisplayedTransfers.Should().ContainSingle();
        }

        [Fact]
        public async Task Receive_ShouldFilterBySelectedAccount()
        {
            var acc1 = Guid.NewGuid();
            var acc2 = Guid.NewGuid();

            var transfer1 = new Transfer
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                FromAccountId = acc1,
                ToAccountId = Guid.NewGuid(),
                Date = DateTime.Today
            };

            var transfer2 = new Transfer
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                FromAccountId = acc2,
                ToAccountId = Guid.NewGuid(),
                Date = DateTime.Today
            };

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllTransfersQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { transfer1, transfer2 });

            var vm = CreateVm();

            vm.Receive(new DataBaseChangedMessage(DataBaseChangedMessageType.Transfers));

            await Task.Delay(50);

            vm.Receive(new SelectedAccountChangedMessage(acc1));

            vm.DisplayedTransfers.Should().ContainSingle();
            vm.DisplayedTransfers[0].Id.Should().Be(transfer1.Id);
        }

        [Fact]
        public async Task TransferMoney_ShouldSendCommand_WhenPopupReturnsValue()
        {
            var command = new CreateTransferCommand(
                _userId,
                100,
                "Test",
                DateTime.Today,
                Guid.NewGuid(),
                Guid.NewGuid());

            _popup
                .Setup(x => x.ShowPopUp<CreateTransferCommand?, TransferCreatePopUp>())
                .ReturnsAsync(command);

            var vm = CreateVm();

            await vm.TransferMoney();

            _mediator.Verify(
                x => x.Send(command, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task TransferMoney_ShouldNotSend_WhenPopupReturnsNull()
        {
            _popup
                .Setup(x => x.ShowPopUp<CreateTransferCommand?, TransferCreatePopUp>())
                .ReturnsAsync((CreateTransferCommand?)null);

            var vm = CreateVm();

            await vm.TransferMoney();

            _mediator.Verify(
                x => x.Send(It.IsAny<CreateTransferCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public void Load_ShouldNotThrow()
        {
            var vm = CreateVm();

            var act = () => vm.Load();

            act.Should().NotThrow();
        }
    }
}
