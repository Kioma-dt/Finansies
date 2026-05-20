using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using BuisnessLogic.UseCases.AccountsUseCases.Commands;
using CommunityToolkit.Maui.Views;

using UI.PopUps.ViewModels;

namespace UI.Popups;

public partial class AccountCreatePopUp : Popup<CreateAccountCommand?>
{
    //readonly IAccountRepository _accountRepository;
    //List<Account> Accounts { get; set; } = new();
    //Guid UserId { get; set; }
    public AccountCreatePopUp(AccountCreatePopUpModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;

        viewModel.CloseAction = async result =>
        {
            await CloseAsync(result);
        };

        Loaded += async (_, _) =>
        {
            await viewModel.Initialize();
        };
    }
    //private async void OnLoad(object sender, EventArgs e)
    //{
    //    this.Clear();

    //    Accounts = (await _accountRepository.GetAll(UserId)).ToList();

    //    Accounts.Insert(0, new Account() {Id=Guid.Empty, Name = "-No Parent-"});
    //    ParentPicker.ItemsSource = Accounts;
    //    ParentPicker.ItemDisplayBinding = new Binding("Name");
    //    ParentPicker.SelectedIndex = 0;
    //}
    //private async void OnCancel(object sender, EventArgs e)
    //{
    //    await CloseAsync(null);
    //}

    //private async void OnCreate(object sender, EventArgs e)
    //{
    //    var name = NameEntry.Text;
    //    var balance = decimal.TryParse(BalanceEntry.Text, out var b) ? b : 0;

    //    var parent = ParentPicker.SelectedItem as Account;


    //    await CloseAsync(new Account
    //    {
    //        Id = Guid.NewGuid(),
    //        Name = name,
    //        Balance = balance,
    //        ParentId = parent == null || parent.Id == Guid.Empty ? null : parent.Id
    //    });
    //}

    //public void Clear()
    //{
    //    NameEntry.Text = string.Empty;
    //    BalanceEntry.Text = string.Empty;

    //    if (ParentPicker.ItemsSource != null)
    //        ParentPicker.SelectedIndex = 0;
    //}
}