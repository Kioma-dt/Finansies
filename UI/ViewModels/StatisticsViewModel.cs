using BuisnessLogic.Entities;
using BuisnessLogic.UseCases.BudgetUseCasses.Commands;
using BuisnessLogic.UseCases.BudgetUseCasses.Queries;
using BuisnessLogic.UseCases.TransactionsUseCasses.Queries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.ObjectModel;
using UI.Messages;
using UI.Popups;
using UI.PopUps.Service;
using UI.Statistics;

namespace UI.ViewModels
{
    public partial class StatisticsViewModel 
        : ObservableObject,
        IRecipient<DataBaseChangedMessage>,
        IRecipient<DateRangeChangedMessage>
    {
        private readonly IMediator _mediator;
        private readonly IUserContext _userContext;
        private readonly IAnalyticsService _analyticsService;

        [ObservableProperty]
        public partial ISeries[] Series { get; set; } = [];

        [ObservableProperty]
        public partial Axis[] XAxes { get; set; } = [];


        private DateTime _startDate = DateTime.Now.AddMonths(-1);
        private DateTime _endDate = DateTime.Now;

        public StatisticsViewModel(
            IMediator mediator,
            IUserContext user,
            IAnalyticsService analyticsService)
        {
            _mediator = mediator;
            _userContext = user;
            _analyticsService = analyticsService;

            WeakReferenceMessenger.Default.Register<DataBaseChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<DateRangeChangedMessage>(this);
        }
        public async void Receive(DataBaseChangedMessage message)
        {
            if (message.Type == DataBaseChangedMessageType.Transactions)
            {
                await ShowGrpahCommand.ExecuteAsync(null);
            }
        }
        public async void Receive(DateRangeChangedMessage message)
        {
            _startDate = message.StartDate;
            _endDate = message.EndDate;

            await ShowGrpahCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        public async Task ShowGrpah()
        {
            var transactions = await _mediator.Send(new GetAllTransactionsQuery(_userContext.UserId));

            var chart = _analyticsService.BuildTransactionChart(transactions,
                _startDate,
                _endDate);

            Series = chart.Series;
            XAxes = chart.XAxes;
        }
    }
}
