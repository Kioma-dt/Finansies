using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccess.RepositoriesImplementation
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IDbContextFactory<FinansiesDbContext> _factory;

        public TransactionRepository(IDbContextFactory<FinansiesDbContext> factory)
        {
            _factory = factory;
        }

        public async Task Add(Transaction entity)
        {
            await using var db = await _factory.CreateDbContextAsync();

            await db.Transactions.AddAsync(entity);
            await db.SaveChangesAsync();
        }

        public async Task<List<Transaction>?> GetAll(Guid userId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.Transactions
                .Where(x => x.UserId == userId)
                .Include(x => x.Account)
                .Include(x => x.Category)
                .Include(x => x.FamilyMember)
                .Include(x => x.TransactionTags)
                .Include(x => x.User)
                .ToListAsync();
        }

        public async Task<List<Transaction>?> GetAllScalar(Guid userId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.Transactions
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Transaction>> GetWithSpecification(Guid userId, Expression<Func<Transaction, bool>> specification)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.Transactions
                .Where(x => x.UserId == userId)
                .Where(specification)
                .ToListAsync();
        }


        public async Task<Transaction?> GetById(Guid userId, Guid id)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.Transactions
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        }

        public async Task Update(Transaction entity)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var dbEntity = await db.Transactions
                .FirstOrDefaultAsync(x => x.Id == entity.Id && x.UserId == entity.UserId);

            if (dbEntity is null)
            {
                await db.Transactions.AddAsync(entity);
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

            await db.SaveChangesAsync();
        }
    }

}
