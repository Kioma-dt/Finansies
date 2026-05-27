using BuisnessLogic.UseCases.CategoryUseCasses.Commands;
using BuisnessLogic.UseCases.CategoryUseCasses.Queries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Popups;

namespace UI.PopUps.ViewModels
{
    public partial class DateRangePopUpModel
        : ObservableObject
    {
        public Action<DateRangeDTO?>? CloseAction { get; set; }

        public DateRangePopUpModel() { }

        [ObservableProperty]
        public partial DateTime StartDate { get; set; }

        [ObservableProperty]
        public partial DateTime EndDate { get; set; }

        public async Task Initialize()
        {
            StartDate = DateTime.Now;
            EndDate = DateTime.Now.AddMonths(1);
        }

        [RelayCommand]
        public async Task Cancel()
        {
            CloseAction?.Invoke(null);
        }

        [RelayCommand]
        public async Task Create()
        {
            try
            {
                var startDate = StartDate;

                var endDate = EndDate;

                if (startDate > endDate)
                {
                    throw new ArgumentException("Start Date Should Be Less Then End Date!");
                }

                CloseAction?.Invoke(new DateRangeDTO(startDate, endDate));
            }
            catch (ArgumentException ex)
            {
                await Shell.Current.DisplayAlert(
                       "Can't Select This Range",
                       ex.Message,
                       "OK");
            }
        }
    }
}
