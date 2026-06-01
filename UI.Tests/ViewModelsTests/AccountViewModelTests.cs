using BuisnessLogic.Entities;
using BuisnessLogic.UseCases.AccountsUseCases.Commands;
using BuisnessLogic.UseCases.AccountsUseCases.Queries;
using CommunityToolkit.Mvvm.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Messages;
using UI.Popups;
using UI.PopUps.Service;
using UI.ViewModels;

namespace UI.Tests.ViewModelsTests
{
    public class AccountViewModelTests
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<IUserContext> _userContext = new();
        private readonly Mock<IPopUpService> _popupService = new();

        private readonly Guid _userId = Guid.NewGuid();

        public AccountViewModelTests()
        {
            _userContext.Setup(x => x.UserId)
                .Returns(_userId);

            WeakReferenceMessenger.Default.Reset();
        }

        private AccountViewModel CreateViewModel()
        {
            return new AccountViewModel(
                _mediator.Object,
                _userContext.Object,
                _popupService.Object);
        }

        [Fact]
        public async Task ChangeSelectedAccount_ShouldSelectAccount()
        {
            var vm = CreateViewModel();

            var account = new DisplayedAccount(
                Guid.NewGuid(),
                "Account",
                "100",
                0);

            vm.DisplayedAccounts.Add(account);

            await vm.ChangeSelectedAccount(account);

            vm.IsSelectedAccount.Should().BeTrue();
            account.IsSelected.Should().BeTrue();
        }

        [Fact]
        public async Task ChangeSelectedAccount_ShouldUnselectPreviousAccounts()
        {
            var vm = CreateViewModel();

            var first = new DisplayedAccount(
                Guid.NewGuid(),
                "A",
                "100",
                0);

            var second = new DisplayedAccount(
                Guid.NewGuid(),
                "B",
                "100",
                0);

            first.IsSelected = true;

            vm.DisplayedAccounts.Add(first);
            vm.DisplayedAccounts.Add(second);

            await vm.ChangeSelectedAccount(second);

            first.IsSelected.Should().BeFalse();
            second.IsSelected.Should().BeTrue();
        }

        [Fact]
        public async Task UnSelectAccount_ShouldClearSelection()
        {
            var vm = CreateViewModel();

            var account = new DisplayedAccount(
                Guid.NewGuid(),
                "Account",
                "100",
                0);

            account.IsSelected = true;

            vm.DisplayedAccounts.Add(account);

            vm.IsSelectedAccount = true;

            await vm.UnSelectAccount();

            vm.IsSelectedAccount.Should().BeFalse();
            account.IsSelected.Should().BeFalse();
        }

        [Fact]
        public async Task AddAccount_ShouldSendCommand_WhenPopupReturnsCommand()
        {
            var vm = CreateViewModel();

            var command = new CreateAccountCommand(
                _userId,
                "Test",
                100,
                null);

            _popupService
                .Setup(x => x.ShowPopUp<CreateAccountCommand?, AccountCreatePopUp>())
                .ReturnsAsync(command);

            await vm.AddAccount();

            _mediator.Verify(
                x => x.Send(command, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task AddAccount_ShouldNotSendCommand_WhenPopupReturnsNull()
        {
            var vm = CreateViewModel();

            _popupService
                .Setup(x => x.ShowPopUp<CreateAccountCommand?, AccountCreatePopUp>())
                .ReturnsAsync((CreateAccountCommand?)null);

            await vm.AddAccount();

            _mediator.Verify(
                x => x.Send(
                    It.IsAny<CreateAccountCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Receive_ShouldLoadAccounts()
        {
            var vm = CreateViewModel();

            var account = new Account
            {
                Id = Guid.NewGuid(),
                Name = "Main",
                UserId = _userId
            };

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllAccountsQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[]
                {
            account
                });

            vm.Receive(
                new DataBaseChangedMessage(
                    DataBaseChangedMessageType.Accounts));

            await Task.Delay(50);

            vm.DisplayedAccounts.Should().HaveCount(1);

            vm.DisplayedAccounts[0].Name.Should().Be("Main");
        }
    }
}
