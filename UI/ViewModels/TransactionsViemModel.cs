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
    public class DisplayedTransaction
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public string? Amount { get; set; }
        public string? Type { get; set; }
        public string? Date {  get; set; }
        public string? AccountName { get; set; }
        public string? CategoryName { get; set; }
        public string? FamilyMemberName { get; set; }
        public string? DebtName { get; set; }
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
                DisplayedTransactions.Add(new DisplayedTransaction()
                {
                    Id = t.Id,
                    Description = t.Description,
                    Amount = t.Amount.ToString(),
                    Type = t.Type.ToString(),
                    Date = t.Date.ToString("dd.MM.yyyy"),
                    AccountName = t.Account?.Name,
                    CategoryName = t.Category?.Name,
                    FamilyMemberName = t.FamilyMember?.Name,
                    DebtName = t.Debt?.Name,
                });
            }
        }
    }
}
