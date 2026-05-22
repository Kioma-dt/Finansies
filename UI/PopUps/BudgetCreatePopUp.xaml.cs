using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using BuisnessLogic.UseCases.AccountsUseCases.Commands;
using BuisnessLogic.UseCases.BudgetUseCasses.Commands;
using CommunityToolkit.Maui.Views;
using DataAccess.RepositoriesImplementation;
using System.Collections.Generic;

using UI.PopUps.ViewModels;
using UI.PopUps.Abstraction;

namespace UI.Popups;


public partial class BudgetCreatePopUp
    : Popup<CreateBudgetCommand?>,
    IPopUp<CreateBudgetCommand>
{

    public BudgetCreatePopUp(BudgetCreatePopUpModel viewModel)
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