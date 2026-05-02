using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using CommunityToolkit.Maui.Views;
using DataAccess.RepositoriesImplementation;
using System.Collections.Generic;

namespace UI.Popups;

public class DateRangeDTO(DateTime StartDate,
    DateTime EndDate)
{
    public DateTime StartDate { get; } = StartDate;
    public DateTime EndDate { get; } = EndDate;
}
    

public partial class DateRangePopUp : Popup<DateRangeDTO?>
{

    public DateRangePopUp()
    {
        InitializeComponent();

        Loaded += OnLoad;
    }

    private void OnLoad(object sender, EventArgs e)
    {
        Clear();

        StartDatePicker.Date = DateTime.Now.AddMonths(-1);
        EndDatePicker.Date = DateTime.Now;
    }    

    private async void OnCancel(object sender, EventArgs e)
    {
        await CloseAsync(null);
    }

    private async void OnCreate(object sender, EventArgs e)
    {
        try
        {
            var startDate = StartDatePicker.Date;
            var endDate = EndDatePicker.Date;   

            if (startDate > endDate)
            {
                throw new ArgumentException($"Start Date is Later Then End Date");
            }


            await CloseAsync(new DateRangeDTO(startDate, endDate));
        }
        catch (FormatException ex)
        {
            await Application.Current.MainPage.DisplayAlert("Can't Select Date Range", $"{ex.Message}", "OK");
        }
        catch (ArgumentException ex)
        {
            await Application.Current.MainPage.DisplayAlert("Can't Select Date Range", $"{ex.Message}", "OK");
        }

    }

    private void Clear()
    {
        StartDatePicker.Date = DateTime.Now.AddMonths(-1);
        EndDatePicker.Date = DateTime.Now;
    }
}