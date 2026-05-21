using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.UseCases.AccountsUseCases.Commands;
using BuisnessLogic.UseCases.AccountsUseCases.Queries;
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
using UI.PopUps.Service;

namespace UI.ViewModels
{
    public class DisplayedAccount(Guid Id,
        string? Name,
        string? Balance,
        int Level)
    {
        public Guid Id { get; } = Id;
        public string? Name { get; } = Name;
        public string? Balance { get; } = Balance;
        public int Level { get; } = Level;
    }

    public partial class AccountViewModel : ObservableObject, IRecipient<DataBaseChangedMessage>
    {
        private readonly IMediator _mediator;
        private readonly IUserContext _user;
        private readonly IPopUpService _popupService;

        private List<Account> _accounts = new();

        [ObservableProperty]
        public partial bool IsLoaded { get; set; } = false;

        [ObservableProperty]
        public partial bool IsSelectedAccount { get; set; } = false;

        public ObservableCollection<DisplayedAccount> DisplayedAccounts { get; } = new();

        public AccountViewModel(
            IMediator mediator,
            IUserContext user,
            IPopUpService popup)
        {

            _mediator = mediator;
            _user = user;
            _popupService = popup;

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

        [RelayCommand]
        public async Task ChangeSelectedAccount(DisplayedAccount account)
        {
            IsSelectedAccount = true;
            WeakReferenceMessenger.Default.Send(new SelectedAccountChangedMessage(account.Id));
        }

        [RelayCommand]
        public async Task UnSelectAccount()
        {
            IsSelectedAccount = false;
            WeakReferenceMessenger.Default.Send(new SelectedAccountChangedMessage(null));
        }

        [RelayCommand]
        public async Task AddAccount()
        {
            var command = await _popupService.ShowPopUp<CreateAccountCommand?, AccountCreatePopUp>();

            if (command is null)
                return;

            await _mediator.Send(command);

            WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Accounts));
        }

        private void BuildTree()
        {
            DisplayedAccounts.Clear();

            void Add(Account acc, int level)
            {
                DisplayedAccounts.Add(new DisplayedAccount
                    (
                        acc.Id,
                        acc.Name,
                        acc.Balance.ToString(),
                        level
                    ));

                foreach (var child in acc.Children)
                    Add(child, level + 1);
            }

            foreach (var root in _accounts.Where(x => x.ParentId == null))
                Add(root, 0);
        }
    }
}
