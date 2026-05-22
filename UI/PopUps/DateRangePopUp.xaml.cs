using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using CommunityToolkit.Maui.Views;
using DataAccess.RepositoriesImplementation;
using System.Collections.Generic;
using UI.PopUps.Abstraction;
using UI.PopUps.ViewModels;

namespace UI.Popups;

public class DateRangeDTO(DateTime StartDate,
    DateTime EndDate)
{
    public DateTime StartDate { get; } = StartDate;
    public DateTime EndDate { get; } = EndDate;
}
    

public partial class DateRangePopUp
    : Popup<DateRangeDTO?>,
    IPopUp<DateRangeDTO>
{

    public DateRangePopUp(DateRangePopUpModel viewModel)
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