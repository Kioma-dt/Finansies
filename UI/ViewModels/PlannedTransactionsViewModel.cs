using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.UseCases.PlannedTransactionsUsesCasses.Commands;
using BuisnessLogic.UseCases.PlannedTransactionsUsesCasses.Queries;
using BuisnessLogic.UseCases.TransactionsUseCasses.Commands;
using BuisnessLogic.UseCases.TransactionsUseCasses.Queries;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using UI.Messages;
using UI.OrderingServices;
using UI.Popups;
using UI.PopUps.Service;
using UI.PopUps.ViewModels;

namespace UI.ViewModels
{
    public enum PlannedTransactionsOrderBy
    {
        Description,
        PlannedDate,
        Status,
        Amount,
        Type,
        AccountName,
        CategoryName,
        FamilyMemberName,
        DebtName
    }
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
        private readonly IUserContext _userContext;
        private readonly IPopUpService _popUpService;
        private readonly IPlannedTransactionsOrderingServiceFactory _plannedTransactionsOrderingServiceFactory;

        private List<PlannedTransaction> _plannedTransactions = new();

        private DateTime _startDate = DateTime.Now.AddMonths(-1);
        private DateTime _endDate = DateTime.Now;

        private Guid? _selectedAccountId = null;

        private PlannedTransactionsOrderBy _orderBy;
        private bool _ascending;

        public ObservableCollection<DisplayedPlanTransaction> DisplayedPlannedTransactions { get; set; } = new();

        [ObservableProperty]
        public partial bool IsLoaded { get; set; } = false;

        public PlannedTransactionsViewModel(
            IMediator mediator,
            IUserContext user,
            IPopUpService popUp,
            IPlannedTransactionsOrderingServiceFactory plannedTransactionsOrderingServiceFactory)
        {
            _mediator = mediator;
            _userContext = user;
            _popUpService = popUp;
            _plannedTransactionsOrderingServiceFactory = plannedTransactionsOrderingServiceFactory;

            WeakReferenceMessenger.Default.Register<DataBaseChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<CurrentTimeMessage>(this);
            WeakReferenceMessenger.Default.Register<DateRangeChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<SelectedAccountChangedMessage>(this);
        }
        public async void Receive(DataBaseChangedMessage message)
        {
            if (message.Type == DataBaseChangedMessageType.Init || message.Type == DataBaseChangedMessageType.PlannedTransactions)
            {
                var data = await _mediator.Send(new GetAllPlannedTransactionsQuery(_userContext.UserId));

                _plannedTransactions.Clear();
                foreach (var t in data)
                    _plannedTransactions.Add(t);

                _orderBy = PlannedTransactionsOrderBy.Status;
                _ascending = true;

                this.ShowPlannedTransactions();
            }
        }

        public async void Receive(CurrentTimeMessage message)
        {
            var data = await _mediator.Send(new GetAllPlannedTransactionsQuery(_userContext.UserId));

            foreach (var pt in data)
            {
                await _mediator.Send(new UpdatePlannedTransactionStatusCommand(_userContext.UserId, pt.Id, message.CurrentTime));
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

            var displayedPlannedTransactions = trans
                .Select(t => new DisplayedPlanTransaction(t.Id,
                    t.Description,
                    t.Amount.ToString(),
                    t.Type.ToString(),
                    t.Status.ToString(),
                    t.PlannedDate.ToString("dd.MM.yyyy"),
                    t.Account?.Name,
                    t.Category?.Name,
                    t.FamilyMember?.Name,
                    t.Debt?.Name));

            var orderingService = _plannedTransactionsOrderingServiceFactory.Create(_orderBy);

            displayedPlannedTransactions = orderingService.Order(displayedPlannedTransactions, _ascending);

            foreach (var transaction in displayedPlannedTransactions)
            {
                DisplayedPlannedTransactions.Add(transaction);
            }
        }

        [RelayCommand]
        public async Task ChangeSorting(string field)
        {
            var orderBy = Enum.Parse<PlannedTransactionsOrderBy>(field);

            if (orderBy == _orderBy)
            {
                _ascending = !_ascending;
            }
            else
            {
                _orderBy = orderBy;
                _ascending = true;
            }

            this.ShowPlannedTransactions();
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
        public async Task ConfirmTransaction(DisplayedPlanTransaction planned)
        {
            try
            {
                if (planned is null)
                    return;

                await _mediator.Send(new ConfirmPlannedTransactionCommand(_userContext.UserId, planned.Id));

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
        public async Task UpdatePlannedTransaction(DisplayedPlanTransaction displayedPlanTransaction)
        {
            var plannedTransaction = await _mediator.Send(new GetPlannedTransactionIdQuery(_userContext.UserId,
                displayedPlanTransaction.Id));

            var command = await _popUpService.ShowPopUp<UpdatePlannedTransactionCommand?,
                PlannedTransactionUpdatePopUp,
                PlannedTransactionUpdatePopUpModel,
                PlannedTransactionUpdatePopUpModelParameters>(new PlannedTransactionUpdatePopUpModelParameters(
                    plannedTransaction.Id,
                    plannedTransaction.Amount,
                    plannedTransaction.Description,
                    plannedTransaction.Type,
                    plannedTransaction.PlannedDate,
                    plannedTransaction.AccountId,
                    plannedTransaction.CategoryId,
                    plannedTransaction.FamilyMemberId,
                    plannedTransaction.DebtId
                ));

            if (command is null)
            {
                return;
            }

            await _mediator.Send(command);

            WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.PlannedTransactions));
        }

        [RelayCommand]
        public void Load()
        {
            WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.PlannedTransactions));
            WeakReferenceMessenger.Default.Send(new CurrentTimeMessage(DateTime.Now));
        }
    }
}
