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

        public async Task Update(TransactionTag transactionTag)
        {
            var dbTransactionTag = await GetById(transactionTag.UserId, transactionTag.Id);

            if (dbTransactionTag is null)
            {
                await this.Add(transactionTag);
            }
            else
            {
                //Need Copy
                dbTransactionTag.Name = transactionTag.Name;
            }

            await _dbContext.SaveChangesAsync();
        }
    }

}
