using BuisnessLogic.Entities;
using BuisnessLogic.UseCases.AccountsUseCases.Commands;
using BuisnessLogic.UseCases.AccountsUseCases.Queries;
using BuisnessLogic.UseCases.TransfersUseCasses.Commands;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace UI.PopUps.ViewModels
{
    public partial class TransferCreatePopUpModel
      : ObservableObject
    {
        readonly IMediator _mediator;
        readonly IUserContext _userContext;

        public Action<CreateTransferCommand?>? CloseAction { get; set; }

        public TransferCreatePopUpModel(IMediator mediator,
            IUserContext userContext)
        {
            _mediator = mediator;
            _userContext = userContext;
        }

        [ObservableProperty]
        public partial string? Description { get; set; }

        [ObservableProperty]
        public partial string? Amount { get; set; }

        [ObservableProperty]
        public partial DateTime Date {  get; set; }


        [ObservableProperty]
        public partial Account? SelectedFromAccount { get; set; }

        [ObservableProperty]
        public partial Account? SelectedToAccount { get; set; }

        public ObservableCollection<Account> Accounts { get; set; } = new();

        public async Task Initialize()
        {
            Accounts.Clear();

            var accounts = await _mediator.Send(new GetAllAccountsQuery(_userContext.UserId));

            foreach (var account in accounts)
            {
                Accounts.Add(account);
            }

            SelectedFromAccount = Accounts.FirstOrDefault();
            SelectedToAccount = Accounts.FirstOrDefault();

            Amount = string.Empty;
            Description = string.Empty;
            Date = DateTime.Now;
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
                var description = Description;

                if (String.IsNullOrWhiteSpace(description))
                {
                    throw new ArgumentException("Enter Description!");
                }

                if (!decimal.TryParse(Amount ?? "", out var amount) || amount <= 0)
                {
                    throw new ArgumentException("Wrong Amount Format! (Decimal >= 0)");
                }

                var date = Date;

                if (SelectedFromAccount is null || SelectedToAccount is null)
                {
                    throw new ArgumentException($"Select Accounts!");
                }

                if (SelectedFromAccount.Id == SelectedToAccount.Id)
                {
                    throw new ArgumentException($"Select Different Accounts!");
                }


                CloseAction?.Invoke(new CreateTransferCommand(
                    _userContext.UserId,
                    amount,
                    description,
                    date,
                    SelectedFromAccount.Id,
                    SelectedToAccount.Id
                    ));
            }
            catch (ArgumentException ex)
            {
                if (Shell.Current is not null)
                {
                    await Shell.Current.DisplayAlert(
                       "Can't Create Transfer",
                       ex.Message,
                       "OK");
                }
            }
           
        }
    }
}
