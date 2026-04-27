using UI.ViewModels;

namespace UI.Views;

public partial class MainPage : ContentPage
{
	public MainPage(MainPageViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MainPageViewModel vm)
        {
            await vm.LoadCommand.ExecuteAsync(null);
        }
    }
}