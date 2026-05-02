using UI.ViewModels;

namespace UI.Views;

public partial class DateRangeSelectorView : ContentView
{
	public DateRangeSelectorView(DateRangeSelectorViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}