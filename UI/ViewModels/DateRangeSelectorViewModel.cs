using BuisnessLogic.DTO;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using UI.Popups;
using UI.Messages;
using UI.PopUps.Service;

namespace UI.ViewModels
{

    public enum DateRangeMode
    {
        Day,
        Month,
        Year,
        Custom
    }
    public partial class DateRangeSelectorViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanNavigate))]
        public partial DateRangeMode Mode { get; set; } = DateRangeMode.Month;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RangeText))]
        public partial DateTime StartDate { get; set; } = DateTime.Now.AddMonths(-1);

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RangeText))]
        public partial DateTime EndDate { get; set; } = DateTime.Now;

        public string RangeText => $"{StartDate:dd.MM.yyyy} - {EndDate:dd.MM.yyyy}";

        public bool CanNavigate => Mode != DateRangeMode.Custom;

        private readonly IPopUpService _popupService;

        public DateRangeSelectorViewModel(IPopUpService popup)
        {
            ResetByMode();
            _popupService = popup;
        }

        private void ResetByMode()
        {
            var prevSartDate = StartDate;
            var prevEndDate = EndDate;

            switch (Mode)
            {
                case DateRangeMode.Day:
                    StartDate = prevSartDate;
                    EndDate = prevSartDate;
                    break;

                case DateRangeMode.Month:
                    StartDate = prevSartDate;
                    EndDate = prevSartDate.AddMonths(1);
                    break;

                case DateRangeMode.Year:
                    StartDate = prevSartDate;
                    EndDate = prevSartDate.AddYears(1);
                    break;
            }

            WeakReferenceMessenger.Default.Send(new DateRangeChangedMessage(StartDate, EndDate));
        }

        [RelayCommand]
        private void Next()
        {
            switch (Mode)
            {
                case DateRangeMode.Day:
                    StartDate = StartDate.AddDays(1);
                    EndDate = StartDate;
                    break;

                case DateRangeMode.Month:
                    StartDate = StartDate.AddMonths(1);
                    EndDate = StartDate.AddMonths(1).AddDays(-1);
                    break;

                case DateRangeMode.Year:
                    StartDate = StartDate.AddYears(1);
                    EndDate = StartDate.AddYears(1).AddDays(-1);
                    break;
            }

            WeakReferenceMessenger.Default.Send(new DateRangeChangedMessage(StartDate, EndDate));
        }

        [RelayCommand]
        private void Prev()
        {
            switch (Mode)
            {
                case DateRangeMode.Day:
                    StartDate = StartDate.AddDays(-1);
                    EndDate = StartDate;
                    break;

                case DateRangeMode.Month:
                    StartDate = StartDate.AddMonths(-1);
                    EndDate = StartDate.AddMonths(1);
                    break;

                case DateRangeMode.Year:
                    StartDate = StartDate.AddYears(-1);
                    EndDate = StartDate.AddYears(1);
                    break;
            }

            WeakReferenceMessenger.Default.Send(new DateRangeChangedMessage(StartDate, EndDate));
        }

        [RelayCommand]
        private async Task ChangeMode()
        {
            var action = await Application.Current.MainPage.DisplayActionSheet(
                "Select mode",
                "Cancel",
                null,
                "Day",
                "Month",
                "Year",
                "Custom");

            if (action == "Cancel" || action == null)
                return;

            Mode = action switch
            {
                "Day" => DateRangeMode.Day,
                "Month" => DateRangeMode.Month,
                "Year" => DateRangeMode.Year,
                _ => Mode
            };

            if (action == "Custom")
            {
                var data = await _popupService.ShowPopUp<DateRangeDTO?, DateRangePopUp>();

                if (data is null)
                    return;

                Mode = DateRangeMode.Custom;
                StartDate = data.StartDate;
                EndDate = data.EndDate;
            }
                

            this.ResetByMode();
        }

    }
}
