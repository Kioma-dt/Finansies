using BuisnessLogic.Entities;
using BuisnessLogic.UseCases.FamilyMembersUseCasses.Commands;
using BuisnessLogic.UseCases.FamilyMembersUseCasses.Queries;
using MediatR;
using UI.Messages;
using UI.Popups;
using UI.PopUps.Service;
using UI.ViewModels;

namespace UI.Tests.ViewModelsTests
{
    public class FamilyMemberViewModelTests
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<IUserContext> _userContext = new();
        private readonly Mock<IPopUpService> _popupService = new();

        private readonly Guid _userId = Guid.NewGuid();

        private FamilyMemberViewModel CreateViewModel()
        {
            _userContext.Setup(x => x.UserId)
                .Returns(_userId);

            return new FamilyMemberViewModel(
                _mediator.Object,
                _userContext.Object,
                _popupService.Object);
        }

        [Fact]
        public async Task Receive_ShouldLoadFamilyMembers()
        {
            var familyMembers = new List<FamilyMember>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                Name = "John"
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                Name = "Kate"
            }
        };

            _mediator.Setup(x => x.Send(
                    It.IsAny<GetAllFamilyMembersQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(familyMembers);

            var vm = CreateViewModel();

            vm.Receive(new DataBaseChangedMessage(
                DataBaseChangedMessageType.FamilyMembers));

            await Task.Delay(50);

            vm.DisplayedFamilyMembers.Should().HaveCount(2);
        }

        [Fact]
        public async Task Receive_ShouldPopulateDisplayedFamilyMembers()
        {
            var familyMember = new FamilyMember
            {
                Id = Guid.NewGuid(),
                UserId = _userId,
                Name = "John"
            };

            _mediator.Setup(x => x.Send(
                    It.IsAny<GetAllFamilyMembersQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { familyMember });

            var vm = CreateViewModel();

            vm.Receive(new DataBaseChangedMessage(
                DataBaseChangedMessageType.FamilyMembers));

            await Task.Delay(50);

            vm.DisplayedFamilyMembers.Should().ContainSingle();

            var displayed = vm.DisplayedFamilyMembers.Single();

            displayed.Id.Should().Be(familyMember.Id);
            displayed.Name.Should().Be("John");
        }


        [Fact]
        public async Task AddFamilyMember_ShouldDoNothing_WhenPopupReturnsNull()
        {
            _popupService.Setup(x =>
                    x.ShowPopUp<CreateFamilyMemberCommand?, FamilyMemberCreatePopUp>())
                .ReturnsAsync((CreateFamilyMemberCommand?)null);

            var vm = CreateViewModel();

            await vm.AddFamilyMember();

            _mediator.Verify(x =>
                    x.Send(It.IsAny<CreateFamilyMemberCommand>(),
                        It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task AddFamilyMember_ShouldSendCommand()
        {
            var command = new CreateFamilyMemberCommand(
                _userId,
                "John");

            _popupService.Setup(x =>
                    x.ShowPopUp<CreateFamilyMemberCommand?, FamilyMemberCreatePopUp>())
                .ReturnsAsync(command);

            var vm = CreateViewModel();

            await vm.AddFamilyMember();

            _mediator.Verify(x =>
                    x.Send(command,
                        It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public void Receive_DateRangeChanged_ShouldNotThrow()
        {
            var vm = CreateViewModel();

            var act = () => vm.Receive(
                new DateRangeChangedMessage(
                    DateTime.Now.AddDays(-7),
                    DateTime.Now));

            act.Should().NotThrow();
        }

        [Fact]
        public void Receive_SelectedAccountChanged_ShouldNotThrow()
        {
            var vm = CreateViewModel();

            var act = () => vm.Receive(
                new SelectedAccountChangedMessage(Guid.NewGuid()));

            act.Should().NotThrow();
        }

        [Fact]
        public void Load_ShouldNotThrow()
        {
            var vm = CreateViewModel();

            var act = () => vm.Load();

            act.Should().NotThrow();
        }
    }
}
