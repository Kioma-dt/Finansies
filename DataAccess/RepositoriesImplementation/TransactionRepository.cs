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

        public async Task<List<Transaction>> GetAll(Guid userId)
        {
            return await _dbContext.Transactions
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<Transaction?> GetById(Guid userId, Guid id)
        {
            return await _dbContext.Transactions
                .Select(a => a)
                .Where(a => a.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task AddCategory(Guid userId, Guid id, Category category)
        {
            var transaction = await GetById(userId, id);

            if (transaction is not null)
            {
                transaction.CategoryId = category.Id;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task AddFamilyMember(Guid userId, Guid id, FamilyMember familyMember)
        {
            var transaction = await GetById(userId, id);

            if (transaction is not null)
            {
                transaction.FamilyMemberId = familyMember.Id;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task AddTag(Guid userId, Guid id, TransactionTag tag)
        {
            var transaction = await GetById(userId, id);

            if (transaction is not null)
            {
                transaction.TransactionTags.Add(tag);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Transaction>> GetWithSpecification(Guid userId, Expression<Func<Transaction, bool>> specification)
        {
            return await _dbContext.Transactions.Where(specification).ToListAsync();
        }

        public async Task Update(Transaction entity)
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
                dbEntity.Date = entity.Date;
                dbEntity.Type = entity.Type;

                dbEntity.AccountId = entity.AccountId;
                dbEntity.CategoryId = entity.CategoryId;
                dbEntity.FamilyMemberId = entity.FamilyMemberId;
                dbEntity.UserId = entity.UserId;
            }

            await _dbContext.SaveChangesAsync();
        }
    }

}
