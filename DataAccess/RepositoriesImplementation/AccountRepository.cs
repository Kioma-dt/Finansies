using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{
    public class AccountRepository : IAccountRepository
    {
        readonly FinansiesDbContext _dbContext;

        public AccountRepository(FinansiesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Account account)
        {
            await _dbContext.Accounts.AddAsync(account);
        }

        public async Task<Account> GetById(Guid userId, Guid id)
        {
            return await _dbContext.Accounts
                .Select(a => a)
                .Where(a => a.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public Task Update(Account account)
        {
            throw new NotImplementedException();
        }
    }
}
