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
        public async Task<List<Account>?> GetAll(Guid userId)
        {
            return await _dbContext.Accounts
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }


        public async Task Add(Account account)
        {
            await _dbContext.Accounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddParent(Guid userId, Guid id, Account parent)
        {
            var account = await GetById(userId, id);

            if (account is not null)
            {
                account.ParentId = parent.Id;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task AddFamilyMember(Guid userId, Guid id, FamilyMember familyMember)
        {
            var account = await GetById(userId, id);

            if (account is not null)
            {
                account.FamilyMemberId = familyMember.Id;
            }

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
                dbAccount.ParentId = account.ParentId;
                dbAccount.FamilyMemberId = account.FamilyMemberId;
                dbAccount.UserId = account.UserId;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
