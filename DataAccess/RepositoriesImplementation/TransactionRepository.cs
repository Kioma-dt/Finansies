using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccess.RepositoriesImplementation
{
    public class TransactionRepository 
        : Repository<Transaction>,
        ITransactionRepository
    {

        public TransactionRepository(IDbContextFactory<FinansiesDbContext> factory)
            :base(factory)
        {
        }

        public async Task<List<Transaction>> GetWithSpecification(Guid userId, Expression<Func<Transaction, bool>> specification)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();

            var transactions = await db.Transactions.ToListAsync();

            transactions = transactions.Where(x => x.UserId == userId).ToList();

            var tr = await db.Transactions
                .Where(specification)
                .Include(x => x.Account)
                .ToListAsync();


            return await db.Transactions
                .Where(x => x.UserId == userId)
                .Where(specification)
                .Include(x => x.Account)
                .Include(x => x.Category)
                .Include(x => x.FamilyMember)
                .Include(x => x.TransactionTags)
                .Include(x => x.Debt)
                .Include(x => x.User)
                .ToListAsync();
        }

    }

}
