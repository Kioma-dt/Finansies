using UI.ViewModels;

namespace UI.Views;

public partial class AccountView : ContentView
{
	public AccountView(AccountViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;

		//viewModel.LoadCommand.ExecuteAsync(null);
	}

    public async Task LoadContent()
    {
        if (BindingContext is AccountViewModel vm && !vm.IsLoaded)
        {
            vm.IsLoaded = true;
            await vm.LoadCommand.ExecuteAsync(null);
        }
    }
}