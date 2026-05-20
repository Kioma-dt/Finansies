using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using CommunityToolkit.Maui.Views;
using DataAccess.RepositoriesImplementation;

namespace UI.Popups;


public class PlannedTransactionCreateDTO(decimal Amount,
    string Description,
    TransactionType Type,
    Guid? AccountId,
    Guid? CategoryId,
    Guid? FamilyMemberId,
    Guid? DebtId,
    DateTime StartDate,
    TransactionPeriodicity Periodicity,
    uint Count)
{
    public decimal Amount { get; } = Amount;
    public string Description { get; } = Description;
    public TransactionType Type { get; } = Type;
    public Guid? AccountId { get; } = AccountId;
    public Guid? CategoryId { get; } = CategoryId;
    public Guid? FamilyMemberId { get; } = FamilyMemberId;
    public Guid? DebtId { get; } = DebtId;
    public DateTime StartDate { get; } = StartDate;
    public TransactionPeriodicity Periodicity { get; } = Periodicity;
    public uint Count { get; } = Count;
}

public partial class PlannedTransactionCreatePopUp : Popup<PlannedTransactionCreateDTO?>
{
    readonly IAccountRepository _accountRepository;
    readonly ICategoryRepository _categoryRepository;
    readonly IFamilyMemberRepository _familyRepository;
    readonly IDebtRepository _debtRepository;
    readonly IUserContext _user;

    List<Account> Accounts = new();
    List<Category> Categories = new();
    List<FamilyMember> Families = new();
    List<Debt> Debts = new();

    public PlannedTransactionCreatePopUp(
        IAccountRepository accountRepository,
        ICategoryRepository categoryRepository,
        IFamilyMemberRepository familyRepository,
        IDebtRepository debtRepository,
        IUserContext user)
    {
        InitializeComponent();

        _accountRepository = accountRepository;
        _categoryRepository = categoryRepository;
        _familyRepository = familyRepository;
        _debtRepository = debtRepository;
        _user = user;

        Loaded += OnLoad;
    }

    private async void OnLoad(object sender, EventArgs e)
    {
        Clear();

        var userId = _user.UserId;

        Accounts = (await _accountRepository.GetAll(userId)).ToList();
        Categories = (await _categoryRepository.GetAll(_user.UserId)).ToList();
        Families = await _familyRepository.GetAllScalar(userId) ?? new();
        Debts = await _debtRepository.GetAllScalar(userId) ?? new();

        TypePicker.ItemsSource = Enum.GetValues(typeof(TransactionType));
        TypePicker.SelectedIndexChanged += OnTypeChanged;
        TypePicker.SelectedItem = TransactionType.Expense;

        PeriodPicker.ItemsSource = Enum.GetValues(typeof(TransactionPeriodicity));
        PeriodPicker.SelectedItem = TransactionPeriodicity.Once;

        AccountPicker.ItemsSource = Accounts;
        AccountPicker.ItemDisplayBinding = new Binding("Name");
        AccountPicker.SelectedIndex = 0;

        Categories.Insert(0, new Category { Id = Guid.Empty, Name = "-No Category-" });
        CategoryPicker.ItemsSource = Categories;
        CategoryPicker.ItemDisplayBinding = new Binding("Name");
        CategoryPicker.SelectedIndex = 0;

        Families.Insert(0, new FamilyMember { Id = Guid.Empty, Name = "-No Member-" });
        FamilyPicker.ItemsSource = Families;
        FamilyPicker.ItemDisplayBinding = new Binding("Name");
        FamilyPicker.SelectedIndex = 0;

        DebtPicker.ItemDisplayBinding = new Binding("Name");

        StartDatePicker.Date = DateTime.Now;
        CountEntry.Text = "1";
        CountEntry.IsEnabled = false;
    }

    private void OnPeriodChanged(object sender, EventArgs e)
    {
        var period = (TransactionPeriodicity)PeriodPicker.SelectedItem;

        if (period == TransactionPeriodicity.Once)
        {
            CountEntry.Text = "1";
            CountEntry.IsEnabled = false;
        }
        else
        {
            CountEntry.IsEnabled = true;
        }
    }

    private async void OnCancel(object sender, EventArgs e)
    {
        await CloseAsync(null);
    }

    private async void OnCreate(object sender, EventArgs e)
    {
        var amount = decimal.Parse(AmountEntry.Text);
        var description = DescriptionEntry.Text ?? "";

        var type = (TransactionType)TypePicker.SelectedItem;
        var period = (TransactionPeriodicity)PeriodPicker.SelectedItem;

        TimeSpan timeSpan = period switch
        {
            TransactionPeriodicity.Daily => TimeSpan.FromDays(1),

            TransactionPeriodicity.Monthly => TimeSpan.FromDays(30),
            TransactionPeriodicity.Yearly => TimeSpan.FromDays(365),

            TransactionPeriodicity.Once => TimeSpan.Zero,

            _ => throw new ArgumentOutOfRangeException(nameof(period), period, null)
        };

        var count = uint.Parse(CountEntry.Text);
        if (period == TransactionPeriodicity.Once)
            count = 1;

        var account = AccountPicker.SelectedItem as Account;
        var category = CategoryPicker.SelectedItem as Category;
        var family = FamilyPicker.SelectedItem as FamilyMember;
        var debt = DebtPicker.SelectedItem as Debt;

        var planned = new PlannedTransactionCreateDTO
        (
            Amount: amount,
            Description: description,
            Type: type,

            StartDate: StartDatePicker.Date,
            Periodicity: period,
            Count: count,

            AccountId: account!.Id,
            CategoryId: category?.Id == Guid.Empty ? null : category?.Id,
            FamilyMemberId: family?.Id == Guid.Empty ? null : family?.Id,
            DebtId: debt?.Id == Guid.Empty ? null : debt?.Id
        );

        await CloseAsync(planned);
    }

    private void OnTypeChanged(object sender, EventArgs e)
    {
        var type = (TransactionType)TypePicker.SelectedItem;

        IEnumerable<Debt> filtered = Enumerable.Empty<Debt>();

        if (type == TransactionType.Expense)
        {
            filtered = Debts.Where(d => d.Type == DebtType.Credit);
        }
        else if (type == TransactionType.Income)
        {
            filtered = Debts.Where(d => d.Type == DebtType.Debit);
        }

        var list = filtered.ToList();

        list.Insert(0, new Debt { Id = Guid.Empty, Name = "-No Debt-" });

        DebtPicker.ItemsSource = list;
        DebtPicker.SelectedIndex = 0;
    }

    private void Clear()
    {
        AmountEntry.Text = "";
        DescriptionEntry.Text = "";

        StartDatePicker.Date = DateTime.Now;

        CountEntry.Text = "1";
        CountEntry.IsEnabled = false;
    }
}