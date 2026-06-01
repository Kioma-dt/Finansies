using BuisnessLogic.Entities;
using MediatR;
using UI.PopUps.ViewModels;
using BuisnessLogic.UseCases.CategoryUseCasses.Queries;
using BuisnessLogic.UseCases.CategoryUseCasses.Commands;

namespace UI.Tests.PopUpsViewModelTests
{
    public class CategoryCreatePopUpModelTests
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<IUserContext> _userContext = new();

        private CategoryCreatePopUpModel CreateVm()
        {
            _userContext.Setup(x => x.UserId).Returns(Guid.NewGuid());
            return new CategoryCreatePopUpModel(_mediator.Object, _userContext.Object);
        }

        [Fact]
        public async Task Initialize_ShouldLoadParentsAndSetDefaults()
        {
            var userId = Guid.NewGuid();

            _userContext.Setup(x => x.UserId).Returns(userId);

            var categories = new List<Category>
        {
            new() { Id = Guid.NewGuid(), Name = "A" },
            new() { Id = Guid.NewGuid(), Name = "B" }
        };

            _mediator
                .Setup(x => x.Send(It.IsAny<GetAllCategoriesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(categories);

            var vm = CreateVm();

            await vm.Initialize();

            vm.Parents.Should().HaveCount(3); // + "-No Parent-"
            vm.Parents.First().Name.Should().Be("-No Parent-");

            vm.SelectedParent.Should().NotBeNull();
            vm.Name.Should().BeEmpty();
            vm.Description.Should().BeEmpty();
        }

        [Fact]
        public async Task Create_ShouldReturnCommand_WhenValidInput()
        {
            var vm = CreateVm();

            var resultCommand = default(CreateCategoryCommand);

            vm.CloseAction = cmd => resultCommand = cmd;

            vm.Name = "Test";
            vm.Description = "Desc";

            vm.SelectedParent = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Parent"
            };

            await vm.Create();

            resultCommand.Should().NotBeNull();
            resultCommand!.Name.Should().Be("Test");
            resultCommand.Description.Should().Be("Desc");
        }

        [Fact]
        public async Task Create_ShouldReturnNull_WhenCancelledParent()
        {
            var vm = CreateVm();

            CreateCategoryCommand? result = null;

            vm.CloseAction = cmd => result = cmd;

            await vm.Create();

            result.Should().BeNull();
        }

        [Fact]
        public async Task Create_ShouldSetNullParent_WhenNoParentSelected()
        {
            var vm = CreateVm();

            CreateCategoryCommand? result = null;

            vm.CloseAction = cmd => result = cmd;

            vm.Name = "Test";
            vm.Description = "Desc";

            vm.SelectedParent = new Category
            {
                Id = Guid.Empty,
                Name = "-No Parent-"
            };

            await vm.Create();

            result.Should().NotBeNull();
            result!.ParentId.Should().BeNull();
        }

        [Fact]
        public async Task Create_ShouldThrowAndNotCallClose_WhenNameEmpty()
        {
            var vm = CreateVm();

            bool called = false;

            vm.CloseAction = _ => called = true;

            vm.Name = "";
            vm.Description = "Desc";

            await vm.Create();

            called.Should().BeFalse();
        }

        [Fact]
        public async Task Create_ShouldThrowAndNotCallClose_WhenDescriptionEmpty()
        {
            var vm = CreateVm();

            bool called = false;

            vm.CloseAction = _ => called = true;

            vm.Name = "Test";
            vm.Description = "";

            await vm.Create();

            called.Should().BeFalse();
        }
    }
}
