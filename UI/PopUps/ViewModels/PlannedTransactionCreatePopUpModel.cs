using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.UseCases.AccountsUseCases.Queries;
using BuisnessLogic.UseCases.CategoryUseCasses.Queries;
using BuisnessLogic.UseCases.DebtsUseCasses.Commands;
using BuisnessLogic.UseCases.DebtsUseCasses.Queries;
using BuisnessLogic.UseCases.FamilyMembersUseCasses.Queries;
using BuisnessLogic.UseCases.PlannedTransactionsUsesCasses.Commands;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace UI.PopUps.ViewModels
{
    public partial class PlannedTransactionCreatePopUpModel
        : ObservableObject
    {
        readonly IMediator _mediator;
        readonly IUserContext _userContext;

        public Action<CreatePlannedTransactionCommand?>? CloseAction { get; set; }

        public PlannedTransactionCreatePopUpModel(IMediator mediator,
            IUserContext userContext)
        {
            _mediator = mediator;
            _userContext = userContext;
        }

        [ObservableProperty]
        public partial string? Amount { get; set; }

        [ObservableProperty]
        public partial string? Description { get; set; }

        public ObservableCollection<TransactionType> TransactionTypes { get; } = new()
        {
            TransactionType.Income,
            TransactionType.Expense
        };

        [ObservableProperty]
        public partial TransactionType SelectedTransactionType { get; set; }

        [ObservableProperty]
        public partial DateTime StartDate { get; set; }

        public ObservableCollection<Account> Accounts { get; } = new();

        [ObservableProperty]
        public partial Account? SelectedAccount {  get; set; }

        public ObservableCollection<Category> Categories { get; } = new();

        [ObservableProperty]
        public partial Category? SelectedCategory { get; set; }

        public ObservableCollection<FamilyMember> FamilyMembers { get; } = new();

        [ObservableProperty]
        public partial FamilyMember? SelectedFamilyMember { get; set; }

        private List<Debt> _debts = new();
        public ObservableCollection<Debt> FilteredDebts { get; } = new();

        [ObservableProperty]
        public partial Debt? SelectedDebt {  get; set; }

        [ObservableProperty]
        public partial string? NumberOfTransactions { get; set; }

        [ObservableProperty]
        public partial bool IsNumberEnabled {  get; set; }


        public ObservableCollection<TransactionPeriodicity> TransactionPeriodicities = new() {
            TransactionPeriodicity.Once,
            TransactionPeriodicity.Daily,
            TransactionPeriodicity.Monthly,
            TransactionPeriodicity.Yearly
        };

        [ObservableProperty]
        public partial TransactionPeriodicity SelectedTransactionPeriodicity { get; set; }


        public async Task Initialize()
        {
            Amount = String.Empty;

            Description = String.Empty;

            StartDate = DateTime.Now;

            SelectedTransactionType = TransactionType.Income;

            Accounts.Clear();

            Accounts.Insert(0, new Account() { Id = Guid.Empty, Name = "-No Account-" });

            var accounts = await _mediator.Send(new GetAllAccountsQuery(_userContext.UserId));

            foreach (var account in accounts)
            {
                Accounts.Add(account);
            }

            SelectedAccount = Accounts.FirstOrDefault();

            Categories.Clear();

            Categories.Insert(0, new Category() { Id = Guid.Empty, Name = "-No Category-" });

            var categories = await _mediator.Send(new GetAllCategoriesQuery(_userContext.UserId));

            foreach (var category in categories)
            {
                Categories.Add(category);
            }

            SelectedCategory = Categories.FirstOrDefault();


            FamilyMembers.Clear();

            FamilyMembers.Insert(0, new FamilyMember() { Id = Guid.Empty, Name = "-No Family Member-" });

            var familyMembers = await _mediator.Send(new GetAllFamilyMembersQuery(_userContext.UserId));

            foreach (var familyMember in familyMembers)
            {
                FamilyMembers.Add(familyMember);
            }

            SelectedFamilyMember = FamilyMembers.FirstOrDefault();


            FilteredDebts.Clear();

            FilteredDebts.Insert(0, new Debt() { Id = Guid.Empty, Name = "-No Debt-" });

            _debts = (await _mediator.Send(new GetAllDebtsQuery(_userContext.UserId))).ToList();
            var filteredDebts = _debts.Where(x => x.Type == DebtType.Debit);

            foreach (var debt in filteredDebts)
            {
                FilteredDebts.Add(debt);
            }

            SelectedDebt = FilteredDebts.FirstOrDefault();

            NumberOfTransactions = "1";

            SelectedTransactionPeriodicity = TransactionPeriodicity.Once;

            IsNumberEnabled = false;
        }

        [RelayCommand]
        public async Task UpdateFilteredDebts()
        {
            FilteredDebts.Clear();

            FilteredDebts.Insert(0, new Debt() { Id = Guid.Empty, Name = "-No Debt-" });

            var filteredDebts = new List<Debt>();
            if (SelectedTransactionType == TransactionType.Income)
            {
                filteredDebts = _debts.Where(x => x.Type == DebtType.Debit).ToList();
            }
            else
            {
                filteredDebts = _debts.Where(x => x.Type == DebtType.Credit).ToList();
            }

            foreach (var debt in filteredDebts)
            {
                FilteredDebts.Add(debt);
            }

            SelectedDebt = FilteredDebts.FirstOrDefault();
        }

        [RelayCommand]
        public async Task UpdateNumberOfTransactions()
        {
            if (SelectedTransactionPeriodicity == TransactionPeriodicity.Once)
            {
                NumberOfTransactions = "1";

                IsNumberEnabled = false;
            }
            else
            {
                NumberOfTransactions = String.Empty;

                IsNumberEnabled = true;
            }
        }

        [RelayCommand]
        public async Task Cancel()
        {
            CloseAction?.Invoke(null);
        }

        [RelayCommand]
        public async Task Create()
        {
            var description = Description;

            if (description is null)
            {
                throw new ArgumentException("Enter Description!");
            }

            var amount = decimal.Parse(Amount ?? "");

            var transactionType = SelectedTransactionType;

            var startDate = StartDate;

            var account = SelectedAccount;
            var category = SelectedCategory;
            var family = SelectedFamilyMember;
            var debt = SelectedDebt;

            var numberOfTransactions = UInt32.Parse(NumberOfTransactions ??  String.Empty);

            TransactionPeriodicity payments = SelectedTransactionPeriodicity;

            var comand = new CreatePlannedTransactionCommand
                (
                    _userContext.UserId,
                    amount,
                    description,
                    transactionType,
                    startDate,
                    account?.Id == Guid.Empty ? null : account?.Id,
                    category?.Id == Guid.Empty ? null : category?.Id,
                    family?.Id == Guid.Empty ? null : family?.Id,
                    debt?.Id == Guid.Empty ? null : debt?.Id,
                    numberOfTransactions,
                    payments
                );

            CloseAction?.Invoke(comand);
        }
    }
}
