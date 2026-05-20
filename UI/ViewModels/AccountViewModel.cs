using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
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

using CommunityToolkit.Mvvm.Messaging;

using UI.Popups;
using UI.Messages;
using BuisnessLogic.UseCases.AccountsUseCases.Queries;
using BuisnessLogic.UseCases.AccountsUseCases.Commands;

namespace UI.ViewModels
{
    public class AccountNode
    {
        public Account Account { get; set; } = null!;
        public int Level { get; set; }
    }

    public partial class AccountViewModel : ObservableObject, IRecipient<DataBaseChangedMessage>
    {
        private readonly IMediator _mediator;
        private readonly IUserContext _user;
        private readonly AccountCreatePopUp _popup;

        private List<Account> _accounts = new();

        [ObservableProperty]
        public partial bool IsLoaded { get; set; } = false;

        public ObservableCollection<AccountNode> FlatAccounts { get; } = new();

        public AccountViewModel(
            IMediator mediator,
            IUserContext user,
            AccountCreatePopUp popup)
        {

            _mediator = mediator;
            _user = user;
            _popup = popup;

            WeakReferenceMessenger.Default.Register<DataBaseChangedMessage>(this);
        }

        public async void Receive(DataBaseChangedMessage message)
        {
            if (message.Type == DataBaseChangedMessageType.Init || message.Type == DataBaseChangedMessageType.Accounts)
            {
                _accounts = (await _mediator.Send(new GetAllAccountsQuery(_user.UserId))).ToList();

                BuildTree();
            }
        }

        //[RelayCommand]
        //public async Task Load()
        //{
        //    _accounts = await _accountService.GetAll(_user.UserId);

        //    BuildTree();
        //}

        [RelayCommand]
        public async Task AddAccount()
        {
            var result = await Application.Current.MainPage
                .ShowPopupAsync<CreateAccountCommand?>(_popup);

            var command = result.Result;

            if (command is null)
                return;

            await _mediator.Send(command);

            WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Accounts));
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
