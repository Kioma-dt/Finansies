using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using CommunityToolkit.Maui.Views;

namespace UI.Popups;

public partial class AccountCreatePopUp : Popup<Account?>
{
    readonly IAccountRepository _accountRepository;
    List<Account> Accounts { get; set; } = new();
    Guid UserId { get; set; }
    public AccountCreatePopUp(IAccountRepository accountRepository, IUserContext user)
    {
        InitializeComponent();

        _accountRepository = accountRepository;

        UserId = user.UserId;

        Loaded += OnLoad;
    }
    private async void OnLoad(object sender, EventArgs e)
    {
        this.Clear();

        Accounts = await _accountRepository.GetAllScalar(UserId) ?? new();

        Accounts.Insert(0, new Account() {Id=Guid.Empty, Name = "-No Parent-"});

        ParentPicker.ItemsSource = Accounts;
        ParentPicker.ItemDisplayBinding = new Binding("Name");
    }
    private async void OnCancel(object sender, EventArgs e)
    {
        await CloseAsync(null);
    }

    private async void OnCreate(object sender, EventArgs e)
    {
        var name = NameEntry.Text;
        var balance = decimal.TryParse(BalanceEntry.Text, out var b) ? b : 0;

        var parent = ParentPicker.SelectedItem as Account;


        await CloseAsync(new Account
        {
            Id = Guid.NewGuid(),
            Name = name,
            Balance = balance,
            ParentId = parent == null || parent.Id == Guid.Empty ? null : parent.Id
        });
    }

    public void Clear()
    {
        NameEntry.Text = string.Empty;
        BalanceEntry.Text = string.Empty;

        if (ParentPicker.ItemsSource != null)
            ParentPicker.SelectedIndex = 0;
    }
}