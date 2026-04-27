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

    public partial class TransactionsViewModel : ObservableObject
    {
        private readonly ITransactionService _transactionService;
        private readonly IUserContext _user;

        public ObservableCollection<Transaction> Transactions { get; } = new();


        public TransactionsViewModel(
            ITransactionService transactionService,
            IUserContext user,
            AccountView accountView)
        {
            _transactionService = transactionService;
            _user = user;
        }

        [RelayCommand]
        public async Task Load()
        {
            var data = await _transactionService.GetAll(_user.UserId);

            Transactions.Clear();
            foreach (var t in data)
                Transactions.Add(t);
        }
    }
}
