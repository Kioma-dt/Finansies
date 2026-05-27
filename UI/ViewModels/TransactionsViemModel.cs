using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using BuisnessLogic.UseCases.TransactionsUseCasses.Queries;
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
using UI.OrderingServices;
using UI.Popups;
using UI.PopUps.Service;
using UI.Views;
using UI.PopUps.ViewModels;
using BuisnessLogic.UseCases.TransactionsUseCasses.Commands;
using BuisnessLogic.UseCases.TransactionsUseCasses.Queries;

namespace UI.ViewModels
{
    public enum TransactionsOrderBy
    {
        Description,
        Date,
        Amount,
        Type,
        AccountName,
        CategoryName,
        FamilyMemberName,
        DebtName
    }

    public class DisplayedTransaction(Guid Id,
        string? Description,
        string? Amount,
        string? Type,
        string? Date,
        string? AccountName,
        string? CategoryName,
        string? FamilyMemberName,
        string? DebtName)
    {
        public Guid Id { get;} = Id;
        public string? Description { get; } = Description;
        public string? Amount { get;} = Amount;
        public string? Type { get; } = Type;
        public string? Date {  get;} = Date;
        public string? AccountName { get; } = AccountName;
        public string? CategoryName { get; } = CategoryName;
        public string? FamilyMemberName { get; } = FamilyMemberName;
        public string? DebtName { get; } = DebtName;
    }
    public partial class TransactionsViewModel : ObservableObject, 
        IRecipient<DataBaseChangedMessage>,
        IRecipient<DateRangeChangedMessage>,
        IRecipient<SelectedAccountChangedMessage>
    {
        private readonly IMediator _mediator;
        private readonly IUserContext _userContext;
        private readonly IPopUpService _popupService;
        private readonly ITransactionsOrderingServiceFactory _transactionsOrderingServiceFactory;


        private List<Transaction> _transactions = new();

        private DateTime _startDate = DateTime.Now.AddMonths(-1);
        private DateTime _endDate = DateTime.Now;

        private Guid? _selectedAccountId = null;

        private TransactionsOrderBy _orderBy;
        private bool _ascending;

        public ObservableCollection<DisplayedTransaction> DisplayedTransactions { get; set; } = new();

        public string DescriptionSortArrow =>
            GetSortArrow(TransactionsOrderBy.Description);

        public string AmountSortArrow =>
            GetSortArrow(TransactionsOrderBy.Amount);

        public string DateSortArrow =>
            GetSortArrow(TransactionsOrderBy.Date);

        public string TypeSortArrow =>
            GetSortArrow(TransactionsOrderBy.Type);

        public string AccountNameSortArrow =>
            GetSortArrow(TransactionsOrderBy.AccountName);

        public string CategoryNameSortArrow =>
            GetSortArrow(TransactionsOrderBy.CategoryName);

        public string FamilyMemberNameSortArrow =>
            GetSortArrow(TransactionsOrderBy.FamilyMemberName);

        public string DebtNameSortArrow =>
            GetSortArrow(TransactionsOrderBy.DebtName);

        public TransactionsViewModel(
            IMediator mediator,
            IUserContext user,
            IPopUpService popUpService,
            ITransactionsOrderingServiceFactory transactionsOrderingServiceFactory)
        {
            _mediator = mediator;
            _userContext = user;
            _popupService = popUpService;
            _transactionsOrderingServiceFactory = transactionsOrderingServiceFactory;

            _orderBy = TransactionsOrderBy.Date;
            _ascending = true;

            WeakReferenceMessenger.Default.Register<DataBaseChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<DateRangeChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<SelectedAccountChangedMessage>(this);
        }
        public async void Receive(DataBaseChangedMessage message)
        {
            if (message.Type == DataBaseChangedMessageType.Init || message.Type == DataBaseChangedMessageType.Transactions)
            {
                var data = await _mediator.Send(new GetAllTransactionsQuery(_userContext.UserId));

                _transactions.Clear();
                foreach (var t in data)
                    _transactions.Add(t);

                this.ShowTransactions();
            }
        }
        public void Receive(DateRangeChangedMessage message)
        {
            _startDate = message.StartDate;
            _endDate = message.EndDate;

            this.ShowTransactions();
        }

        public void Receive(SelectedAccountChangedMessage message)
        {
            _selectedAccountId = message.SelectedAccountId;

            this.ShowTransactions();
        }

        private void ShowTransactions()
        {
            DisplayedTransactions.Clear();

            var trans = _transactions.Where(x => x.Date.Date >= _startDate.Date && x.Date.Date <= _endDate.Date).ToList();

            if (_selectedAccountId is not null)
            {
                trans = trans.Where(x => x.AccountId ==  _selectedAccountId).ToList();
            }

            var displayedTransactions = trans.Select(t => new DisplayedTransaction
                    (
                        t.Id,
                        t.Description,
                        t.SignedAmount.ToString(),
                        t.Type.ToString(),
                        t.Date.ToString("dd.MM.yyyy"),
                        t.Account?.Name,
                        t.Category?.Name,
                        t.FamilyMember?.Name,
                        t.Debt?.Name
                    ));

            displayedTransactions = _transactionsOrderingServiceFactory
                .Create(_orderBy)
                .Order(displayedTransactions, _ascending);

            foreach(var transaction in displayedTransactions)
            {
                DisplayedTransactions.Add(transaction);
            }

            OnPropertyChanged(nameof(DescriptionSortArrow));
            OnPropertyChanged(nameof(AmountSortArrow));
            OnPropertyChanged(nameof(DateSortArrow));
            OnPropertyChanged(nameof(TypeSortArrow));
            OnPropertyChanged(nameof(AccountNameSortArrow));
            OnPropertyChanged(nameof(CategoryNameSortArrow));
            OnPropertyChanged(nameof(FamilyMemberNameSortArrow));
            OnPropertyChanged(nameof(DebtNameSortArrow));
        }

        [RelayCommand]
        public async Task ChangeSorting(string field)
        {
            var orderBy = Enum.Parse<TransactionsOrderBy>(field);

            if (orderBy == _orderBy)
            {
                _ascending = !_ascending;
            }
            else
            {
                _orderBy = orderBy;
                _ascending = true;
            }

            this.ShowTransactions();
        }

        [RelayCommand]
        public async Task UpdateTransaction(DisplayedTransaction displayedTransaction)
        {
            var transaction  = await _mediator.Send(new GetTransactionsByIdQuery(_userContext.UserId, 
                displayedTransaction.Id));

            var command = await _popupService.ShowPopUp<UpdateTransactionCommand?, 
                TransactionUpdatePopUp,
                TransactionUpdatePopUpModel,
                TransactionUpdatePopUpModelParameters>(new TransactionUpdatePopUpModelParameters(
                    transaction.Id,
                    transaction.Description,
                    transaction.Date,
                    transaction.CategoryId,
                    transaction.FamilyMemberId
                ));

            if (command is null)
            {
                return;
            }

            await _mediator.Send(command);

            WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Transactions));
        }

        [RelayCommand]
        public void Load()
        {
            WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Transactions));
            //WeakReferenceMessenger.Default.Send(new CurrentTimeMessage(DateTime.Now));
        }

        public string GetSortArrow(TransactionsOrderBy orderBy)
        {
            if (_orderBy != orderBy)
                return string.Empty;

            return _ascending ? " ▲" : " ▼";
        }
    }
}
