using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{
    public class PlannedTransactionRepository : IPlannedTransactionRepository
    {
        readonly FinansiesDbContext _dbContext;

        public PlannedTransactionRepository(FinansiesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(PlannedTransaction plannedTransaction)
        {
            await _dbContext.PlannedTransactions.AddAsync(plannedTransaction);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<PlannedTransaction?> GetById(Guid userId, Guid id)
        {
            return await _dbContext.PlannedTransactions
                .Select(a => a)
                .Where(a => a.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task Update(PlannedTransaction plannedTransaction)
        {
            var dbPlannedTransactions = await GetById(plannedTransaction.UserId, plannedTransaction.Id);

            if (dbPlannedTransactions is null)
            {
                await this.Add(plannedTransaction);
            }
            else
            {
                dbPlannedTransactions.AccountId = plannedTransaction.AccountId;
                dbPlannedTransactions.Amount = plannedTransaction.Amount;
                dbPlannedTransactions.CategoryId = plannedTransaction.CategoryId;
                dbPlannedTransactions.Description = plannedTransaction.Description;
                dbPlannedTransactions.FamilyMemberId = plannedTransaction.FamilyMemberId;
                dbPlannedTransactions.PlannedDate = plannedTransaction.PlannedDate;
                dbPlannedTransactions.Status = plannedTransaction.Status;
                dbPlannedTransactions.UserId = plannedTransaction.UserId;
            }

            await _dbContext.SaveChangesAsync();
        }
    }

}
