using UI.ViewModels;

namespace UI.Views;

public partial class AccountView : ContentView
{
	public AccountView(AccountViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
        viewModel.LoadCommand.Execute(null);
    }

}