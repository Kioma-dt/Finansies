using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{
    public class PlannedTransactionRepository : IPlannedTransactionRepository
    {
        private readonly IDbContextFactory<FinansiesDbContext> _factory;

        public PlannedTransactionRepository(IDbContextFactory<FinansiesDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<PlannedTransaction>> GetAll(Guid userId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.PlannedTransactions
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task Add(PlannedTransaction entity)
        {
            await using var db = await _factory.CreateDbContextAsync();

            await db.PlannedTransactions.AddAsync(entity);
            await db.SaveChangesAsync();
        }

        public async Task<PlannedTransaction?> GetById(Guid userId, Guid id)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.PlannedTransactions
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        }

        public async Task AddAccount(Guid userId, Guid id, Account account)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var entity = await db.PlannedTransactions
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (entity != null)
            {
                entity.AccountId = account.Id;
                await db.SaveChangesAsync();
            }
        }

        public async Task AddFamilyMember(Guid userId, Guid id, FamilyMember familyMember)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var entity = await db.PlannedTransactions
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (entity != null)
            {
                entity.FamilyMemberId = familyMember.Id;
                await db.SaveChangesAsync();
            }
        }

        public async Task AddCategory(Guid userId, Guid id, Category category)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var entity = await db.PlannedTransactions
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (entity != null)
            {
                entity.CategoryId = category.Id;
                await db.SaveChangesAsync();
            }
        }

        public async Task AddTag(Guid userId, Guid id, TransactionTag tag)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var entity = await db.PlannedTransactions
                .Include(x => x.TransactionTags)
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (entity != null)
            {
                entity.TransactionTags.Add(tag);
                await db.SaveChangesAsync();
            }
        }

        public async Task Update(PlannedTransaction entity)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var dbEntity = await db.PlannedTransactions
                .FirstOrDefaultAsync(x => x.Id == entity.Id && x.UserId == entity.UserId);

            if (dbEntity is null)
            {
                await db.PlannedTransactions.AddAsync(entity);
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

            await db.SaveChangesAsync();
        }
    }

}
