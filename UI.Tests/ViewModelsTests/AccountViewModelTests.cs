using CommunityToolkit.Mvvm.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
