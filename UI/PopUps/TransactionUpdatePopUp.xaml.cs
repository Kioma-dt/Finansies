using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using BuisnessLogic.UseCases.TransactionsUseCasses.Commands;
using CommunityToolkit.Maui.Views;
using DataAccess.RepositoriesImplementation;
using UI.PopUps.Abstraction;
using UI.PopUps.ViewModels;

namespace UI.Popups;

public partial class TransactionUpdatePopUp 
    : Popup<UpdateTransactionCommand?>,
    IPopUp<UpdateTransactionCommand>
{
    public TransactionUpdatePopUp(TransactionUpdatePopUpModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;

        viewModel.CloseAction = async result =>
        {
            await CloseAsync(result);
        };
    }
}