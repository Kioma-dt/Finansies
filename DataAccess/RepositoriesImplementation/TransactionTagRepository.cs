using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{
    public class TransactionTagRepository : ITransactionTagRepository
    {
        readonly FinansiesDbContext _dbContext;

        public TransactionTagRepository(FinansiesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(TransactionTag transactionTag)
        {
            await _dbContext.TransactionTags.AddAsync(transactionTag);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<TransactionTag?> GetById(Guid userId, Guid id)
        {
            return await _dbContext.TransactionTags
                .Select(a => a)
                .Where(a => a.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task Update(TransactionTag tag)
        {
            var dbEntity = await GetById(tag.UserId, tag.Id);

            if (dbEntity is null)
            {
                await Add(tag);
            }
            else
            {
                dbEntity.Name = tag.Name;
                dbEntity.UserId = tag.UserId;
            }

            await _dbContext.SaveChangesAsync();
        }
    }

}
