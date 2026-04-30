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
    readonly IUserContext _user;

    List<Account> Accounts = new();
    List<Category> Categories = new();
    List<FamilyMember> FamilyMembers = new();

    public TransactionCreatePopUp(
        IAccountRepository accountRepository,
        ICategoryRepository categoryRepository,
        IFamilyMemberRepository familyRepository,
        IUserContext user)
    {
        InitializeComponent();

        _accountRepository = accountRepository;
        _categoryRepository = categoryRepository;
        _familyRepository = familyRepository;
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

        if (Accounts.Count == 0)
        {
            throw new ArgumentException("No Accounts! Create an Account Before Transaction!");
        }

        TypePicker.ItemsSource = Enum.GetValues(typeof(TransactionType));
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

            UserId = _user.UserId
        };

        await CloseAsync(transaction);
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