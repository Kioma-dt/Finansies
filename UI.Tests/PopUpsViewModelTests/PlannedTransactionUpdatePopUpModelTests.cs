using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.UseCases.CategoryUseCasses.Queries;
using BuisnessLogic.UseCases.FamilyMembersUseCasses.Queries;
using UI.PopUps.ViewModels;

namespace UI.Tests.PopUpsViewModelTests
{
    using BuisnessLogic.UseCases.AccountsUseCases.Queries;
    using BuisnessLogic.UseCases.DebtsUseCasses.Queries;
    using BuisnessLogic.UseCases.PlannedTransactionsUsesCasses.Commands;
    using FluentAssertions;
    using MediatR;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class PlannedTransactionUpdatePopUpModelTests
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<IUserContext> _userContext = new();

        private PlannedTransactionUpdatePopUpModel CreateVm()
        {
            _userContext.Setup(x => x.UserId).Returns(Guid.NewGuid());
            return new PlannedTransactionUpdatePopUpModel(_mediator.Object, _userContext.Object);
        }

        [Fact]
        public async Task Initialize_ShouldLoadAllDataAndSetSelectedValues()
        {
            var userId = Guid.NewGuid();
            _userContext.Setup(x => x.UserId).Returns(userId);

            var accountId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var familyId = Guid.NewGuid();
            var debtId = Guid.NewGuid();

            _mediator.Setup(x => x.Send(It.IsAny<GetAllAccountsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Account> { new() { Id = accountId, Name = "A" } });

            _mediator.Setup(x => x.Send(It.IsAny<GetAllCategoriesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Category> { new() { Id = categoryId, Name = "C" } });

            _mediator.Setup(x => x.Send(It.IsAny<GetAllFamilyMembersQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<FamilyMember> { new() { Id = familyId, Name = "F" } });

            _mediator.Setup(x => x.Send(It.IsAny<GetAllDebtsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Debt> { new() { Id = debtId, Name = "D", Type = DebtType.Debit } });

            var vm = CreateVm();

            await vm.Initialize(new PlannedTransactionUpdatePopUpModelParameters(
                Guid.NewGuid(),
                100,
                "desc",
                TransactionType.Income,
                DateTime.Now,
                accountId,
                categoryId,
                familyId,
                debtId
            ));

            vm.Amount.Should().Be("100");
            vm.Description.Should().Be("desc");

            vm.SelectedAccount.Should().NotBeNull();
            vm.SelectedCategory.Should().NotBeNull();
            vm.SelectedFamilyMember.Should().NotBeNull();
            vm.SelectedDebt.Should().NotBeNull();
        }

        

        [Fact]
        public async Task UpdateFilteredDebts_ShouldFilterExpenseAsCredit()
        {
            var vm = CreateVm();

            var debitId = Guid.NewGuid();
            var creditId = Guid.NewGuid();

            vm.GetType()
                .GetField("_debts", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .SetValue(vm, new List<Debt>
                {
                new() { Id = debitId, Type = DebtType.Debit },
                new() { Id = creditId, Type = DebtType.Credit }
                });

            vm.SelectedTransactionType = TransactionType.Expense;

            await vm.UpdateFilteredDebts();

            vm.FilteredDebts.Should().ContainSingle(x => x.Type == DebtType.Credit);
        }

        [Fact]
        public async Task Create_ShouldReturnCommand_WhenValid()
        {
            var vm = CreateVm();

            UpdatePlannedTransactionCommand? result = null;

            vm.CloseAction = cmd => result = cmd;

            vm.Description = "Test";
            vm.Amount = "150";
            vm.StartDate = DateTime.Now;

            vm.SelectedAccount = new Account { Id = Guid.NewGuid() };
            vm.SelectedCategory = new Category { Id = Guid.NewGuid() };
            vm.SelectedFamilyMember = new FamilyMember { Id = Guid.NewGuid() };
            vm.SelectedDebt = new Debt { Id = Guid.NewGuid() };

            await vm.Create();

            result.Should().NotBeNull();
            result!.Description.Should().Be("Test");
        }

        [Fact]
        public async Task Create_ShouldFail_WhenDescriptionEmpty()
        {
            var vm = CreateVm();

            bool called = false;

            vm.CloseAction = _ => called = true;

            vm.Description = "";
            vm.Amount = "100";

            await vm.Create();

            called.Should().BeFalse();
        }

        [Fact]
        public async Task Create_ShouldFail_WhenAmountInvalid()
        {
            var vm = CreateVm();

            bool called = false;

            vm.CloseAction = _ => called = true;

            vm.Description = "Test";
            vm.Amount = "abc";

            await vm.Create();

            called.Should().BeFalse();
        }
    }
}
