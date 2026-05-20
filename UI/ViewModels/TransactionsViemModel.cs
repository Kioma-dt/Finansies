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

    public partial class TransactionsViewModel : ObservableObject, 
        IRecipient<DataBaseChangedMessage>,
        IRecipient<DateRangeChangedMessage>
    {
        private readonly ITransactionService _transactionService;
        private readonly IUserContext _user;

        //public List<Transaction> _transactions { get; } = new();
        //public List<Category> _categories { get; } = new();

        //public ObservableCollection<object> FlatItems { get; } = new();

        private List<Transaction> _allTransactions = new();
        private DateTime _startDate = DateTime.Now.AddMonths(-1);
        private DateTime _endDate = DateTime.Now;

        public ObservableCollection<Transaction> Transactions { get; set; } = new();

        public TransactionsViewModel(
            ITransactionService transactionService,
            IUserContext user)
        {
            _transactionService = transactionService;
            _user = user;

            WeakReferenceMessenger.Default.Register<DataBaseChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<DateRangeChangedMessage>(this);
        }
        public async void Receive(DataBaseChangedMessage message)
        {
            if (message.Type == DataBaseChangedMessageType.Init || message.Type == DataBaseChangedMessageType.Transactions)
            {
                var data = await _transactionService.GetAll(_user.UserId);

                _allTransactions.Clear();
                foreach (var t in data)
                    _allTransactions.Add(t);

                this.FilterTransactions();
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
        public void Receive(DateRangeChangedMessage message)
        {
            _startDate = message.StartDate;
            _endDate = message.EndDate;

            this.FilterTransactions();
        }

        private void FilterTransactions()
        {
            Transactions.Clear();

            var trans = _allTransactions.Where(x => x.Date.Date >= _startDate.Date && x.Date.Date <= _endDate.Date).ToList();

            foreach(var t in trans)
            {
                Transactions.Add(t);
            }
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


    }
}
