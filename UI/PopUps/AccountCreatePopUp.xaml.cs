using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using BuisnessLogic.UseCases.AccountsUseCases.Commands;
using CommunityToolkit.Maui.Views;
using UI.PopUps.Abstraction;
using UI.PopUps.ViewModels;

namespace UI.Popups;

public partial class AccountCreatePopUp 
    : Popup<CreateAccountCommand?>,
    IPopUp<CreateAccountCommand>
{
    public AccountCreatePopUp(AccountCreatePopUpModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;

        viewModel.CloseAction = async result =>
        {
            await CloseAsync(result);
        };

        Loaded += async (_, _) =>
        {
            await viewModel.Initialize();
        };
    }
}