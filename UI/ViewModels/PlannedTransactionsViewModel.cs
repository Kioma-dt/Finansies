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
using UI.PopUps.Service;

namespace UI.ViewModels
{
    public class DisplayedPlanTransaction(Guid Id,
        string? Description,
        string? Amount,
        string? Type,
        string? Status,
        string? PlannedDate,
        string? AccountName,
        string? CategoryName,
        string? FamilyMemberName,
        string? DebtName)
    {
        public Guid Id { get; } = Id;
        public string? Description { get; } = Description;
        public string? Amount { get; } = Amount;
        public string? Type { get; } = Type;
        public string? Status { get;} = Status;
        public string? PlannedDate { get; } = PlannedDate;
        public string? AccountName { get; } = AccountName;
        public string? CategoryName { get; } = CategoryName;
        public string? FamilyMemberName { get; } = FamilyMemberName;
        public string? DebtName { get; } = DebtName;
    }

    public partial class PlannedTransactionsViewModel : ObservableObject, 
        IRecipient<DataBaseChangedMessage>,
        IRecipient<CurrentTimeMessage>,
        IRecipient<DateRangeChangedMessage>,
        IRecipient<SelectedAccountChangedMessage>
    {
        private readonly IMediator _mediator;
        private readonly IUserContext _user;
        private readonly IPopUpService _popUpService;

        private List<PlannedTransaction> _plannedTransactions = new();

        private DateTime _startDate = DateTime.Now.AddMonths(-1);
        private DateTime _endDate = DateTime.Now;

        private Guid? _selectedAccountId = null;

        public ObservableCollection<DisplayedPlanTransaction> DisplayedPlannedTransactions { get; set; } = new();

        [ObservableProperty]
        public partial bool IsLoaded { get; set; } = false;

        public PlannedTransactionsViewModel(
            IMediator mediator,
            IUserContext user,
            IPopUpService popUp)
        {
            _mediator = mediator;
            _user = user;
            _popUpService = popUp;

            WeakReferenceMessenger.Default.Register<DataBaseChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<CurrentTimeMessage>(this);
            WeakReferenceMessenger.Default.Register<DateRangeChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<SelectedAccountChangedMessage>(this);
        }
        public async void Receive(DataBaseChangedMessage message)
        {
            if (message.Type == DataBaseChangedMessageType.Init || message.Type == DataBaseChangedMessageType.PlannedTransactions)
            {
                var data = await _mediator.Send(new GetAllPlannedTransactionsQuery(_user.UserId));

                _plannedTransactions.Clear();
                foreach (var t in data)
                    _plannedTransactions.Add(t);

                this.ShowPlannedTransactions();
            }
        }

        public async void Receive(CurrentTimeMessage message)
        {
            var data = await _mediator.Send(new GetAllPlannedTransactionsQuery(_user.UserId));

            foreach (var pt in data)
            {
                await _mediator.Send(new UpdatePlannedTransactionCommand(_user.UserId, pt.Id, message.CurrentTime));
            }

            WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Debts));
        }

        public void Receive(DateRangeChangedMessage message)
        {
            _startDate = message.StartDate;
            _endDate = message.EndDate;

            this.ShowPlannedTransactions();
        }

        public void Receive(SelectedAccountChangedMessage message)
        {
            _selectedAccountId = message.SelectedAccountId;

            this.ShowPlannedTransactions();
        }

        private void ShowPlannedTransactions()
        {
            DisplayedPlannedTransactions.Clear();

            var trans = _plannedTransactions.Where(x => x.PlannedDate.Date >= _startDate.Date && x.PlannedDate.Date <= _endDate.Date).ToList();

            if (_selectedAccountId is not null)
            {
                trans = trans.Where(x => x.AccountId == _selectedAccountId).ToList();
            }

            foreach (var t in trans)
            {
                DisplayedPlannedTransactions.Add(new DisplayedPlanTransaction(t.Id,
                    t.Description,
                    t.Amount.ToString(),
                    t.Type.ToString(),
                    t.Status.ToString(),
                    t.PlannedDate.ToString("dd.MM.yyyy"),
                    t.Account?.Name,
                    t.Category?.Name,
                    t.FamilyMember?.Name,
                    t.Debt?.Name));
            }
        }

        [RelayCommand]
        public async Task PlanTransaction()
        {
            try
            {
                var command = await _popUpService.ShowPopUp<CreatePlannedTransactionCommand?, PlannedTransactionCreatePopUp>();
                if (command is null)
                    return;

                await _mediator.Send(command);

                WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.PlannedTransactions));
            }
            catch (FormatException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Can't plan Transaction", $"{ex.Message}", "OK");
            }
            catch (ArgumentException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Can't plan Transaction", $"{ex.Message}", "OK");
            }

        }

        [RelayCommand]
        public async Task ConfirmTransaction(PlannedTransaction planned)
        {
            try
            {
                if (planned is null)
                    return;

                await _mediator.Send(new ConfirmPlannedTransactionCommand(_user.UserId, planned.Id));

                WeakReferenceMessenger.Default.Send(
                    new DataBaseChangedMessage(DataBaseChangedMessageType.PlannedTransactions));
                WeakReferenceMessenger.Default.Send(
                    new DataBaseChangedMessage(DataBaseChangedMessageType.Transactions));
                WeakReferenceMessenger.Default.Send(
                    new DataBaseChangedMessage(DataBaseChangedMessageType.Accounts));
                WeakReferenceMessenger.Default.Send(
                    new DataBaseChangedMessage(DataBaseChangedMessageType.Debts));
            }
            catch (FormatException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Can't confirm Transaction", $"{ex.Message}", "OK");
            }
            catch (ArgumentException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Can't confirm Transaction", $"{ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public void Load()
        {
            WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.PlannedTransactions));
            WeakReferenceMessenger.Default.Send(new CurrentTimeMessage(DateTime.Now));
        }
    }
}
