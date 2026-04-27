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
        public partial AccountView LeftView { get; set; }

        [ObservableProperty]
        public partial View RightView { get; set; }


        private readonly TransactionView _transactionView;
        private readonly AccountView _accountView;
        private readonly CategoryView _categoryView;

        public MainPageViewModel(AccountView accountView, 
            TransactionView transactionView,
            CategoryView categoryView)
        {
            _accountView = accountView;
            _transactionView = transactionView;

            LeftView = _accountView;
            RightView = _transactionView;
            _categoryView = categoryView;
        }

        [RelayCommand]
        public void ShowTransactions()
        {
            RightView = _transactionView;
        }

        [RelayCommand]
        public void ShowCategories()
        {
            RightView = _categoryView;
        }

        [RelayCommand]
        public async Task Load()
        {
            await _transactionView.LoadContent();

            await _accountView.LoadContent();
        }
    }
}
