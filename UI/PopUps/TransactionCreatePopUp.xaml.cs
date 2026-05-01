using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using CommunityToolkit.Maui.Views;

namespace UI.Popups;

public partial class TransactionCreatePopUp : Popup<Transaction?>
{
    readonly IAccountRepository _accountRepository;
    readonly ICategoryRepository _categoryRepository;
    readonly IFamilyMemberRepository _familyRepository;
    readonly IDebtRepository _debtRepository;

    readonly IUserContext _user;

    List<Account> Accounts = new();
    List<Category> Categories = new();
    List<FamilyMember> FamilyMembers = new();
    List<Debt> Debts = new();

    public TransactionCreatePopUp(
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

        Accounts = await _accountRepository.GetAllScalar(userId) ?? new();
        Categories = await _categoryRepository.GetAllScalar(userId) ?? new();
        FamilyMembers = await _familyRepository.GetAllScalar(userId) ?? new();
        Debts = await _debtRepository.GetAllScalar(userId) ?? new();

        if (Accounts.Count == 0)
        {
            throw new ArgumentException("No Accounts! Create an Account Before Transaction!");
        }

        TypePicker.ItemsSource = Enum.GetValues(typeof(TransactionType));
        TypePicker.SelectedIndexChanged += OnTypeChanged;
        TypePicker.SelectedItem = TransactionType.Expense;

        AccountPicker.ItemsSource = Accounts;
        AccountPicker.ItemDisplayBinding = new Binding("Name");
        if (Accounts.Any())
            AccountPicker.SelectedItem = Accounts.First();

        Categories.Insert(0, new Category { Id = Guid.Empty, Name = "-No Category-" });
        CategoryPicker.ItemsSource = Categories;
        CategoryPicker.ItemDisplayBinding = new Binding("Name");
        CategoryPicker.SelectedIndex = 0;


        FamilyMembers.Insert(0, new FamilyMember { Id = Guid.Empty, Name = "-No Member-" });
        FamilyMemberPicker.ItemsSource = FamilyMembers;
        FamilyMemberPicker.ItemDisplayBinding = new Binding("Name");
        FamilyMemberPicker.SelectedIndex = 0;

        DebtPicker.ItemDisplayBinding = new Binding("Name");

        TransactionDatePicker.Date = DateTime.Now;
    }

    private async void OnCancel(object sender, EventArgs e)
    {
        await CloseAsync(null);
    }

    private async void OnCreate(object sender, EventArgs e)
    {
        var amount = decimal.TryParse(AmountEntry.Text, out var a) ? a : 0;
        var description = DescriptionEntry.Text ?? "";
        var date = TransactionDatePicker.Date;

        var type = (TransactionType)TypePicker.SelectedItem;

        var account = AccountPicker.SelectedItem as Account;
        var category = CategoryPicker.SelectedItem as Category;
        var family = FamilyMemberPicker.SelectedItem as FamilyMember;
        var debt = DebtPicker.SelectedItem as Debt;

        if (account is null)
        {
            throw new Exception("No Account Provided");
        }

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Amount = amount,
            Description = description,
            Date = date,
            Type = type,

            AccountId = account.Id,

            CategoryId = category == null || category.Id == Guid.Empty
                ? null
                : category.Id,

            FamilyMemberId = family == null || family.Id == Guid.Empty
                ? null
                : family.Id,

            DebtId = debt == null || debt.Id == Guid.Empty
                ? null
                : debt.Id,

            UserId = _user.UserId
        };

        await CloseAsync(transaction);
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

    public void Clear()
    {
        AmountEntry.Text = string.Empty;
        DescriptionEntry.Text = string.Empty;
        TransactionDatePicker.Date = DateTime.Now;

        TypePicker.SelectedIndex = 0;

        if (AccountPicker.ItemsSource != null)
            AccountPicker.SelectedIndex = 0;

        if (CategoryPicker.ItemsSource != null)
            CategoryPicker.SelectedIndex = 0;

        if (FamilyMemberPicker.ItemsSource != null)
            FamilyMemberPicker.SelectedIndex = 0;
    }
}