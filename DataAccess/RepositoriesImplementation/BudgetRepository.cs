using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly IDbContextFactory<FinansiesDbContext> _dbContextFactory;

        public BudgetRepository(IDbContextFactory<FinansiesDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task Add(Budget budget)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            await dbContext.Budgets.AddAsync(budget);
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<Budget>> GetAll(Guid userId)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            return await dbContext.Budgets
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<Budget?> GetById(Guid userId, Guid id)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            return await dbContext.Budgets
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
        }

        public async Task AddBudgetFilter(Guid userId, Guid id, BudgetFilter filter)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var budget = await dbContext.Budgets
                .Include(b => b.Filters)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (budget is not null)
            {
                budget.Filters.Add(filter);
                await dbContext.BudgetFilters.AddAsync(filter);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task Update(Budget budget)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var dbEntity = await dbContext.Budgets
                .FirstOrDefaultAsync(b => b.Id == budget.Id && b.UserId == budget.UserId);

            if (dbEntity is null)
            {
                await dbContext.Budgets.AddAsync(budget);
            }
            else
            {
                dbEntity.Name = budget.Name;
                dbEntity.Limit = budget.Limit;
                dbEntity.StartDate = budget.StartDate;
                dbEntity.EndDate = budget.EndDate;
                dbEntity.UserId = budget.UserId;
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
