using BuisnessLogic.DTO;
using BuisnessLogic.Entities;
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
    public partial class DebtViewModel : ObservableObject, 
        IRecipient<DataBaseChangedMessage>,
        IRecipient<CurrentTimeMessage>
    {
        private readonly IDebtService _debtService;
        private readonly IUserContext _user;
        private readonly DebtCreatePopUp _popup;


        public ObservableCollection<Debt> Debts { get; set; } = new();
        public ObservableCollection<Transaction> Transactions { get; set; } = new();

        [ObservableProperty]
        public partial Debt? SelectedDebt { get; set; } = null;


        public DebtViewModel(
            IDebtService debtService,
            IUserContext user,
            DebtCreatePopUp popup)
        {
            _debtService = debtService;
            _user = user;
            _popup = popup;

            WeakReferenceMessenger.Default.Register<DataBaseChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<CurrentTimeMessage>(this);
        }
        public async void Receive(DataBaseChangedMessage message)
        {
            if (message.Type == DataBaseChangedMessageType.Init || message.Type == DataBaseChangedMessageType.Debts)
            {
                var data = await _debtService.GetAll(_user.UserId);

                Debts.Clear();
                foreach (var t in data)
                    Debts.Add(t);
            }
        }

        public async void Receive(CurrentTimeMessage message)
        {
            var data = await _debtService.GetAll(_user.UserId);

            foreach (var debt in data)
            {
                await _debtService.UpdateDebt(_user.UserId, debt.Id, message.CurrentTime);
            }

            WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Debts));
        }

        [RelayCommand]
        public async Task SelectDebt()
        {
            Transactions.Clear();
            if (SelectedDebt is not null)
            {
                var transactions = await _debtService.GetRelevantTransactions(_user.UserId, SelectedDebt.Id);

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
                var result = await Application.Current.MainPage
                .ShowPopupAsync<DebtCreateDTO?>(_popup);

                var data = result.Result;

                if (data is null)
                    return;

                await _debtService.CreateDebt(_user.UserId, data);
                WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Debts));
                if (data.IsAutoPlanned)
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
