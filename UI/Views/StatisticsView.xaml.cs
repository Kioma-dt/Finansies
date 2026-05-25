using UI.ViewModels;

namespace UI.Views;

public partial class StatisticsView : ContentView
{
	public StatisticsView(StatisticsViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}