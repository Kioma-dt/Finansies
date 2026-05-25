using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UI.Messages;
using UI.Popups;
using UI.Views;

using BuisnessLogic.UseCases.TransactionsUseCasses.Commands;
using UI.PopUps.Service;
using UI.Views.Service;

namespace UI.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly IUserContext _user;
        private readonly IMediator _mediator;
        private readonly IPopUpService _popUpService;
        private readonly IViewService _viewService;

        [ObservableProperty]
        public partial View? LeftView { get; set; }

        [ObservableProperty]
        public partial View? RightView { get; set; }

        [ObservableProperty]
        public partial View? CentralView { get; set; }

        [ObservableProperty]
        public partial View? DateRangeSelectorView { get; set; }

        public MainPageViewModel( IUserContext user,
            IMediator mediator,
            IPopUpService popUpService,
            IViewService viewService)
        {
            _user = user;
            _mediator = mediator;
            _popUpService = popUpService;
            _viewService = viewService;

            LeftView = _viewService.GetView<AccountView>();
            RightView = _viewService.GetView<TransactionView>();
            CentralView = null;
            DateRangeSelectorView = _viewService.GetView<DateRangeSelectorView>();
        }

        [RelayCommand]
        public void ShowTransactions()
        {
            LeftView = _viewService.GetView<AccountView>();
            RightView = _viewService.GetView<TransactionView>();
            CentralView = null;
        }

        [RelayCommand]
        public void ShowTransfers()
        {
            LeftView = _viewService.GetView<AccountView>();
            RightView = _viewService.GetView<TransferView>();
            CentralView = null;
        }

        [RelayCommand]
        public void ShowCategories()
        {
            LeftView = _viewService.GetView<AccountView>();
            RightView = _viewService.GetView<CategoryView>();
            CentralView = null;
        }

        [RelayCommand]
        public void ShowFamilyMembers()
        {
            LeftView = _viewService.GetView<AccountView>();
            RightView = _viewService.GetView<FamilyMemberView>();

            CentralView = null;
        }


        [RelayCommand]
        public void ShowPlannedTransactions()
        {
            LeftView = _viewService.GetView<AccountView>();
            RightView = _viewService.GetView<PlannedTransactionView>();
            CentralView = null;
        }

        [RelayCommand]
        public void ShowBudgets()
        {
            LeftView = null;
            RightView = null;

            CentralView = _viewService.GetView<BudgetView>();
        }

        [RelayCommand]
        public void ShowDebts()
        {
            LeftView = null;
            RightView = null;

            CentralView = _viewService.GetView<DebtView>();
        }

        [RelayCommand]
        public async Task CreateTransaction()
        {
            try
            {
                var command = await _popUpService.ShowPopUp<CreateTransactionCommand?, TransactionCreatePopUp>();

                if (command is null)
                    return;

                await _mediator.Send(command);

                WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Accounts));
                WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Transactions));
                WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Debts));
            }
            catch (ArgumentException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Can't create Transaction", $"{ex.Message}", "OK");
            }

        }

        [RelayCommand]
        public void Load()
        {
            WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Init));
            WeakReferenceMessenger.Default.Send(new CurrentTimeMessage(DateTime.Now));
        }
    }
}
