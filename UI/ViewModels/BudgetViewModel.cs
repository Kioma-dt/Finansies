using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.UseCases.BudgetUseCasses.Queries;
using BuisnessLogic.UseCases.BudgetUseCasses.Commands;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using UI.Messages;
using UI.Popups;
using UI.PopUps.Service;

namespace UI.ViewModels
{
    public class BudgetItem
    {
        public Budget Budget { get; set; }
        public decimal UsedAmount { get; set; }

        public double UsedPercent =>
        Budget.Limit == 0 ? 0 : (double)(UsedAmount / Budget.Limit);

        public string LimitText => $"{UsedAmount:F0} / {Budget.Limit:F0}";
    }

    public partial class BudgetViewModel : ObservableObject,
        IRecipient<DataBaseChangedMessage>,
        IRecipient<DateRangeChangedMessage>
    {
        private readonly IMediator _mediator;
        private readonly IUserContext _user;
        private readonly IPopUpService _popupService;

        public ObservableCollection<BudgetItem> Budgets { get; set; } = new();
        public ObservableCollection<Transaction> Transactions { get; set; } = new();

        [ObservableProperty]
        public partial BudgetItem? SelectedBudget { get; set; } = null;

        private DateTime _startDate = DateTime.Now.AddMonths(-1);
        private DateTime _endDate = DateTime.Now;

        public BudgetViewModel(
            IMediator mediator,
            IUserContext user,
            IPopUpService popup)
        {
            _mediator = mediator;
            _user = user;
            _popupService = popup;

            WeakReferenceMessenger.Default.Register<DataBaseChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<DateRangeChangedMessage>(this);
        }
        public async void Receive(DataBaseChangedMessage message)
        {
            if (message.Type == DataBaseChangedMessageType.Init || message.Type == DataBaseChangedMessageType.Budgets)
            {
                var data = await _mediator.Send(new GetAllBudgetsQuery(_user.UserId));

                Budgets.Clear();
                foreach (var t in data)
                    Budgets.Add(new BudgetItem()
                    {
                        Budget = t,
                        UsedAmount = (await _mediator.Send(new GetRelevantTransactionsQuery(_user.UserId, t.Id))).Sum(x => x.Amount),
                    });
            }
        }
        public async void Receive(DateRangeChangedMessage message)
        {
            _startDate = message.StartDate;
            _endDate = message.EndDate;

            await this.SelectBudgetCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        public async Task SelectBudget()
        {
            Transactions.Clear();
            if (SelectedBudget is not null)
            {
                var transactions = await _mediator.Send(new GetRelevantTransactionsQuery(_user.UserId, SelectedBudget.Budget.Id));

                transactions = transactions.Where(x => x.Date.Date >= _startDate.Date && x.Date.Date <= _endDate.Date).ToList();

                foreach (var transaction in transactions)
                {
                    Transactions.Add(transaction);
                }
            }
        }

        [RelayCommand]
        public async Task CreateBudget()
        {
            try
            {
                var command = await _popupService.ShowPopUp<CreateBudgetCommand?, BudgetCreatePopUp>();

                if (command is null)
                    return;

                await _mediator.Send(command);

                WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Budgets));
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
