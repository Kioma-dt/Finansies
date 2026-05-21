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

namespace UI.ViewModels
{
    public class DisplayedTransfer
    {
        public Guid Id { get; set; }
        public string? Desciption { get; set; }
        public string? Amount { get; set; }
        public string? Date {  get; set; }
        public string? FromAccountName { get; set; }
        public string? ToAccountName { get; set; }
    }

    public partial class TransfersViewModel 
        : ObservableObject,
        IRecipient<DataBaseChangedMessage>,
        IRecipient<DateRangeChangedMessage>
    {
        private readonly IMediator _mediator;
        private readonly IUserContext _userContext;
        private readonly TransferCreatePopUp _popUp;

        private List<Transfer> _transfers = new();
        private DateTime _startDate = DateTime.Now.AddMonths(-1);
        private DateTime _endDate = DateTime.Now;

        public ObservableCollection<DisplayedTransfer> DisplayedTransfers { get; set; } = new();


        public TransfersViewModel(
            IMediator mediator,
            IUserContext user,
            TransferCreatePopUp popUp)
        {
            _mediator = mediator;
            _userContext = user;
            _popUp = popUp;

            WeakReferenceMessenger.Default.Register<DataBaseChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<DateRangeChangedMessage>(this);
        }
        public async void Receive(DataBaseChangedMessage message)
        {
            if (message.Type == DataBaseChangedMessageType.Init || message.Type == DataBaseChangedMessageType.PlannedTransactions)
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

        private void ShowTransfers()
        {
            DisplayedTransfers.Clear();

            var transfers = _transfers.Where(x => x.Date.Date >= _startDate.Date && x.Date.Date <= _endDate.Date).ToList();

            foreach (var transfer in transfers)
            {
                DisplayedTransfers.Add(new DisplayedTransfer()
                {
                    Id = transfer.Id,
                    Desciption = transfer.Description,
                    Amount = transfer.Amount.ToString(),
                    Date = transfer.Date.ToString("dd.MM.yyyy"),
                    FromAccountName = transfer?.FromAccount?.Name,
                    ToAccountName = transfer?.ToAccount?.Name
                });
            }
        }

        [RelayCommand]
        public async Task TransferMoney()
        {
            try
            {
                var result = await Application.Current.MainPage
                    .ShowPopupAsync<CreateTransferCommand?>(_popUp);

                var command = result.Result;

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
    }
}
