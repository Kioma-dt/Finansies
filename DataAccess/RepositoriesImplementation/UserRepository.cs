using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{
    public class UserRepository : IUserRepository
    {
        readonly FinansiesDbContext _dbContext;

        public UserRepository(FinansiesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User?> GetById(Guid id)
        {
            return await _dbContext.Users
                .Select(a => a)
                .FirstOrDefaultAsync();
        }

        public async Task Update(User user)
        {
            var dbUser = await GetById(user.Id);

            if (dbUser is null)
            {
                await this.Add(user);
            }
            else
            {
                //dbAccount.Name = account.Name;
                //dbAccount.Balance = account.Balance;
                //dbAccount.Children = account.Children;
                //dbAccount.PlannedTransactions = account.PlannedTransactions;
                //dbAccount.Transactions = account.Transactions;
                //dbAccount.TransfersFrom = account.TransfersFrom;
                //dbAccount.ParentId = account.ParentId;
                //dbAccount.FamilyMemberId = account.FamilyMemberId;
                //dbAccount.UserId = account.UserId;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
