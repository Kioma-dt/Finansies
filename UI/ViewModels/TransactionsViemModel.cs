using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Services;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using UI.Popups;

namespace UI.ViewModels
{
    public partial class TransactionsViewModel : ObservableObject
    {
        private readonly IAccountService _accountService;
        private readonly ITransactionService _transactionService;

        [ObservableProperty]
        public partial Guid UserId { get; set; }

        public ObservableCollection<Account> Accounts { get;} = new ObservableCollection<Account>();
        public ObservableCollection<Transaction> Transactions { get; } = new ObservableCollection<Transaction>();

        public TransactionsViewModel(
            IAccountService accountService,
            ITransactionService transactionService)
        {
            _accountService = accountService;
            _transactionService = transactionService;
        }

        [RelayCommand]
        public async Task Load()
        {
            UserId = new Guid("00000000-0000-0000-0000-000000000001");

            var accounts = await _accountService.GetAll(UserId);

            Accounts.Clear();
            foreach (var acc in accounts)
                Accounts.Add(acc);


            var transactions = await _transactionService.GetAll(UserId);
            Transactions.Clear();
            foreach (var tran in transactions)
                Transactions.Add(tran);

        }

        [RelayCommand]
        public async Task AddAccount()
        {
            var popup = new AddAccountPopup();

            var result = await Application.Current.MainPage.ShowPopupAsync<Account?>(popup);

            var acc = result.Result;

            if (acc is not null)
            {
                acc.UserId = UserId;

                await _accountService.Add(acc);

                Accounts.Add(acc);
            }
        }
    }
}
