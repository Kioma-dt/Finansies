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
using UI.Popups;
using UI.Views;

namespace UI.ViewModels
{
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
        private readonly IUserContext _user;


        private List<Transaction> _transactions = new();

        private DateTime _startDate = DateTime.Now.AddMonths(-1);
        private DateTime _endDate = DateTime.Now;

        private Guid? _selectedAccountId = null;

        public ObservableCollection<DisplayedTransaction> DisplayedTransactions { get; set; } = new();

        public TransactionsViewModel(
            IMediator mediator,
            IUserContext user)
        {
            _mediator = mediator;
            _user = user;

            WeakReferenceMessenger.Default.Register<DataBaseChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<DateRangeChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<SelectedAccountChangedMessage>(this);
        }
        public async void Receive(DataBaseChangedMessage message)
        {
            if (message.Type == DataBaseChangedMessageType.Init || message.Type == DataBaseChangedMessageType.Transactions)
            {
                var data = await _mediator.Send(new GetAllTransactionsQuery(_user.UserId));

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

            foreach(var t in trans)
            {
                DisplayedTransactions.Add(new DisplayedTransaction
                    (
                        t.Id,
                        t.Description,
                        t.Amount.ToString(),
                        t.Type.ToString(),
                        t.Date.ToString("dd.MM.yyyy"),
                        t.Account?.Name,
                        t.Category?.Name,
                        t.FamilyMember?.Name,
                        t.Debt?.Name
                    ));
            }
        }

        [RelayCommand]
        public void Load()
        {
            WeakReferenceMessenger.Default.Send(new DataBaseChangedMessage(DataBaseChangedMessageType.Transactions));
            //WeakReferenceMessenger.Default.Send(new CurrentTimeMessage(DateTime.Now));
        }
    }
}
