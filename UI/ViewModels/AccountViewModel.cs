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

    public partial class AccountViewModel : ObservableObject
    {
        private readonly IAccountService _accountService;
        private readonly IUserContext _user;
        private readonly AccountCreatePopUp _popup;

        private List<Account> _accounts = new();

        [ObservableProperty]
        public partial bool IsLoaded { get; set; } = false;

        public ObservableCollection<AccountNode> FlatAccounts { get; } = new();

        public AccountViewModel(
            IAccountService accountService,
            IUserContext user,
            AccountCreatePopUp popup)
        {

            _accountService = accountService;
            _user = user;
            _popup = popup;
        }

        [RelayCommand]
        public async Task Load()
        {
            _accounts = await _accountService.GetAll(_user.UserId);

            BuildTree();
        }

        [RelayCommand]
        public async Task AddAccount()
        {
            var result = await Application.Current.MainPage
                .ShowPopupAsync<Account?>(_popup);

            var acc = result.Result;

            if (acc is null)
                return;

            acc.UserId = _user.UserId;

            await _accountService.Add(acc);

            _accounts.Add(acc);

            BuildTree();
        }

        private void BuildTree()
        {
            FlatAccounts.Clear();

            void Add(Account acc, int level)
            {
                FlatAccounts.Add(new AccountNode
                {
                    Account = acc,
                    Level = level
                });

                foreach (var child in acc.Children)
                    Add(child, level + 1);
            }

            foreach (var root in _accounts.Where(x => x.ParentId == null))
                Add(root, 0);
        }
    }
}
