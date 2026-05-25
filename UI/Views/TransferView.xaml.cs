using UI.ViewModels;

namespace UI.Views;

public partial class TransferView : ContentView
{
	public TransferView(TransfersViewModel viewModel)
	{
        InitializeComponent();

        BindingContext = viewModel;
        viewModel.LoadCommand.Execute(null);
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