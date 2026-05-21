using BuisnessLogic.DTO;
using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using CommunityToolkit.Maui.Views;
using UI.PopUps.Abstraction;

namespace UI.Popups;

public partial class FamilyMemberCreatePopUp
    : Popup<FamilyMemberCreateDTO?>,
    IPopUp<FamilyMemberCreateDTO>
{
    public FamilyMemberCreatePopUp()
    {
        InitializeComponent();
        Loaded += OnLoad;
    }

    private void OnLoad(object sender, EventArgs e)
    {
        Clear();
    }

    private async void OnCancel(object sender, EventArgs e)
    {
        await CloseAsync(null);
    }

    private async void OnCreate(object sender, EventArgs e)
    {
        try
        {
            var name = NameEntry.Text ?? "";

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty");

            await CloseAsync(new FamilyMemberCreateDTO
            {
                Name = name
            });
        }
        catch (ArgumentException ex)
        {
            await Application.Current.MainPage
                .DisplayAlert("Can't create Family Member", ex.Message, "OK");
        }
    }

    private void Clear()
    {
        NameEntry.Text = string.Empty;
    }
}