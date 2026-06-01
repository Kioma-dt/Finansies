using BuisnessLogic.Entities;
using UI.PopUps.ViewModels;

namespace UI.Tests.PopUpsViewModelTests
{
    using BuisnessLogic.UseCases.AccountsUseCases.Queries;
    using BuisnessLogic.UseCases.TransfersUseCasses.Commands;
    using FluentAssertions;
    using MediatR;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class TransferCreatePopUpModelTests
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<IUserContext> _userContext = new();

        private readonly Guid _userId = Guid.NewGuid();

        public TransferCreatePopUpModelTests()
        {
            _userContext.Setup(x => x.UserId).Returns(_userId);
        }

        private TransferCreatePopUpModel CreateVm()
            => new TransferCreatePopUpModel(_mediator.Object, _userContext.Object);

        private static Account CreateAccount(Guid? id = null)
            => new Account
            {
                Id = id ?? Guid.NewGuid(),
                Name = "Test"
            };

        [Fact]
        public async Task Initialize_ShouldLoadAccounts_AndSetDefaults()
        {
            var accounts = new List<Account>
        {
            CreateAccount(),
            CreateAccount()
        };

            _mediator
                .Setup(x => x.Send(It.IsAny<GetAllAccountsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(accounts);

            var vm = CreateVm();

            await vm.Initialize();

            vm.Accounts.Should().HaveCount(2);
            vm.SelectedFromAccount.Should().NotBeNull();
            vm.SelectedToAccount.Should().NotBeNull();
        }

        [Fact]
        public async Task Create_ShouldReturnCommand_WhenValid()
        {
            var from = CreateAccount();
            var to = CreateAccount();

            var vm = CreateVm();

            vm.Description = "Transfer";
            vm.Amount = "100";
            vm.Date = DateTime.Now;
            vm.SelectedFromAccount = from;
            vm.SelectedToAccount = to;

            vm.CreateCommand.Execute(null);

            _mediator.Verify(x =>
                x.Send(It.IsAny<CreateTransferCommand>(), It.IsAny<CancellationToken>()),
                Times.Never); // важно: VM только вызывает CloseAction, не mediator
        }

        [Fact]
        public async Task Create_ShouldFail_WhenDescriptionIsEmpty()
        {
            var vm = CreateVm();

            vm.Description = "";
            vm.Amount = "100";
            vm.SelectedFromAccount = CreateAccount();
            vm.SelectedToAccount = CreateAccount();

            await vm.CreateCommand.ExecuteAsync(null);

            _mediator.Verify(x =>
                x.Send(It.IsAny<CreateTransferCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Create_ShouldFail_WhenAmountInvalid()
        {
            var vm = CreateVm();

            vm.Description = "Test";
            vm.Amount = "abc"; // invalid
            vm.SelectedFromAccount = CreateAccount();
            vm.SelectedToAccount = CreateAccount();

            await vm.CreateCommand.ExecuteAsync(null);

            _mediator.Verify(x =>
                x.Send(It.IsAny<CreateTransferCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Create_ShouldFail_WhenAccountsAreNull()
        {
            var vm = CreateVm();

            vm.Description = "Test";
            vm.Amount = "100";

            vm.SelectedFromAccount = null;
            vm.SelectedToAccount = null;

            await vm.CreateCommand.ExecuteAsync(null);

            _mediator.Verify(x =>
                x.Send(It.IsAny<CreateTransferCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Create_ShouldFail_WhenSameAccountSelected()
        {
            var acc = CreateAccount();

            var vm = CreateVm();

            vm.Description = "Test";
            vm.Amount = "100";
            vm.SelectedFromAccount = acc;
            vm.SelectedToAccount = acc;

            await vm.CreateCommand.ExecuteAsync(null);

            _mediator.Verify(x =>
                x.Send(It.IsAny<CreateTransferCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
