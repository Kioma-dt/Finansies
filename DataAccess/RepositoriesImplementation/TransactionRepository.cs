using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccess.RepositoriesImplementation
{
    public class TransactionRepository : ITransactionRepository
    {
        readonly FinansiesDbContext _dbContext;

        public TransactionRepository(FinansiesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Transaction transaction)
        {
            await _dbContext.Transactions.AddAsync(transaction);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Transaction>?> GetAll(Guid userId)
        {
            return await _dbContext.Transactions.ToListAsync();
        }

        public async Task<Transaction?> GetById(Guid userId, Guid id)
        {
            return await _dbContext.Transactions
                .Select(a => a)
                .Where(a => a.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Transaction>?> GetWithSpecification(Guid userId, Expression<Func<Transaction, bool>> specification)
        {
            return await _dbContext.Transactions.Where(specification).ToListAsync();
        }

        public async Task Update(Transaction transaction)
        {
            var dbTransaction = await GetById(transaction.UserId, transaction.Id);

            if (dbTransaction is null)
            {
                await this.Add(transaction);
            }
            else
            {
                dbTransaction.AccountId = transaction.AccountId;
                dbTransaction.Amount = transaction.Amount;
                dbTransaction.CategoryId = transaction.CategoryId;
                dbTransaction.Description = transaction.Description;
                dbTransaction.FamilyMemberId = transaction.FamilyMemberId;
                dbTransaction.Date = transaction.Date;
                dbTransaction.UserId = transaction.UserId;
            }

            await _dbContext.SaveChangesAsync();
        }
    }

}
