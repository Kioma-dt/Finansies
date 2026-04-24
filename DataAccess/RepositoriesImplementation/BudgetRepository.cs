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

        public async Task<Budget?> GetById(Guid userId, Guid id)
        {
            return await _dbContext.Budgets
                .Select(b => b)
                .Where(b => b.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task Update(Budget budget)
        {
            var dbBudget = await GetById(budget.UserId, budget.Id);

            if (dbBudget is null)
            {
                await this.Add(budget);
            }
            else
            {
                dbBudget.Limit = budget.Limit;
                dbBudget.Name = budget.Name;
                dbBudget.StartDate = budget.StartDate;
                dbBudget.EndDate = budget.EndDate;
                dbBudget.UserId = budget.UserId;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
