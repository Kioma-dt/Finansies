using UI.ViewModels;

namespace UI.Views;

public partial class DebtView : ContentView
{
	public DebtView(DebtViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
        viewModel.LoadCommand.Execute(null);
    }
}