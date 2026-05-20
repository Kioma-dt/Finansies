using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Popups;
using UI.Messages;
using BuisnessLogic.UseCases.CategoryUseCasses.Queries;
using BuisnessLogic.UseCases.CategoryUseCasses.Commands;

namespace UI.ViewModels
{
    public class CategoryNode
    {
        public Category Category { get; set; } = null!;
        public decimal Amount { get; set; } = 0;
        public int NumberOfTransactions {  get; set; } = 0;
        public int Level { get; set; }
    }

    public partial class CategoryViewModel : ObservableObject, 
        IRecipient<DataBaseChangedMessage>,
        IRecipient<DateRangeChangedMessage>
    {
        private readonly IMediator _mediator;
        private readonly IUserContext _user;
        private readonly CategoryCreatePopUp _categoryCreatePopUp;

        private List<Category> _categories = new();

        private DateTime _startDate = DateTime.Now.AddMonths(-1);
        private DateTime _endDate = DateTime.Now;

        public ObservableCollection<CategoryNode> FlatCategories { get; } = new();


        public CategoryViewModel(
            IMediator mediator,
            IUserContext user,
            CategoryCreatePopUp categoryCreatePopUp)
        {
            _mediator = mediator;
            _user = user;
            _categoryCreatePopUp = categoryCreatePopUp;

            WeakReferenceMessenger.Default.Register<DataBaseChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<DateRangeChangedMessage>(this);
        }

        public async void Receive(DataBaseChangedMessage message)
        {
            if (message.Type == DataBaseChangedMessageType.Init 
                || message.Type == DataBaseChangedMessageType.Categories
                || message.Type == DataBaseChangedMessageType.Transactions)
            {
                _categories = (await _mediator.Send(new GetAllCategoriesQuery(_user.UserId))).ToList();

                BuildTree();
                //this.FilterCategoryTransactions();
            }
        }

        public void Receive(DateRangeChangedMessage message)
        {
            _startDate = message.StartDate;
            _endDate = message.EndDate;

            this.BuildTree();
        }

        //private void FilterCategoryTransactions()
        //{
        //    foreach(var categoryNode in FlatCategories)
        //    {
        //        categoryNode.Amount = categoryNode.Category.PeriodTransactionsSum(_startDate, _endDate);
        //    }
        //}


        [RelayCommand]
        public async Task AddCategory()
        {
            var result = await Application.Current.MainPage
                .ShowPopupAsync<CreateCategoryCommand?>(_categoryCreatePopUp);

            var command = result.Result;

            if (command is null)
                return;


            await _mediator.Send(command);

            WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Categories));
        }


        private void BuildTree()
        {
            FlatCategories.Clear();

            void Add(Category cat, int level)
            {
                FlatCategories.Add(new CategoryNode
                {
                    Category = cat,
                    Amount = cat.PeriodTransactionsSum(_startDate, _endDate),
                    NumberOfTransactions = cat.PeriodTransactionsNumber(_startDate, _endDate),
                    Level = level
                });

                foreach (var child in cat.Children)
                    Add(child, level + 1);
            }

            var roots = _categories.Where(x => x.ParentId == null);

            foreach (var root in roots)
                Add(root, 0);
        }
    }
}
