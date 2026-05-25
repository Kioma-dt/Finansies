using BuisnessLogic.DTO;
using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using BuisnessLogic.UseCases.FamilyMembersUseCasses.Commands;
using CommunityToolkit.Maui.Views;
using UI.PopUps.Abstraction;
using UI.PopUps.ViewModels;

namespace UI.Popups;

public partial class FamilyMemberCreatePopUp
    : Popup<CreateFamilyMemberCommand?>,
    IPopUp<CreateFamilyMemberCommand>
{
    public FamilyMemberCreatePopUp(FamilyMemberCreatePopUpModel viewModel)
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

    //private void OnLoad(object sender, EventArgs e)
    //{
    //    Clear();
    //}

    //private async void OnCancel(object sender, EventArgs e)
    //{
    //    await CloseAsync(null);
    //}

    //private async void OnCreate(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        var name = NameEntry.Text ?? "";

    //        if (string.IsNullOrWhiteSpace(name))
    //            throw new ArgumentException("Name cannot be empty");

    //        await CloseAsync(new FamilyMemberCreateDTO
    //        {
    //            Name = name
    //        });
    //    }
    //    catch (ArgumentException ex)
    //    {
    //        await Application.Current.MainPage
    //            .DisplayAlert("Can't create Family Member", ex.Message, "OK");
    //    }
    //}

    //private void Clear()
    //{
    //    NameEntry.Text = string.Empty;
    //}
}