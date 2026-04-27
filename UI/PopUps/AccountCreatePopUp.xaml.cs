using CommunityToolkit.Maui.Views;
using BuisnessLogic.Entities;

namespace UI.Popups;

public partial class AddAccountPopup : Popup<Account?>
{
    public AddAccountPopup()
    {
        InitializeComponent();
    }

    private async void OnCancel(object sender, EventArgs e)
    {
        await CloseAsync(null);
    }

    private async void OnCreate(object sender, EventArgs e)
    {
        var name = NameEntry.Text;
        var balance = decimal.TryParse(BalanceEntry.Text, out var b) ? b : 0;

        await CloseAsync(new Account
        {
            Name = name,
            Balance = balance
        });
    }
}