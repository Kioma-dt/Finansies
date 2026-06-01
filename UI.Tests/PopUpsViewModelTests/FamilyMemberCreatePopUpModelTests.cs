using UI.PopUps.ViewModels;

namespace UI.Tests.PopUpsViewModelTests
{
    using BuisnessLogic.UseCases.FamilyMembersUseCasses.Commands;
    using FluentAssertions;
    using MediatR;
    using Moq;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class FamilyMemberCreatePopUpModelTests
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<IUserContext> _userContext = new();

        private readonly Guid _userId = Guid.NewGuid();

        public FamilyMemberCreatePopUpModelTests()
        {
            _userContext.Setup(x => x.UserId).Returns(_userId);
        }

        private FamilyMemberCreatePopUpModel CreateVm()
            => new FamilyMemberCreatePopUpModel(_mediator.Object, _userContext.Object);

        [Fact]
        public async Task Initialize_ShouldSetEmptyName()
        {
            var vm = CreateVm();

            await vm.Initialize();

            vm.Name.Should().Be(string.Empty);
        }

        [Fact]
        public async Task Create_ShouldReturnCommand_WhenValidName()
        {
            CreateFamilyMemberCommand? result = null;

            var vm = CreateVm();
            vm.Name = "John";

            vm.CloseAction = cmd => result = cmd;

            await vm.CreateCommand.ExecuteAsync(null);

            result.Should().NotBeNull();
            result!.UserId.Should().Be(_userId);
            result.Name.Should().Be("John");
        }

        [Fact]
        public async Task Create_ShouldNotReturnCommand_WhenNameIsEmpty()
        {
            CreateFamilyMemberCommand? result = null;

            var vm = CreateVm();
            vm.Name = "";

            vm.CloseAction = cmd => result = cmd;

            await vm.CreateCommand.ExecuteAsync(null);

            result.Should().BeNull();
        }

        [Fact]
        public async Task Create_ShouldNotReturnCommand_WhenNameIsWhitespace()
        {
            CreateFamilyMemberCommand? result = null;

            var vm = CreateVm();
            vm.Name = "   ";

            vm.CloseAction = cmd => result = cmd;

            await vm.CreateCommand.ExecuteAsync(null);

            result.Should().BeNull();
        }

        [Fact]
        public async Task Cancel_ShouldReturnNull()
        {
            CreateFamilyMemberCommand? result = new(_userId, "test");

            var vm = CreateVm();

            vm.CloseAction = cmd => result = cmd;

            await vm.CancelCommand.ExecuteAsync(null);

            result.Should().BeNull();
        }
    }
}
