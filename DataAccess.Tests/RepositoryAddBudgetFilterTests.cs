namespace DataAccess.Tests
{
    using BuisnessLogic.Entities;
    using BuisnessLogic.Enums;
    using DataAccess.RepositoriesImplementation;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;

    public class RepositoryAddBudgetFilterTests
    {
        private string CreateDbName() => Guid.NewGuid().ToString();

        private IDbContextFactory<FinansiesDbContext> CreateFactory(string dbName)
        {
            var options = new DbContextOptionsBuilder<FinansiesDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            var factory = new Mock<IDbContextFactory<FinansiesDbContext>>();

            factory.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new TestFinansiesDbContext(options));

            return factory.Object;
        }

        private TestFinansiesDbContext CreateVerifyContext(string dbName)
        {
            return new TestFinansiesDbContext(
                new DbContextOptionsBuilder<FinansiesDbContext>()
                    .UseInMemoryDatabase(dbName)
                    .Options);
        }

        [Fact]
        public async Task AddBudgetFilter_ShouldAddFilterToBudget()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);

            var repo = new BudgetRepository(factory);

            var userId = Guid.NewGuid();

            var budgetId = Guid.NewGuid();

            await using (var context = CreateVerifyContext(dbName))
            {
                context.Budgets.Add(new Budget
                {
                    Id = budgetId,
                    UserId = userId,
                    Filters = new List<BudgetFilter>()
                });

                await context.SaveChangesAsync();
            }

            var filter = new BudgetFilter
            {
                Id = Guid.NewGuid(),
                BudgetId = budgetId,
                Type = BudgetFilterType.Account,
                Value = Guid.NewGuid().ToString()
            };

            await repo.AddBudgetFilter(userId, filter);

            await using var verifyContext = CreateVerifyContext(dbName);

            var budget = await verifyContext.Budgets
                .Include(x => x.Filters)
                .FirstAsync();

            budget.Filters.Should().ContainSingle();

            budget.Filters.First().Id.Should().Be(filter.Id);
        }

        [Fact]
        public async Task AddBudgetFilter_ShouldNotAddFilter_WhenBudgetDoesNotExist()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);

            var repo = new BudgetRepository(factory);

            var filter = new BudgetFilter
            {
                Id = Guid.NewGuid(),
                BudgetId = Guid.NewGuid(),
                Type = BudgetFilterType.Account,
                Value = Guid.NewGuid().ToString()
            };

            await repo.AddBudgetFilter(Guid.NewGuid(), filter);

            await using var verifyContext = CreateVerifyContext(dbName);

            var filtersCount = await verifyContext.BudgetFilters.CountAsync();

            filtersCount.Should().Be(0);
        }

        [Fact]
        public async Task AddBudgetFilter_ShouldNotAddFilter_WhenBudgetBelongsToAnotherUser()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);

            var repo = new BudgetRepository(factory);

            var ownerId = Guid.NewGuid();

            var anotherUserId = Guid.NewGuid();

            var budgetId = Guid.NewGuid();

            await using (var context = CreateVerifyContext(dbName))
            {
                context.Budgets.Add(new Budget
                {
                    Id = budgetId,
                    UserId = ownerId,
                    Filters = new List<BudgetFilter>()
                });

                await context.SaveChangesAsync();
            }

            var filter = new BudgetFilter
            {
                Id = Guid.NewGuid(),
                BudgetId = budgetId,
                Type = BudgetFilterType.Category,
                Value = Guid.NewGuid().ToString()
            };

            await repo.AddBudgetFilter(anotherUserId, filter);

            await using var verifyContext = CreateVerifyContext(dbName);

            var budget = await verifyContext.Budgets
                .Include(x => x.Filters)
                .FirstAsync();

            budget.Filters.Should().BeEmpty();
        }

        [Fact]
        public async Task AddBudgetFilter_ShouldAppendFilter_WhenBudgetAlreadyContainsFilters()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);

            var repo = new BudgetRepository(factory);

            var userId = Guid.NewGuid();

            var budgetId = Guid.NewGuid();

            await using (var context = CreateVerifyContext(dbName))
            {
                context.Budgets.Add(new Budget
                {
                    Id = budgetId,
                    UserId = userId,
                    Filters =
                    [
                        new BudgetFilter
                    {
                        Id = Guid.NewGuid(),
                        BudgetId = budgetId,
                        Type = BudgetFilterType.Account,
                        Value = "Old"
                    }
                    ]
                });

                await context.SaveChangesAsync();
            }

            var newFilter = new BudgetFilter
            {
                Id = Guid.NewGuid(),
                BudgetId = budgetId,
                Type = BudgetFilterType.Category,
                Value = "New"
            };

            await repo.AddBudgetFilter(userId, newFilter);

            await using var verifyContext = CreateVerifyContext(dbName);

            var budget = await verifyContext.Budgets
                .Include(x => x.Filters)
                .FirstAsync();

            budget.Filters.Should().HaveCount(2);

            budget.Filters.Should()
                .Contain(x => x.Id == newFilter.Id);
        }

        [Fact]
        public async Task AddBudgetFilter_ShouldPersistFilterValue()
        {
            var dbName = CreateDbName();

            var factory = CreateFactory(dbName);

            var repo = new BudgetRepository(factory);

            var userId = Guid.NewGuid();

            var budgetId = Guid.NewGuid();

            await using (var context = CreateVerifyContext(dbName))
            {
                context.Budgets.Add(new Budget
                {
                    Id = budgetId,
                    UserId = userId,
                    Filters = new List<BudgetFilter>()
                });

                await context.SaveChangesAsync();
            }

            var filter = new BudgetFilter
            {
                Id = Guid.NewGuid(),
                BudgetId = budgetId,
                Type = BudgetFilterType.TransactionType,
                Value = "Expense"
            };

            await repo.AddBudgetFilter(userId, filter);

            await using var verifyContext = CreateVerifyContext(dbName);

            var savedFilter = await verifyContext.BudgetFilters.FirstAsync();

            savedFilter.Value.Should().Be("Expense");
            savedFilter.Type.Should().Be(BudgetFilterType.TransactionType);
        }
    }
}
