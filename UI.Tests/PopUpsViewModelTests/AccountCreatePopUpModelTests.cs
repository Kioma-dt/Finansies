using BuisnessLogic.Entities;
using BuisnessLogic.UseCases.AccountsUseCases.Commands;
using BuisnessLogic.UseCases.AccountsUseCases.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.PopUps.ViewModels;

namespace UI.Tests.PopUpsViewModelTests
{
    public class AccountCreatePopUpModelTests
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<IUserContext> _userContext = new();

        private readonly Guid _userId = Guid.NewGuid();

        public AccountCreatePopUpModelTests()
        {
            _userContext
                .Setup(x => x.UserId)
                .Returns(_userId);
        }

        private AccountCreatePopUpModel CreateVm()
        {
            return new AccountCreatePopUpModel(
                _mediator.Object,
                _userContext.Object);
        }

        [Fact]
        public async Task Initialize_ShouldLoadParents()
        {
            var parents = new[]
            {
            new Account { Id = Guid.NewGuid(), Name = "A" },
            new Account { Id = Guid.NewGuid(), Name = "B" }
        };

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllAccountsQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(parents);

            var vm = CreateVm();

            await vm.Initialize();

            vm.Parents.Should().HaveCount(3); // + "-No Parent-"
            vm.SelectedParent.Should().NotBeNull();
            vm.Name.Should().BeEmpty();
            vm.Balance.Should().BeEmpty();
        }

        [Fact]
        public async Task Cancel_ShouldInvokeCloseAction_WithNull()
        {
            var vm = CreateVm();

            CreateAccountCommand? result = new CreateAccountCommand(
                _userId,
                "Test",
                100,
                null);

            CreateAccountCommand? received = result;

            vm.CloseAction = cmd =>
            {
                received = cmd;
            };

            await vm.Cancel();

            received.Should().BeNull();
        }

        [Fact]
        public async Task Create_ShouldReturnCommand_WhenValidInput()
        {
            var vm = CreateVm();

            vm.Name = "Test";
            vm.Balance = "100";

            CreateAccountCommand? result = null;

            vm.CloseAction = cmd => result = cmd;

            await vm.Create();

            result.Should().NotBeNull();
            result!.Name.Should().Be("Test");
            result.Balance.Should().Be(100);
        }

        [Fact]
        public async Task Create_ShouldUseNullParent_WhenNoParentSelected()
        {
            var vm = CreateVm();

            vm.Name = "Test";
            vm.Balance = "100";
            vm.SelectedParent = new Account { Id = Guid.Empty };

            CreateAccountCommand? result = null;

            vm.CloseAction = cmd => result = cmd;

            await vm.Create();

            result!.ParentId.Should().BeNull();
        }

       
    }
}
