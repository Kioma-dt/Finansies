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
    public class AccountNode
    {
        public Account Account { get; set; } = null!;
        public int Level { get; set; }
    }

    public partial class TransactionsViewModel : ObservableObject
    {
        private readonly IAccountService _accountService;
        private readonly ITransactionService _transactionService;
        private readonly AccountCreatePopUp _accountCreatePopUp;

        private List<Account> _accounts = new();

        [ObservableProperty]
        public partial Guid UserId { get; set; }

        public ObservableCollection<AccountNode> FlatAccounts { get;} = new ObservableCollection<AccountNode>();
        public ObservableCollection<Transaction> Transactions { get; } = new ObservableCollection<Transaction>();

        public TransactionsViewModel(
            IAccountService accountService,
            ITransactionService transactionService,
            IUserContext user,
            AccountCreatePopUp accountCreatePopUp)
        {
            _accountService = accountService;
            _transactionService = transactionService;
            UserId = user.UserId;
            _accountCreatePopUp = accountCreatePopUp;
        }

        [RelayCommand]
        public async Task Load()
        {

            _accounts = await _accountService.GetAll(UserId);

            BuildFlatTree(_accounts);

            var transactions = await _transactionService.GetAll(UserId);
            Transactions.Clear();
            foreach (var tran in transactions)
                Transactions.Add(tran);

        }

        [RelayCommand]
        public async Task AddAccount()
        {
            var result = await Application.Current.MainPage.ShowPopupAsync<Account?>(_accountCreatePopUp);

            var acc = result.Result;

            if (acc is not null)
            {
                acc.UserId = UserId;

                await _accountService.Add(acc);

                _accounts.Add(acc);

                this.BuildFlatTree(_accounts);
            }
        }

        private void BuildFlatTree(List<Account> accounts)
        {
            FlatAccounts.Clear();

            void AddChildren(Account acc, int level)
            {
                FlatAccounts.Add(new AccountNode
                {
                    Account = acc,
                    Level = level
                });

                foreach (var child in acc.Children)
                {
                    AddChildren(child, level + 1);
                }
            }

            var roots = accounts.Where(x => x.ParentId == null);

            foreach (var root in roots)
            {
                AddChildren(root, 0);
            }
        }
    }
}
