using BuisnessLogic.Entities;
using BuisnessLogic.UseCases.TransactionsUseCasses.Queries;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using MediatR;
using UI.Messages;
using UI.Statistics;
using UI.ViewModels;

namespace UI.Tests.ViewModelsTests
{
    public class StatisticsViewModelTests
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<IUserContext> _userContext = new();
        private readonly Mock<IAnalyticsService> _analyticsService = new();

        private readonly Guid _userId = Guid.NewGuid();

        public StatisticsViewModelTests()
        {
            _userContext.Setup(x => x.UserId)
                .Returns(_userId);
        }

        private StatisticsViewModel CreateVm()
        {
            return new StatisticsViewModel(
                _mediator.Object,
                _userContext.Object,
                _analyticsService.Object);
        }

        [Fact]
        public async Task ChangeGraphType_ShouldSetReviewMode()
        {
            var vm = CreateVm();

            vm.SelectedGraphType = GraphType.Review;

            await vm.ChangeGrpahType();

            vm.IsReviewGraph.Should().BeTrue();
            vm.IsDynamicGraph.Should().BeFalse();
        }

        [Fact]
        public async Task ChangeGraphType_ShouldSetDynamicMode()
        {
            var vm = CreateVm();

            vm.SelectedGraphType = GraphType.Dynamic;

            await vm.ChangeGrpahType();

            vm.IsReviewGraph.Should().BeFalse();
            vm.IsDynamicGraph.Should().BeTrue();
        }

        [Fact]
        public async Task ShowDynamicGraph_ShouldBuildDynamicChart()
        {
            var transactions = new List<Transaction>();

            var series = new ISeries[]
            {
            Mock.Of<ISeries>()
            };

            var axes = new Axis[]
            {
            new Axis()
            };

            var chart = new ChartData(series, axes);

            _mediator.Setup(x =>
                    x.Send(
                        It.IsAny<GetAllTransactionsQuery>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactions);

            _analyticsService.Setup(x =>
                    x.BuildTransactionsDynamicChart(
                        transactions,
                        It.IsAny<DateTime>(),
                        It.IsAny<DateTime>()))
                .Returns(chart);

            var vm = CreateVm();

            await vm.ShowDynamicGrpah();

            vm.IsPieGraph.Should().BeFalse();
            vm.Series.Should().BeSameAs(series);
            vm.XAxes.Should().BeSameAs(axes);

            _analyticsService.Verify(x =>
                x.BuildTransactionsDynamicChart(
                    transactions,
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()),
                Times.Once);
        }

        [Fact]
        public async Task ShowColumnGraph_ShouldBuildColumnChart()
        {
            var transactions = new List<Transaction>();

            var series = new ISeries[]
            {
            Mock.Of<ISeries>()
            };

            var axes = new Axis[]
            {
            new Axis()
            };

            var chart = new ChartData(series, axes);

            _mediator.Setup(x =>
                    x.Send(
                        It.IsAny<GetAllTransactionsQuery>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactions);

            _analyticsService.Setup(x =>
                    x.BuildTransactionsColumnChart(
                        transactions,
                        It.IsAny<DateTime>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<Func<Transaction, string>>(),
                        It.IsAny<GraphSumType>()))
                .Returns(chart);

            var vm = CreateVm();

            await vm.ShowColumnGrpah();

            vm.IsPieGraph.Should().BeFalse();
            vm.Series.Should().BeSameAs(series);
            vm.XAxes.Should().BeSameAs(axes);

            _analyticsService.Verify(x =>
                x.BuildTransactionsColumnChart(
                    transactions,
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<Func<Transaction, string>>(),
                    It.IsAny<GraphSumType>()),
                Times.Once);
        }

        [Fact]
        public async Task ShowPieGraph_ShouldBuildPieChart()
        {
            var transactions = new List<Transaction>();

            var series = new ISeries[]
            {
            Mock.Of<ISeries>()
            };

            var axes = new Axis[]
            {
            new Axis()
            };

            var chart = new ChartData(series, axes);

            _mediator.Setup(x =>
                    x.Send(
                        It.IsAny<GetAllTransactionsQuery>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactions);

            _analyticsService.Setup(x =>
                    x.BuildTransactionsPieChart(
                        transactions,
                        It.IsAny<DateTime>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<Func<Transaction, string>>(),
                        It.IsAny<GraphSumType>()))
                .Returns(chart);

            var vm = CreateVm();

            await vm.ShowPieGrpah();

            vm.IsPieGraph.Should().BeTrue();
            vm.Series.Should().BeSameAs(series);
            vm.XAxes.Should().BeSameAs(axes);

            _analyticsService.Verify(x =>
                x.BuildTransactionsPieChart(
                    transactions,
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<Func<Transaction, string>>(),
                    It.IsAny<GraphSumType>()),
                Times.Once);
        }

        [Theory]
        [InlineData(GraphGroupsType.Description)]
        [InlineData(GraphGroupsType.Account)]
        [InlineData(GraphGroupsType.Category)]
        [InlineData(GraphGroupsType.FamilyMember)]
        public async Task ShowColumnGraph_ShouldPassSelectedGroupType(
            GraphGroupsType groupType)
        {
            var transactions = new List<Transaction>();

            _mediator.Setup(x =>
                    x.Send(
                        It.IsAny<GetAllTransactionsQuery>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactions);

            _analyticsService.Setup(x =>
                    x.BuildTransactionsColumnChart(
                        It.IsAny<IEnumerable<Transaction>>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<Func<Transaction, string>>(),
                        It.IsAny<GraphSumType>()))
                .Returns(new ChartData([], []));

            var vm = CreateVm();

            vm.SelectedGraphGroupType = groupType;

            await vm.ShowColumnGrpah();

            _analyticsService.Verify(x =>
                x.BuildTransactionsColumnChart(
                    It.IsAny<IEnumerable<Transaction>>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<Func<Transaction, string>>(),
                    It.IsAny<GraphSumType>()),
                Times.Once);
        }

        [Fact]
        public void Receive_DateRangeChanged_ShouldNotThrow()
        {
            var vm = CreateVm();

            var act = () => vm.Receive(
                new DateRangeChangedMessage(
                    DateTime.Today.AddDays(-7),
                    DateTime.Today));

            act.Should().NotThrow();
        }

        [Fact]
        public void Constructor_ShouldInitializeCollections()
        {
            var vm = CreateVm();

            vm.GraphTypes.Should().HaveCount(2);
            vm.GraphSumTypes.Should().HaveCount(4);
            vm.GraphGroupTypes.Should().HaveCount(4);

            vm.Series.Should().NotBeNull();
            vm.XAxes.Should().NotBeNull();
        }

        [Fact]
        public void IsDynamicGraph_ShouldBeInverseOfReviewGraph()
        {
            var vm = CreateVm();

            vm.IsReviewGraph = true;
            vm.IsDynamicGraph.Should().BeFalse();

            vm.IsReviewGraph = false;
            vm.IsDynamicGraph.Should().BeTrue();
        }

        [Fact]
        public void IsColumnGraph_ShouldBeInverseOfPieGraph()
        {
            var vm = CreateVm();

            vm.IsPieGraph = true;
            vm.IsColumnGrpah.Should().BeFalse();

            vm.IsPieGraph = false;
            vm.IsColumnGrpah.Should().BeTrue();
        }
    }
}
