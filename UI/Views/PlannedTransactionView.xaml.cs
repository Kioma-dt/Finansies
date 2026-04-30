using UI.ViewModels;

namespace UI.Views;

public partial class PlannedTransactionView : ContentView
{
	public PlannedTransactionView(PlannedTransactionsViewModel viewModel)
	{
        InitializeComponent();

        BindingContext = viewModel;
    }

    //public async Task LoadContent()
    //{
    //    if (BindingContext is TransactionsViewModel vm && !vm.IsLoaded)
    //    {
    //        vm.IsLoaded = true;
    //        await vm.LoadCommand.ExecuteAsync(null);
    //    }
    //}
}