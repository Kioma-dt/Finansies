using UI.ViewModels;

namespace UI.Views;

public partial class AuthentificationPage : ContentPage
{
    private readonly AuthentificationPageViewModel _viewModel;
    public AuthentificationPage(AuthentificationPageViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;

		_viewModel = viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.Name = string.Empty;
        _viewModel.Password = string.Empty;
    }
}