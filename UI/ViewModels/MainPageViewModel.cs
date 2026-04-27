using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Services;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using UI.Popups;
using UI.Views;

namespace UI.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial AccountView AccountView { get; set; }

        [ObservableProperty]
        public partial TransactionView TransactionView { get; set; }

        public MainPageViewModel(AccountView accountView, TransactionView transactionView)
        {
            AccountView = accountView;
            TransactionView = transactionView;
        }

        [RelayCommand]
        public async Task Load()
        {
            await TransactionView.LoadContent();

            await AccountView.LoadContent();
        }
    }
}
