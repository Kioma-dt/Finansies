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
using UI.PopUps.Service;

namespace UI.ViewModels
{
    public class DispalyedCategory(Guid Id,
        string? Name,
        string? Description,
        string? Amount,
        string? NumberOfTransactions,
        int Level)
    {
        public Guid Id { get; } = Id;
        public string? Name { get; } = Name;
        public string? Description { get; } = Description;
        public string? Amount { get; } = Amount;
        public string? NumberOfTransactions {  get;  } = NumberOfTransactions;
        public int Level { get; } = Level;
    }

    public partial class CategoryViewModel : ObservableObject, 
        IRecipient<DataBaseChangedMessage>,
        IRecipient<DateRangeChangedMessage>,
        IRecipient<SelectedAccountChangedMessage>
    {
        private readonly IMediator _mediator;
        private readonly IUserContext _userContext;
        private readonly IPopUpService _popUpService;

        private List<Category> _categories = new();

        private DateTime _startDate = DateTime.Now.AddMonths(-1);
        private DateTime _endDate = DateTime.Now;

        private Guid? _selectedAccountId = null;

        public ObservableCollection<DispalyedCategory> DispalyedCategories { get; } = new();


        public CategoryViewModel(
            IMediator mediator,
            IUserContext user,
            IPopUpService popUpService)
        {
            _mediator = mediator;
            _userContext = user;
            _popUpService = popUpService;

            WeakReferenceMessenger.Default.Register<DataBaseChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<DateRangeChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<SelectedAccountChangedMessage>(this);
        }

        public async void Receive(DataBaseChangedMessage message)
        {
            if (message.Type == DataBaseChangedMessageType.Init 
                || message.Type == DataBaseChangedMessageType.Categories
                || message.Type == DataBaseChangedMessageType.Transactions)
            {
                _categories.Clear();
                
                var categories = (await _mediator.Send(new GetAllCategoriesQuery(_userContext.UserId))).ToList();

                foreach(var category in categories)
                {
                    _categories.Add(category);
                }

                this.ShowCategories();
            }
        }

        public void Receive(DateRangeChangedMessage message)
        {
            _startDate = message.StartDate;
            _endDate = message.EndDate;

            this.ShowCategories();
        }
        public void Receive(SelectedAccountChangedMessage message)
        {
            _selectedAccountId = message.SelectedAccountId;

            this.ShowCategories();
        }

        [RelayCommand]
        public async Task AddCategory()
        {

            var command = await _popUpService.ShowPopUp<CreateCategoryCommand?, CategoryCreatePopUp>();

            if (command is null)
                return;


            await _mediator.Send(command);

            WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Categories));
        }


        private void ShowCategories()
        {
            DispalyedCategories.Clear();

            void Add(Category cat, int level)
            {
                DispalyedCategories.Add(new DispalyedCategory
                    (
                        cat.Id,
                        cat.Name,
                        cat.Description,
                        cat.PeriodTransactionsSum(_startDate, _endDate, _selectedAccountId).ToString(),
                        cat.PeriodTransactionsNumber(_startDate,_endDate, _selectedAccountId).ToString(),
                        level
                    )
                );

                foreach (var child in cat.Children)
                    Add(child, level + 1);
            }

            var roots = _categories.Where(x => x.ParentId == null);

            foreach (var root in roots)
                Add(root, 0);
        }
    }
}
