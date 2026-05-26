using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using BuisnessLogic.UseCases.PlannedTransactionsUsesCasses.Commands;
using CommunityToolkit.Maui.Views;
using DataAccess.RepositoriesImplementation;
using UI.PopUps.Abstraction;

using UI.PopUps.ViewModels;

namespace UI.Popups;



public partial class PlannedTransactionUpdatePopUp 
    : Popup<UpdatePlannedTransactionCommand?>,
    IPopUp<UpdatePlannedTransactionCommand>
{

    public PlannedTransactionUpdatePopUp(PlannedTransactionUpdatePopUpModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;

        viewModel.CloseAction = async result =>
        {
            await CloseAsync(result);
        };
    }
}