using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

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
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Account?> GetById(Guid userId, Guid id)
        {
            return await _dbContext.Accounts
                .Select(a => a)
                .Where(a => a.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task Update(Account account)
        {
            var dbAccount = await GetById(account.UserId, account.Id);

            if (dbAccount is null)
            {
                await this.Add(account);
            }
            else
            {
                dbAccount.Name = account.Name;
                dbAccount.Balance = account.Balance;
                dbAccount.Children = account.Children;
                dbAccount.PlannedTransactions = account.PlannedTransactions;
                dbAccount.Transactions = account.Transactions;
                dbAccount.TransfersFrom = account.TransfersFrom;
                dbAccount.ParentId = account.ParentId;
                dbAccount.FamilyMemberId = account.FamilyMemberId;
                dbAccount.UserId = account.UserId;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
