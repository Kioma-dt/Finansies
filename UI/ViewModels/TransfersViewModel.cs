using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using UI.Messages;
using UI.Popups;

using BuisnessLogic.UseCases.PlannedTransactionsUsesCasses.Queries;
using BuisnessLogic.UseCases.PlannedTransactionsUsesCasses.Commands;
using BuisnessLogic.UseCases.TransfersUseCasses.Queries;
using BuisnessLogic.UseCases.TransfersUseCasses.Commands;
using UI.PopUps.Service;

namespace UI.ViewModels
{
    public class DisplayedTransfer(Guid Id,
        string? Desciption,
        string? Amount,
        string? Date,
        string? FromAccountName,
        string? ToAccountName)
    {
        public Guid Id { get; } = Id;
        public string? Desciption { get; } = Desciption;
        public string? Amount { get;  } = Amount;
        public string? Date {  get;  } = Date;
        public string? FromAccountName { get; } = FromAccountName;
        public string? ToAccountName { get; } = ToAccountName;
    }

    public partial class TransfersViewModel 
        : ObservableObject,
        IRecipient<DataBaseChangedMessage>,
        IRecipient<DateRangeChangedMessage>,
        IRecipient<SelectedAccountChangedMessage>
    {
        private readonly IMediator _mediator;
        private readonly IUserContext _userContext;
        private readonly IPopUpService _popUpService;

        private List<Transfer> _transfers = new();

        private DateTime _startDate = DateTime.Now.AddMonths(-1);
        private DateTime _endDate = DateTime.Now;

        private Guid? _selectedAccountId = null;

        public ObservableCollection<DisplayedTransfer> DisplayedTransfers { get; set; } = new();


        public TransfersViewModel(
            IMediator mediator,
            IUserContext user,
            IPopUpService popUpService)
        {
            _mediator = mediator;
            _userContext = user;
            _popUpService = popUpService;

            WeakReferenceMessenger.Default.Register<DataBaseChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<DateRangeChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<SelectedAccountChangedMessage>(this);
        }
        public async void Receive(DataBaseChangedMessage message)
        {
            if (message.Type == DataBaseChangedMessageType.Init || message.Type == DataBaseChangedMessageType.Transfers)
            {
                var data = await _mediator.Send(new GetAllTransfersQuery(_userContext.UserId));

                _transfers.Clear();
                foreach (var t in data)
                    _transfers.Add(t);

                this.ShowTransfers();
            }
        }

        public async void Receive(CurrentTimeMessage message)
        {
            var data = await _mediator.Send(new GetAllPlannedTransactionsQuery(_userContext.UserId));

            foreach (var pt in data)
            {
                await _mediator.Send(new UpdatePlannedTransactionCommand(_userContext.UserId, pt.Id, message.CurrentTime));
            }

            WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Debts));
        }

        public void Receive(DateRangeChangedMessage message)
        {
            _startDate = message.StartDate;
            _endDate = message.EndDate;

            this.ShowTransfers();
        }

        public void Receive(SelectedAccountChangedMessage message)
        {
            _selectedAccountId = message.SelectedAccountId;

            this.ShowTransfers();
        }

        private void ShowTransfers()
        {
            DisplayedTransfers.Clear();

            var transfers = _transfers.Where(x => x.Date.Date >= _startDate.Date && x.Date.Date <= _endDate.Date).ToList();

            if (_selectedAccountId is not null)
            {
                transfers = transfers.Where(x => x.FromAccountId == _selectedAccountId || x.ToAccountId == _selectedAccountId).ToList();
            }

            foreach (var transfer in transfers)
            {
                DisplayedTransfers.Add(new DisplayedTransfer
                    (
                        transfer.Id,
                        transfer.Description,
                        transfer.Amount.ToString(),
                        transfer.Date.ToString("dd.MM.yyyy"),
                        transfer?.FromAccount?.Name,
                        transfer?.ToAccount?.Name
                    ));
            }
        }

        [RelayCommand]
        public async Task TransferMoney()
        {
            try
            {
                var command = await _popUpService.ShowPopUp<CreateTransferCommand?, TransferCreatePopUp>();

                if (command is null)
                    return;

                await _mediator.Send(command);

                WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.PlannedTransactions));
                WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Accounts));
            }
            catch (FormatException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Can't Transfer Money", $"{ex.Message}", "OK");
            }
            catch (ArgumentException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Can't Transfer Money", $"{ex.Message}", "OK");
            }

        }

        [RelayCommand]
        public void Load()
        {
            WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Transfers));
            //WeakReferenceMessenger.Default.Send(new CurrentTimeMessage(DateTime.Now));
        }
    }
}
