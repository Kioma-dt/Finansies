using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using BuisnessLogic.Services;
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

namespace UI.ViewModels
{
    public class TransactionTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CategoryTemplate { get; set; }
        public DataTemplate TransactionTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return item switch
            {
                TransactionNode => CategoryTemplate,
                Transaction => TransactionTemplate,
                _ => null
            };
        }
    }

    public class TransactionNode
    {
        public Category Category { get; set; } = null!;
        public int Level { get; set; }

        public List<Transaction> Transactions { get; set; } = new();
    }

    public partial class TransactionsViewModel : ObservableObject, IRecipient<DataBaseChangedMessage>
    {
        private readonly ITransactionService _transactionService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserContext _user;

        //public List<Transaction> _transactions { get; } = new();
        //public List<Category> _categories { get; } = new();

        //public ObservableCollection<object> FlatItems { get; } = new();

        public ObservableCollection<Transaction> Transactions { get; set; } = new();

        [ObservableProperty]
        public partial bool IsLoaded { get; set; } = false;

        public TransactionsViewModel(
            ITransactionService transactionService,
            ICategoryRepository categoryRepository,
            IUserContext user,
            AccountView accountView)
        {
            _transactionService = transactionService;
            _categoryRepository = categoryRepository;
            _user = user;

            WeakReferenceMessenger.Default.Register<DataBaseChangedMessage>(this);
        }
        public async void Receive(DataBaseChangedMessage message)
        {
            bool isUpdated = false;
            if (message.Type == DataBaseChangedMessageType.Init || message.Type == DataBaseChangedMessageType.Transactions)
            {
                var data = await _transactionService.GetAll(_user.UserId);

                Transactions.Clear();
                foreach (var t in data)
                    Transactions.Add(t);

                isUpdated = true;
            }

            //if (message.Type == DataBaseChangedMessageType.Init || message.Type == DataBaseChangedMessageType.Categories)
            //{
            //    var data = await _categoryRepository.GetAll(_user.UserId) ?? new();

            //    _categories.Clear();
            //    foreach (var t in data)
            //        _categories.Add(t);

            //    isUpdated = true;
            //}

            //if (isUpdated)
            //{
            //    BuildTree(_categories, _transactions);
            //}
        }

        //private void BuildTree(List<Category> categories, List<Transaction> transactions)
        //{
        //    FlatItems.Clear();

        //    void AddCategory(Category category, int level)
        //    {
        //        var catTransactions = transactions
        //            .Where(t => t.Category?.Id == category.Id)
        //            .ToList();

        //        FlatItems.Add(new TransactionNode
        //        {
        //            Category = category,
        //            Level = level,
        //            Transactions = catTransactions
        //        });

        //        foreach (var t in catTransactions)
        //            FlatItems.Add(t);

        //        foreach (var child in category.Children)
        //            AddCategory(child, level + 1);
        //    }

        //    var roots = categories.Where(c => c.ParentId == null);

        //    foreach (var root in roots)
        //        AddCategory(root, 0);
        //}


        //[RelayCommand]
        //public async Task Load()
        //{
        //    var data = await _transactionService.GetAll(_user.UserId);

        //    Transactions.Clear();
        //    foreach (var t in data)
        //        Transactions.Add(t);
        //}
    }
}
