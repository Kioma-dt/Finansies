using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Services;
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

namespace UI.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly IUserContext _user;
        private readonly ITransactionService _transactionService;
        private readonly TransactionCreatePopUp _transctionCreatePopUp;

        [ObservableProperty]
        public partial View? LeftView { get; set; }

        [ObservableProperty]
        public partial View? RightView { get; set; }

        [ObservableProperty]
        public partial View? CentralView { get; set; }


        private readonly TransactionView _transactionView;
        private readonly PlannedTransactionView _plannedTransactionView;
        private readonly AccountView _accountView;
        private readonly CategoryView _categoryView;
        private readonly BudgetView _budgetView;
        private readonly DebtView _debtView;

        public MainPageViewModel( IUserContext user,
            ITransactionService transactionService,
            TransactionCreatePopUp transactionCreatePopUp,
            AccountView accountView, 
            TransactionView transactionView,
            PlannedTransactionView plannedTransactionView,
            CategoryView categoryView,
            BudgetView budgetView,
            DebtView debtView)
        {
            _user = user;
            _transactionService = transactionService;
            _transctionCreatePopUp = transactionCreatePopUp;

            _accountView = accountView;
            _transactionView = transactionView;
            _plannedTransactionView = plannedTransactionView;
            _categoryView = categoryView;
            _budgetView = budgetView;
            _debtView = debtView;

            LeftView = _accountView;
            RightView = _transactionView;
            CentralView = null;
        }

        [RelayCommand]
        public void ShowTransactions()
        {
            LeftView = _accountView;
            RightView = _transactionView;
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
                var result = await Application.Current.MainPage
                .ShowPopupAsync<Transaction?>(_transctionCreatePopUp);

                var transaction = result.Result;

                if (transaction is null)
                    return;

                await _transactionService.RegsiterTransaction(_user.UserId,
                    transaction.Amount,
                    transaction.Description,
                    transaction.Date,
                    transaction.Type,
                    transaction.AccountId,
                    transaction.CategoryId,
                    transaction.FamilyMemberId,
                    transaction.DebtId);

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
