using UI.ViewModels;

namespace UI.Views;

public partial class TransactionsPage : ContentPage
{
	public TransactionsPage(TransactionsViewModel viewModel)
	{
        InitializeComponent();

        BindingContext = viewModel;

        //Loaded += async (_, _) =>
        //{
        //    await viewModel.LoadCommand.ExecuteAsync(null);
        //};
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is TransactionsViewModel vm)
        {
            await vm.LoadCommand.ExecuteAsync(null);
        }
    }
}