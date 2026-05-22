using BuisnessLogic.DTO;
using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.UseCases.AccountsUseCases.Commands;
using BuisnessLogic.UseCases.AccountsUseCases.Queries;
using BuisnessLogic.UseCases.CategoryUseCasses.Queries;
using BuisnessLogic.UseCases.DebtsUseCasses.Commands;
using BuisnessLogic.UseCases.FamilyMembersUseCasses.Queries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace UI.PopUps.ViewModels
{
    public partial class DebtCreatePopUpModel
        : ObservableObject
    {
        readonly IMediator _mediator;
        readonly IUserContext _userContext;

        public Action<CreateDebtCommand?>? CloseAction { get; set; }

        public DebtCreatePopUpModel(IMediator mediator,
            IUserContext userContext)
        {
            _mediator = mediator;
            _userContext = userContext;
        }

        [ObservableProperty]
        public partial string? Name { get; set; }

        [ObservableProperty]
        public partial string? Amount { get; set; }

        public ObservableCollection<DebtType> DebtTypes { get; } = new()
        {
            DebtType.Debit,
            DebtType.Credit
        };

        [ObservableProperty]
        public partial DebtType SelectedDebtType {  get; set; }

        [ObservableProperty]
        public partial DateTime StartDate {  get; set; }

        [ObservableProperty]
        public partial DateTime EndDate { get; set; }

        public ObservableCollection<Category> Categories { get; } = new();

        [ObservableProperty]
        public partial Category? SelectedCategory { get; set; }

        public ObservableCollection<FamilyMember> FamilyMembers { get; } = new();

        [ObservableProperty]
        public partial FamilyMember? SelectedFamilyMember { get; set; }

        public ObservableCollection<string> CapitalisationsPerYear = new()
        {
            "Monthly",
            "Yearly"
        };

        [ObservableProperty]
        public partial string? SelectedCapitalisationsPerYear { get; set; }


        public ObservableCollection<InterestType> InterestTypes = new() {
            InterestType.None,
            InterestType.Fixed,
            InterestType.Simple,
            InterestType.Complex
        };

        [ObservableProperty]
        public partial InterestType SelectedInterestType { get; set; }

        [ObservableProperty]
        public partial string? InterestRate { get; set; }

        [ObservableProperty]
        public partial string? FixedAddition { get; set; }

        [ObservableProperty]
        public partial bool IsAutoPlanned { get; set; }

        public ObservableCollection<TransactionPeriodicity> TransactionPeriodicities = new() {
            TransactionPeriodicity.Once,
            TransactionPeriodicity.Daily,
            TransactionPeriodicity.Monthly,
            TransactionPeriodicity.Yearly
        };

        [ObservableProperty]
        public partial TransactionPeriodicity SelectedTransactionPeriodicity { get; set; }

        [ObservableProperty]
        public partial bool IsInterestValueEnabled { get; set; } = false;

        [ObservableProperty]
        public partial string? InterestValuePlaceholder { get; set; }

        public async Task Initialize()
        {
            Name = String.Empty;
            
            Amount = String.Empty;

            SelectedDebtType = DebtType.Credit;

            StartDate = DateTime.Now;

            EndDate = DateTime.Now.AddYears(1);


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


            SelectedCapitalisationsPerYear = CapitalisationsPerYear.FirstOrDefault();

            SelectedInterestType = InterestType.None;

            IsInterestValueEnabled = false;

            InterestValuePlaceholder = "No Interest";

            InterestRate = String.Empty;

            FixedAddition = String.Empty;

            IsAutoPlanned = false;

            SelectedTransactionPeriodicity = TransactionPeriodicities.FirstOrDefault();
        }

        [RelayCommand]
        public async Task UpdateInterestField()
        {
            var type = SelectedInterestType;

            switch (type)
            {
                case InterestType.None:
                    IsInterestValueEnabled = false;
                    InterestRate = String.Empty;
                    InterestValuePlaceholder = "No Interest";
                    break;

                case InterestType.Simple:
                case InterestType.Complex:
                    IsInterestValueEnabled = true;
                    InterestRate = String.Empty;
                    InterestValuePlaceholder = "Interest %";
                    break;

                case InterestType.Fixed:
                    IsInterestValueEnabled = true;
                    InterestRate = String.Empty;
                    InterestValuePlaceholder = "Fixed amount";
                    break;
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
            var name = Name;

            if (name is null)
            {
                throw new ArgumentException("Enter Name!");
            }

            var amount = decimal.Parse(Amount ?? "");
            var interestValue = decimal.Parse(InterestRate ?? "");

            var interestType = SelectedInterestType;

            var debtType = SelectedDebtType;

            var startDate = StartDate;

            var endDate = EndDate;

            if (startDate > endDate)
            {
                throw new ArgumentException("Wring Dates!");
            }

            var capitalisations = SelectedCapitalisationsPerYear == CapitalisationsPerYear[1] ? 1 : 12;

            var category = SelectedCategory;
            var family = SelectedFamilyMember;

            var autoCalculate = IsAutoPlanned;

            TransactionPeriodicity payments = SelectedTransactionPeriodicity;

            var comand = new CreateDebtCommand
                (
                    _userContext.UserId,
                    name,
                    amount,
                    debtType,
                    startDate,
                    endDate,
                    category?.Id == Guid.Empty ? null : category?.Id,
                    family?.Id == Guid.Empty ? null : family?.Id,
                    capitalisations,
                    interestType,
                    interestValue,
                    interestValue,
                    autoCalculate,
                    payments
                );

            CloseAction?.Invoke(comand);
        }
    }
}
