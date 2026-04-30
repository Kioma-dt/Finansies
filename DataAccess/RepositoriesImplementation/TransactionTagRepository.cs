using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{
    public class TransactionTagRepository : ITransactionTagRepository
    {
        private readonly IDbContextFactory<FinansiesDbContext> _factory;

        public TransactionTagRepository(IDbContextFactory<FinansiesDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<TransactionTag>> GetAll(Guid userId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.TransactionTags
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task Add(TransactionTag transactionTag)
        {
            await using var db = await _factory.CreateDbContextAsync();

            await db.TransactionTags.AddAsync(transactionTag);
            await db.SaveChangesAsync();
        }

        public async Task<TransactionTag?> GetById(Guid userId, Guid id)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.TransactionTags
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        }

        public async Task Update(TransactionTag tag)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var dbEntity = await db.TransactionTags
                .FirstOrDefaultAsync(x => x.Id == tag.Id && x.UserId == tag.UserId);

            if (dbEntity is null)
            {
                await db.TransactionTags.AddAsync(tag);
            }
            else
            {
                dbEntity.Name = tag.Name;
                dbEntity.UserId = tag.UserId;
            }

            await db.SaveChangesAsync();
        }
    }

}
