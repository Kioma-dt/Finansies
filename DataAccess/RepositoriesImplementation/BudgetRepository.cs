using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{
    public class BudgetRepository 
        : Repository<Budget>,
        IBudgetRepository
    {
        public BudgetRepository(IDbContextFactory<FinansiesDbContext> dbContextFactory)
            :base(dbContextFactory)
        {
        }

        public async Task AddBudgetFilter(Guid userId, BudgetFilter filter)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var budget = await dbContext.Budgets
                .Include(b => b.Filters)
                .FirstOrDefaultAsync(b => b.Id == filter.BudgetId && b.UserId == userId);

            if (budget is not null)
            {
                budget.Filters.Add(filter);
                await dbContext.BudgetFilters.AddAsync(filter);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
