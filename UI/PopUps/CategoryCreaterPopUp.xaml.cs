using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using BuisnessLogic.UseCases.CategoryUseCasses.Commands;
using CommunityToolkit.Maui.Views;
using UI.PopUps.Abstraction;
using UI.PopUps.ViewModels;

namespace UI.Popups;

public partial class CategoryCreatePopUp
    : Popup<CreateCategoryCommand?>,
    IPopUp<CreateCategoryCommand>
{
    public CategoryCreatePopUp(CategoryCreatePopUpModel viewModel)
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