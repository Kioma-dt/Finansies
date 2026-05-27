using BuisnessLogic.Entities;
using BuisnessLogic.UseCases.AccountsUseCases.Commands;
using BuisnessLogic.UseCases.AccountsUseCases.Queries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.PopUps.ViewModels
{
    public partial class AccountCreatePopUpModel
        : ObservableObject
    {
        readonly IMediator _mediator;
        readonly IUserContext _userContext;

        public Action<CreateAccountCommand?>? CloseAction {  get; set; }

        public AccountCreatePopUpModel(IMediator mediator,
            IUserContext userContext)
        {
            _mediator = mediator;
            _userContext = userContext;
        }

        [ObservableProperty]
        public partial string? Name {  get; set; }

        [ObservableProperty]
        public partial string? Balance {  get; set; }

        [ObservableProperty]
        public partial Account? SelectedParent {  get; set; }

        public ObservableCollection<Account> Parents { get; set; } = new();

        public async Task Initialize()
        {
            Parents.Clear();

            Parents.Insert(0, new Account() { Id = Guid.Empty, Name = "-No Parent-" });

            var parents = await _mediator.Send(new GetAllAccountsQuery(_userContext.UserId));

            foreach (var parent in parents) 
            {
                Parents.Add(parent);
            }

            SelectedParent = Parents.FirstOrDefault();

            Name = string.Empty;
            Balance = string.Empty;
        }

        [RelayCommand]
        public async Task Cancel()
        {
            CloseAction?.Invoke(null);
        }

        [RelayCommand]
        public async Task Create()
        {
            try
            {
                var name = Name;

                if (String.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException($"Enter Name!");
                }

                if (!decimal.TryParse(Balance, out var balance) || balance <= 0)
                {
                    throw new ArgumentException("Wrong Balnce Format! (Decimal > 0)");
                }

                var parent = SelectedParent;


                CloseAction?.Invoke(new CreateAccountCommand(
                    _userContext.UserId,
                    name,
                    balance,
                    parent == null || parent.Id == Guid.Empty ? null : parent.Id));
            }
            catch (ArgumentException ex)
            {
                await Shell.Current.DisplayAlert(
                       "Can't Create Account",
                       ex.Message,
                       "OK");
            }
            
        }
    }
}
