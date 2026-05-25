using UI.ViewModels;

namespace UI.Views;

public partial class AuthentificationPage : ContentPage
{
	public AuthentificationPage(AuthentificationPageViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}