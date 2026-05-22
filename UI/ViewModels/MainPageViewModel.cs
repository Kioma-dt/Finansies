using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UI.Messages;
using UI.Popups;
using UI.Views;

using BuisnessLogic.UseCases.TransactionsUseCasses.Commands;
using UI.PopUps.Service;

namespace UI.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly IUserContext _user;
        private readonly IMediator _mediator;
        private readonly IPopUpService _popUpService;

        [ObservableProperty]
        public partial View? LeftView { get; set; }

        [ObservableProperty]
        public partial View? RightView { get; set; }

        [ObservableProperty]
        public partial View? CentralView { get; set; }

        [ObservableProperty]
        public partial View? DateRangeSelectorView { get; set; }


        private readonly TransactionView _transactionView;
        private readonly PlannedTransactionView _plannedTransactionView;
        private readonly AccountView _accountView;
        private readonly CategoryView _categoryView;
        private readonly FamilyMemberView _familyMemberView;
        private readonly BudgetView _budgetView;
        private readonly DebtView _debtView;
        private readonly TransferView _transferView;

        public MainPageViewModel( IUserContext user,
            IMediator mediator,
            IPopUpService popUpService,
            AccountView accountView, 
            TransactionView transactionView,
            PlannedTransactionView plannedTransactionView,
            CategoryView categoryView,
            FamilyMemberView familyMemberView,
            BudgetView budgetView,
            DebtView debtView,
            TransferView transferView,
            DateRangeSelectorView dateRangeSelectorView)
        {
            _user = user;
            _mediator = mediator;
            _popUpService = popUpService;

            _accountView = accountView;
            _transactionView = transactionView;
            _plannedTransactionView = plannedTransactionView;
            _categoryView = categoryView;
            _familyMemberView = familyMemberView;
            _budgetView = budgetView;
            _debtView = debtView;
            _transferView = transferView;

            LeftView = _accountView;
            RightView = _transactionView;
            CentralView = null;
            DateRangeSelectorView = dateRangeSelectorView;
        }

        [RelayCommand]
        public void ShowTransactions()
        {
            LeftView = _accountView;
            RightView = _transactionView;
            CentralView = null;
        }

        [RelayCommand]
        public void ShowTransfers()
        {
            LeftView = _accountView;
            RightView = _transferView;
            CentralView = null;
        }

        [RelayCommand]
        public void ShowCategories()
        {
            LeftView = _accountView;
            RightView = _categoryView;
            CentralView = null;
        }

        [RelayCommand]
        public void ShowFamilyMembers()
        {
            LeftView = _accountView;
            RightView = _familyMemberView;

            CentralView = null;
        }


        [RelayCommand]
        public void ShowPlannedTransactions()
        {
            LeftView = _accountView;
            RightView = _plannedTransactionView;
            CentralView = null;
        }

        [RelayCommand]
        public void ShowBudgets()
        {
            LeftView = null;
            RightView = null;

            CentralView = _budgetView;
        }

        [RelayCommand]
        public void ShowDebts()
        {
            LeftView = null;
            RightView = null;

            CentralView = _debtView;
        }

        [RelayCommand]
        public async Task CreateTransaction()
        {
            try
            {
                var transaction = await _popUpService.ShowPopUp<Transaction?, TransactionCreatePopUp>();

                if (transaction is null)
                    return;

                await _mediator.Send(new CreateTransactionCommand(_user.UserId,
                    transaction.Amount,
                    transaction.Description,
                    transaction.Date,
                    transaction.Type,
                    transaction.AccountId,
                    transaction.CategoryId,
                    transaction.FamilyMemberId,
                    transaction.DebtId));

                WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Accounts));
                WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Transactions));
                WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Debts));
            }
            catch (ArgumentException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Can't create Transaction", $"{ex.Message}", "OK");
            }

        }

        [RelayCommand]
        public void Load()
        {
            WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Init));
            WeakReferenceMessenger.Default.Send(new CurrentTimeMessage(DateTime.Now));
        }
    }
}
