using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Services;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using UI.Messages;
using UI.Popups;

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

    public partial class BudgetViewModel : ObservableObject, IRecipient<DataBaseChangedMessage>
    {
        private readonly IBudgetService _budgetService;
        private readonly IUserContext _user;
        private readonly BudgetCreatePopUp _popup;


        public ObservableCollection<BudgetItem> Budgets { get; set; } = new();
        public ObservableCollection<Transaction> Transactions { get; set; } = new();

        [ObservableProperty]
        public partial BudgetItem? SelectedBudget { get; set; } = null;


        public BudgetViewModel(
            IBudgetService budgetService,
            IUserContext user,
            BudgetCreatePopUp popup)
        {
            _budgetService = budgetService;
            _user = user;

            WeakReferenceMessenger.Default.Register<DataBaseChangedMessage>(this);
            _popup = popup;
        }
        public async void Receive(DataBaseChangedMessage message)
        {
            if (message.Type == DataBaseChangedMessageType.Init || message.Type == DataBaseChangedMessageType.Budgets)
            {
                var data = await _budgetService.GetAll(_user.UserId);

                Budgets.Clear();
                foreach (var t in data)
                    Budgets.Add(new BudgetItem()
                    {
                        Budget = t,
                        UsedAmount = await _budgetService.CountTransactions(_user.UserId, t.Id)
                    });
            }
        }

        [RelayCommand]
        public async Task SelectBudget()
        {
            Transactions.Clear();
            if (SelectedBudget is not null)
            {
                var transactions = await _budgetService.GetRelevantTransactions(_user.UserId, SelectedBudget.Budget.Id);

                foreach(var transaction in transactions)
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
                var result = await Application.Current.MainPage
                .ShowPopupAsync<BudgetCreateDTO?>(_popup);

                var data = result.Result;

                if (data is null)
                    return;

                await _budgetService.CreateBudget(_user.UserId, data.Name, data.Limit, data.StartDate, data.EndDate, data.Filters);

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
