using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using BuisnessLogic.UseCases.AccountsUseCases.Commands;
using BuisnessLogic.UseCases.TransfersUseCasses.Commands;
using CommunityToolkit.Maui.Views;
using UI.PopUps.Abstraction;
using UI.PopUps.ViewModels;

namespace UI.Popups;

public partial class TransferCreatePopUp 
    : Popup<CreateTransferCommand?>,
    IPopUp<CreateTransferCommand>
{
    public TransferCreatePopUp(TransferCreatePopUpModel viewModel)
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