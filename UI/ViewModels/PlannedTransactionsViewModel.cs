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

    public partial class PlannedTransactionsViewModel : ObservableObject, 
        IRecipient<DataBaseChangedMessage>,
        IRecipient<CurrentTimeMessage>
    {
        private readonly IPlannedTransactionService _plannedTransactionService;
        private readonly IUserContext _user;
        private readonly PlannedTransactionCreatePopUp _popUp;


        public ObservableCollection<PlannedTransaction> PlannedTransactions { get; set; } = new();

        [ObservableProperty]
        public partial bool IsLoaded { get; set; } = false;

        public PlannedTransactionsViewModel(
            IPlannedTransactionService plannedTransactionService,
            IUserContext user,
            PlannedTransactionCreatePopUp popUp)
        {
            _plannedTransactionService = plannedTransactionService;
            _user = user;
            _popUp = popUp;

            WeakReferenceMessenger.Default.Register<DataBaseChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<CurrentTimeMessage>(this);
        }
        public async void Receive(DataBaseChangedMessage message)
        {
            if (message.Type == DataBaseChangedMessageType.Init || message.Type == DataBaseChangedMessageType.PlannedTransactions)
            {
                var data = await _plannedTransactionService.GetAll(_user.UserId);

                PlannedTransactions.Clear();
                foreach (var t in data)
                    PlannedTransactions.Add(t);
            }
        }

        public async void Receive(CurrentTimeMessage message)
        {
            var data = await _plannedTransactionService.GetAll(_user.UserId);

            foreach (var pt in data)
            {
                await _plannedTransactionService.UpdatePlannedTransaction(_user.UserId, pt.Id, message.CurrentTime);
            }

            WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Debts));
        }

        [RelayCommand]
        public async Task PlanTransaction()
        {
            try
            {
                var result = await Application.Current.MainPage
                .ShowPopupAsync<PlannedTransactionCreateDTO?>(_popUp);

                var data = result.Result;

                if (data is null)
                    return;

                if (data.Periodicity == TransactionPeriodicity.Once)
                {
                    await _plannedTransactionService.PlanTransaction(_user.UserId,
                        data.Amount,
                        data.Description,
                        data.Type,
                        data.StartDate,
                        data.AccountId,
                        data.CategoryId,
                        data.FamilyMemberId,
                        data.DebtId);
                }
                else
                {
                    await _plannedTransactionService.PlanMultipleTransactions(_user.UserId,
                        data.Amount,
                        data.Description,
                        data.Type,
                        data.StartDate,
                        data.Periodicity,
                        data.Count, 
                        data.AccountId,
                        data.CategoryId,
                        data.FamilyMemberId,
                        data.DebtId);
                }

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

                await _plannedTransactionService.ConfirmTransaction(_user.UserId, planned.Id);

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
    }
}
