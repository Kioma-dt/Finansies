using BuisnessLogic.Entities;
using BuisnessLogic.UseCases.UserUseCasses.Commands;
using BuisnessLogic.UseCases.UserUseCasses.Queries;
using MediatR;
using UI.ViewModels;

namespace UI.Tests.ViewModelsTests
{
    public class AuthentificationPageViewModelTests
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<IUserContext> _userContext = new();

        private AuthentificationPageViewModel CreateViewModel()
        {
            return new AuthentificationPageViewModel(
                _mediator.Object,
                _userContext.Object);
        }

        [Fact]
        public async Task Login_ShouldNotCallMediator_WhenNameIsEmpty()
        {
            var vm = CreateViewModel();

            vm.Name = "";
            vm.Password = "123456";

            await vm.Login();

            _mediator.Verify(
                x => x.Send(
                    It.IsAny<LogInQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Login_ShouldNotCallMediator_WhenPasswordIsEmpty()
        {
            var vm = CreateViewModel();

            vm.Name = "Roman";
            vm.Password = "";

            await vm.Login();

            _mediator.Verify(
                x => x.Send(
                    It.IsAny<LogInQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Login_ShouldNotCallMediator_WhenPasswordTooShort()
        {
            var vm = CreateViewModel();

            vm.Name = "Roman";
            vm.Password = "123";

            await vm.Login();

            _mediator.Verify(
                x => x.Send(
                    It.IsAny<LogInQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Login_ShouldNotSetUserId_WhenUserNotFound()
        {
            _mediator
                .Setup(x => x.Send(
                    It.IsAny<LogInQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            var vm = CreateViewModel();

            vm.Name = "Roman";
            vm.Password = "123456";

            await vm.Login();

            _userContext.Verify(
                x => x.SetUserId(It.IsAny<Guid>()),
                Times.Never);
        }

        


        [Fact]
        public async Task Register_ShouldNotCallMediator_WhenNameIsEmpty()
        {
            var vm = CreateViewModel();

            vm.Name = "";
            vm.Password = "123456";

            await vm.Register();

            _mediator.Verify(
                x => x.Send(
                    It.IsAny<RegisterCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Register_ShouldNotCallMediator_WhenPasswordIsEmpty()
        {
            var vm = CreateViewModel();

            vm.Name = "Roman";
            vm.Password = "";

            await vm.Register();

            _mediator.Verify(
                x => x.Send(
                    It.IsAny<RegisterCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Register_ShouldNotCallMediator_WhenPasswordTooShort()
        {
            var vm = CreateViewModel();

            vm.Name = "Roman";
            vm.Password = "123";

            await vm.Register();

            _mediator.Verify(
                x => x.Send(
                    It.IsAny<RegisterCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Register_ShouldNotSetUserId_WhenRegistrationFailed()
        {
            _mediator
                .Setup(x => x.Send(
                    It.IsAny<RegisterCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            var vm = CreateViewModel();

            vm.Name = "Roman";
            vm.Password = "123456";

            await vm.Register();

            _userContext.Verify(
                x => x.SetUserId(It.IsAny<Guid>()),
                Times.Never);
        }

        [Fact]
        public void Clear_ShouldResetNameAndPassword()
        {
            var vm = CreateViewModel();

            vm.Name = "Roman";
            vm.Password = "123456";

            vm.Clear();

            vm.Name.Should().BeEmpty();
            vm.Password.Should().BeEmpty();
        }
    }
}
