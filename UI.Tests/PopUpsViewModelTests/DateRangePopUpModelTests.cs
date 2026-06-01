using UI.Popups;
using UI.PopUps.ViewModels;

namespace UI.Tests.PopUpsViewModelTests
{
    public class DateRangePopUpModelTests
    {
        private DateRangePopUpModel CreateVm()
            => new DateRangePopUpModel();

        [Fact]
        public async Task Initialize_ShouldSetDefaultDates()
        {
            var vm = CreateVm();

            await vm.Initialize();

            vm.StartDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(2));
            vm.EndDate.Should().BeCloseTo(DateTime.Now.AddMonths(1), TimeSpan.FromSeconds(2));
        }

        [Fact]
        public async Task Cancel_ShouldReturnNullThroughCloseAction()
        {
            var vm = CreateVm();

            DateRangeDTO? result = new(DateTime.Now, DateTime.Now.AddDays(5));

            vm.CloseAction = dto => result = dto;

            await vm.Cancel();

            result.Should().BeNull();
        }

        [Fact]
        public async Task Create_ShouldReturnValidDateRange_WhenDatesAreCorrect()
        {
            var vm = CreateVm();

            DateRangeDTO? result = null;

            vm.CloseAction = dto => result = dto;

            vm.StartDate = DateTime.Now;
            vm.EndDate = DateTime.Now.AddDays(10);

            await vm.Create();

            result.Should().NotBeNull();
            result!.StartDate.Should().Be(vm.StartDate);
            result.EndDate.Should().Be(vm.EndDate);
        }

        [Fact]
        public async Task Create_ShouldFail_WhenStartDateGreaterThanEndDate()
        {
            var vm = CreateVm();

            bool called = false;

            vm.CloseAction = _ => called = true;

            vm.StartDate = DateTime.Now.AddDays(10);
            vm.EndDate = DateTime.Now;

            await vm.Create();

            called.Should().BeFalse();
        }

        [Fact]
        public async Task Create_ShouldNotThrow_WhenValidRange()
        {
            var vm = CreateVm();

            vm.StartDate = DateTime.Now;
            vm.EndDate = DateTime.Now.AddDays(1);

            vm.CloseAction = _ => { };

            Func<Task> act = async () => await vm.Create();

            await act.Should().NotThrowAsync();
        }
    }
}
