using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace DataAccess.RepositoriesImplementation
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDbContextFactory<FinansiesDbContext> _dbContextFactory;

        public AccountRepository(IDbContextFactory<FinansiesDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<Account>?> GetAll(Guid userId)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            return await dbContext.Accounts
                .Where(x => x.UserId == userId)

                .Include(x => x.Parent)
                .Include(x => x.Children)

                .Include(x => x.Transactions)
                .Include(x => x.PlannedTransactions)

                .Include(x => x.TransfersFrom)
                .Include(x => x.TransfersTo)

                .Include(x => x.FamilyMember)
                .Include(x => x.User)

                .ToListAsync();
        }

        public async Task<List<Account>?> GetAllScalar(Guid userId)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            return await dbContext.Accounts
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task Add(Account account)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();


            account.Parent = null;

            await dbContext.Accounts.AddAsync(account);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddParent(Guid userId, Guid id, Account parent)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var account = await dbContext.Accounts
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (account is not null)
            {
                account.ParentId = parent.Id;
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task AddFamilyMember(Guid userId, Guid id, FamilyMember familyMember)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var account = await dbContext.Accounts
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (account is not null)
            {
                account.FamilyMemberId = familyMember.Id;
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<Account?> GetById(Guid userId, Guid id)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            return await dbContext.Accounts
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        }

        public async Task Update(Account account)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var dbAccount = await dbContext.Accounts
                .FirstOrDefaultAsync(a => a.Id == account.Id && a.UserId == account.UserId);

            if (dbAccount is null)
            {
                await dbContext.Accounts.AddAsync(account);
            }
            else
            {
                dbAccount.Name = account.Name;
                dbAccount.Balance = account.Balance;
                dbAccount.ParentId = account.ParentId;
                dbAccount.FamilyMemberId = account.FamilyMemberId;
                dbAccount.UserId = account.UserId;
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
