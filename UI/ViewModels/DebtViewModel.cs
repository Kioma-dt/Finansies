using BuisnessLogic.DTO;
using BuisnessLogic.Entities;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using UI.Messages;
using UI.Popups;

using BuisnessLogic.UseCases.DebtsUseCasses.Queries;
using BuisnessLogic.UseCases.DebtsUseCasses.Commands;
using UI.PopUps.Service;

namespace UI.ViewModels
{
    public partial class DebtViewModel : ObservableObject, 
        IRecipient<DataBaseChangedMessage>,
        IRecipient<CurrentTimeMessage>,
        IRecipient<DateRangeChangedMessage>
    {
        private readonly IMediator _mediator;
        private readonly IUserContext _user;
        private readonly IPopUpService _popupService;


        public ObservableCollection<Debt> Debts { get; set; } = new();
        public ObservableCollection<Transaction> Transactions { get; set; } = new();

        [ObservableProperty]
        public partial Debt? SelectedDebt { get; set; } = null;

        private DateTime _startDate = DateTime.Now.AddMonths(-1);
        private DateTime _endDate = DateTime.Now;


        public DebtViewModel(
            IMediator mediator,
            IUserContext user,
            IPopUpService popup)
        {
            _mediator = mediator;
            _user = user;
            _popupService = popup;

            WeakReferenceMessenger.Default.Register<DataBaseChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<CurrentTimeMessage>(this);
            WeakReferenceMessenger.Default.Register<DateRangeChangedMessage>(this);
        }
        public async void Receive(DataBaseChangedMessage message)
        {
            if (message.Type == DataBaseChangedMessageType.Init || message.Type == DataBaseChangedMessageType.Debts)
            {
                var data = await _mediator.Send(new GetAllDebtsQuery(_user.UserId));

                Debts.Clear();
                foreach (var t in data)
                    Debts.Add(t);
            }
        }

        public async void Receive(CurrentTimeMessage message)
        {
            var data = await _mediator.Send(new GetAllDebtsQuery(_user.UserId));

            foreach (var debt in data)
            {
                await _mediator.Send(new UpdateDebtCommand(_user.UserId, debt.Id, message.CurrentTime));
            }

            WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Debts));
        }

        public async void Receive(DateRangeChangedMessage message)
        {
            _startDate = message.StartDate;
            _endDate = message.EndDate;

            await this.SelectDebtCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        public async Task SelectDebt()
        {
            Transactions.Clear();
            if (SelectedDebt is not null)
            {
                var transactions = await _mediator.Send(new GetRelevantTransactionsForDebtQuery(_user.UserId, SelectedDebt.Id));

                transactions = transactions.Where(x => x.Date.Date >= _startDate.Date && x.Date.Date <= _endDate.Date).ToList();

                foreach (var transaction in transactions)
                {
                    Transactions.Add(transaction);
                }
            }
        }

        [RelayCommand]
        public async Task CreateDebt()
        {
            try
            {
                var command = await _popupService.ShowPopUp<CreateDebtCommand,DebtCreatePopUp>();

                if (command is null)
                    return;

                await _mediator.Send(command);
                WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Debts));
                if (command.IsAutoPlanned)
                {
                    WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.PlannedTransactions));
                }
            }
            catch (FormatException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Can't Create Budget", $"{ex.Message}", "OK");
            }
            catch (ArgumentException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Can't Create Budget", $"{ex.Message}", "OK");
            }

        }
    }
}
