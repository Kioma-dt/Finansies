using BuisnessLogic.DTO;
using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using BuisnessLogic.UseCases.DebtsUseCasses.Commands;
using CommunityToolkit.Maui.Views;
using DataAccess.RepositoriesImplementation;
using UI.PopUps.Abstraction;
using UI.PopUps.ViewModels;

namespace UI.Popups;

public partial class DebtUpdatePopUp 
    : Popup<UpdateDebtCommand>,
    IPopUp<UpdateDebtCommand>
{
    public DebtUpdatePopUp(DebtUpdatePopUpModel viewModel)
    {

        InitializeComponent();

        BindingContext = viewModel;

        viewModel.CloseAction = async result =>
        {
            await CloseAsync(result);
        };
    }
}