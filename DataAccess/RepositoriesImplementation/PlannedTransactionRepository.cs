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

        public async Task AddAccount(Guid userId, Guid id, Account account)
        {
            var planned = await GetById(userId, id);

            if (planned is not null)
            {
                planned.AccountId = account.Id;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task AddFamilyMember(Guid userId, Guid id, FamilyMember familyMember)
        {
            var planned = await GetById(userId, id);

            if (planned is not null)
            {
                planned.FamilyMemberId = familyMember.Id;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task AddTag(Guid userId, Guid id, TransactionTag tag)
        {
            var planned = await GetById(userId, id);

            if (planned is not null)
            {
                planned.TransactionTags.Add(tag);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task AddCategory(Guid userId, Guid id, Category category)
        {
            var planned = await GetById(userId, id);

            if (planned is not null)
            {
                planned.CategoryId = category.Id;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(PlannedTransaction entity)
        {
            var dbEntity = await GetById(entity.UserId, entity.Id);

            if (dbEntity is null)
            {
                await Add(entity);
            }
            else
            {
                dbEntity.Amount = entity.Amount;
                dbEntity.Description = entity.Description;
                dbEntity.Type = entity.Type;

                dbEntity.PlannedDate = entity.PlannedDate;
                dbEntity.Status = entity.Status;

                dbEntity.AccountId = entity.AccountId;
                dbEntity.CategoryId = entity.CategoryId;
                dbEntity.FamilyMemberId = entity.FamilyMemberId;
                dbEntity.UserId = entity.UserId;
            }

            await _dbContext.SaveChangesAsync();
        }
    }

}
