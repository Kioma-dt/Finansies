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
    public enum GraphType { Dynamic, Review }

    public enum GraphGroupsType { Description, Account, Category, FamilyMember }

    public partial class StatisticsViewModel 
        : ObservableObject,
        //IRecipient<DataBaseChangedMessage>,
        IRecipient<DateRangeChangedMessage>
    {
        private readonly IMediator _mediator;
        private readonly IUserContext _userContext;
        private readonly IAnalyticsService _analyticsService;

        public ObservableCollection<GraphType> GraphTypes { get; } = new()
        {
            GraphType.Dynamic,
            GraphType.Review
        };

        [ObservableProperty]
        public partial GraphType SelectedGraphType { get; set; } = GraphType.Dynamic;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDynamicGraph))]
        public partial bool IsReviewGraph { get; set; } = false;

        public bool IsDynamicGraph => !IsReviewGraph;

        public ObservableCollection<GraphSumType> GraphSumTypes { get; } = new()
        {
            GraphSumType.Income,
            GraphSumType.Expense,
            GraphSumType.TotalSum,
            GraphSumType.TotalIncrease
        };

        [ObservableProperty]
        public partial GraphSumType SelectedGraphSumType { get; set; } = GraphSumType.Income;

        public ObservableCollection<GraphGroupsType> GraphGroupTypes { get; } = new()
        {
            GraphGroupsType.Description,
            GraphGroupsType.Account,
            GraphGroupsType.Category,
            GraphGroupsType.FamilyMember,
        };

        [ObservableProperty]
        public partial GraphGroupsType SelectedGraphGroupType { get; set; } = GraphGroupsType.Description;


        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsColumnGrpah))]
        public partial bool IsPieGraph { get; set; } = false;

        public bool IsColumnGrpah => !IsPieGraph;

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

            //WeakReferenceMessenger.Default.Register<DataBaseChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<DateRangeChangedMessage>(this);
        }
        //public async void Receive(DataBaseChangedMessage message)
        //{
        //    if (message.Type == DataBaseChangedMessageType.Transactions)
        //    {
        //        //await ShowGrpahCommand.ExecuteAsync(null);
        //    }
        //}
        public async void Receive(DateRangeChangedMessage message)
        {
            _startDate = message.StartDate;
            _endDate = message.EndDate;

            //await ShowGrpahCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        public async Task ChangeGrpahType()
        {
            if (SelectedGraphType == GraphType.Review)
            {
                IsReviewGraph = true;
            }
            else
            {
                IsReviewGraph = false;
            }
        }

        [RelayCommand]
        public async Task ShowDynamicGrpah()
        {
            var transactions = await _mediator.Send(new GetAllTransactionsQuery(_userContext.UserId));

            var chart = _analyticsService.BuildTransactionsDynamicChart(transactions,
                _startDate,
                _endDate);

            IsPieGraph = false;
            Series = chart.Series;
            XAxes = chart.XAxes;
        }

        [RelayCommand]
        public async Task ShowColumnGrpah()
        {
            var transactions = await _mediator.Send(new GetAllTransactionsQuery(_userContext.UserId));

            var groupSelector = SelectedGraphGroupType switch
            {
                GraphGroupsType.Description => (Func<Transaction, string>)(x => x.Description),
                GraphGroupsType.Account => x => x.Account?.Name ?? "No Account",
                GraphGroupsType.Category => x => x.Category?.Name ?? "No Category",
                GraphGroupsType.FamilyMember => x => x.FamilyMember?.Name ?? "No Family Member",
                _ => throw new NotImplementedException()
            };

            var chart = _analyticsService.BuildTransactionsColumnChart(transactions,
                _startDate,
                _endDate,
                groupSelector,
                SelectedGraphSumType);

            IsPieGraph = false;
            Series = chart.Series;
            XAxes = chart.XAxes;
        }

        [RelayCommand]
        public async Task ShowPieGrpah()
        {
            var transactions = await _mediator.Send(new GetAllTransactionsQuery(_userContext.UserId));

            var groupSelector = SelectedGraphGroupType switch
            {
                GraphGroupsType.Description => (Func<Transaction, string>)(x => x.Description),
                GraphGroupsType.Account => x => x.Account?.Name ?? "No Account",
                GraphGroupsType.Category => x => x.Category?.Name ?? "No Category",
                GraphGroupsType.FamilyMember => x => x.FamilyMember?.Name ?? "No Family Member",
                _ => throw new NotImplementedException()
            };

            var chart = _analyticsService.BuildTransactionsPieChart(transactions,
                _startDate,
                _endDate,
                groupSelector,
                SelectedGraphSumType);

            IsPieGraph = true;
            Series = chart.Series;
            XAxes = chart.XAxes;
        }
    }
}
