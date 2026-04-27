using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{
    public class BudgetRepository : IBudgetRepository
    {
        readonly FinansiesDbContext _dbContext;

        public BudgetRepository(FinansiesDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Add(Budget budget)
        {
            await _dbContext.Budgets.AddAsync(budget);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Budget>> GetAll(Guid userId)
        {
            return await _dbContext.Budgets
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<Budget?> GetById(Guid userId, Guid id)
        {
            return await _dbContext.Budgets
                .Select(b => b)
                .Where(b => b.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task AddBudgetFilter(Guid userId, Guid id, BudgetFilter filter)
        {
            var budget = await GetById(userId, id);

            
            if (budget is not null)
            {
                budget.Filters.Add(filter);
            }

            await _dbContext.BudgetFilters.AddAsync(filter);

            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Budget budget)
        {
            var dbEntity = await GetById(budget.UserId, budget.Id);

            if (dbEntity is null)
            {
                await Add(budget);
            }
            else
            {
                dbEntity.Name = budget.Name;
                dbEntity.Limit = budget.Limit;
                dbEntity.StartDate = budget.StartDate;
                dbEntity.EndDate = budget.EndDate;
                dbEntity.UserId = budget.UserId;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
