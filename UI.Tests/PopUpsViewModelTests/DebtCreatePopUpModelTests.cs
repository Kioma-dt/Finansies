using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.UseCases.CategoryUseCasses.Queries;
using BuisnessLogic.UseCases.DebtsUseCasses.Commands;
using BuisnessLogic.UseCases.FamilyMembersUseCasses.Queries;
using UI.PopUps.ViewModels;

namespace UI.Tests.PopUpsViewModelTests
{
    using FluentAssertions;
    using MediatR;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;
    public class DebtCreatePopUpModelTests
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<IUserContext> _userContext = new();

        private DebtCreatePopUpModel CreateVm()
        {
            _userContext.Setup(x => x.UserId).Returns(Guid.NewGuid());
            return new DebtCreatePopUpModel(_mediator.Object, _userContext.Object);
        }

        [Fact]
        public async Task Initialize_ShouldResetFieldsAndLoadData()
        {
            var userId = Guid.NewGuid();

            _userContext.Setup(x => x.UserId).Returns(userId);

            _mediator
                .Setup(x => x.Send(It.IsAny<GetAllCategoriesQuery>(), default))
                .ReturnsAsync(new List<Category>
                {
                new() { Id = Guid.NewGuid(), Name = "Cat1" }
                });

            _mediator
                .Setup(x => x.Send(It.IsAny<GetAllFamilyMembersQuery>(), default))
                .ReturnsAsync(new List<FamilyMember>
                {
                new() { Id = Guid.NewGuid(), Name = "Fam1" }
                });

            var vm = CreateVm();

            await vm.Initialize();

            vm.Name.Should().BeEmpty();
            vm.Amount.Should().BeEmpty();

            vm.Categories.Should().Contain(x => x.Name == "-No Category-");
            vm.FamilyMembers.Should().Contain(x => x.Name == "-No Family Member-");

            vm.SelectedDebtType.Should().Be(DebtType.Credit);
            vm.SelectedInterestType.Should().Be(InterestType.None);
            vm.IsInterestValueEnabled.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateInterestField_ShouldDisable_WhenNone()
        {
            var vm = CreateVm();

            vm.SelectedInterestType = InterestType.None;

            await vm.UpdateInterestField();

            vm.IsInterestValueEnabled.Should().BeFalse();
            vm.InterestValuePlaceholder.Should().Be("No Interest");
        }

        [Fact]
        public async Task UpdateInterestField_ShouldEnable_WhenSimpleOrComplex()
        {
            var vm = CreateVm();

            vm.SelectedInterestType = InterestType.Simple;

            await vm.UpdateInterestField();

            vm.IsInterestValueEnabled.Should().BeTrue();
            vm.InterestValuePlaceholder.Should().Be("Interest %");
        }

        [Fact]
        public async Task UpdateInterestField_ShouldEnableFixed()
        {
            var vm = CreateVm();

            vm.SelectedInterestType = InterestType.Fixed;

            await vm.UpdateInterestField();

            vm.IsInterestValueEnabled.Should().BeTrue();
            vm.InterestValuePlaceholder.Should().Be("Fixed amount");
        }

        [Fact]
        public async Task Create_ShouldReturnCommand_WhenValid()
        {
            var vm = CreateVm();

            CreateDebtCommand? result = null;

            vm.CloseAction = cmd => result = cmd;

            vm.Name = "Debt";
            vm.Amount = "100";
            vm.InterestRate = "10";

            vm.StartDate = DateTime.Now;
            vm.EndDate = DateTime.Now.AddDays(10);

            vm.SelectedCategory = new Category { Id = Guid.Empty };
            vm.SelectedFamilyMember = new FamilyMember { Id = Guid.Empty };

            await vm.Create();

            result.Should().NotBeNull();
            result!.Name.Should().Be("Debt");
        }

        [Fact]
        public async Task Create_ShouldFail_WhenNameEmpty()
        {
            var vm = CreateVm();

            bool called = false;

            vm.CloseAction = _ => called = true;

            vm.Name = "";
            vm.Amount = "100";
            vm.InterestRate = "10";

            await vm.Create();

            called.Should().BeFalse();
        }

        [Fact]
        public async Task Create_ShouldFail_WhenAmountInvalid()
        {
            var vm = CreateVm();

            bool called = false;

            vm.CloseAction = _ => called = true;

            vm.Name = "Debt";
            vm.Amount = "abc";
            vm.InterestRate = "10";

            await vm.Create();

            called.Should().BeFalse();
        }

        [Fact]
        public async Task Create_ShouldFail_WhenInterestInvalid()
        {
            var vm = CreateVm();

            bool called = false;

            vm.CloseAction = _ => called = true;

            vm.Name = "Debt";
            vm.Amount = "100";
            vm.InterestRate = "abc";

            await vm.Create();

            called.Should().BeFalse();
        }

        [Fact]
        public async Task Create_ShouldFail_WhenStartDateGreaterThanEndDate()
        {
            var vm = CreateVm();

            bool called = false;

            vm.CloseAction = _ => called = true;

            vm.Name = "Debt";
            vm.Amount = "100";
            vm.InterestRate = "10";

            vm.StartDate = DateTime.Now.AddDays(10);
            vm.EndDate = DateTime.Now;

            await vm.Create();

            called.Should().BeFalse();
        }
    }
}
