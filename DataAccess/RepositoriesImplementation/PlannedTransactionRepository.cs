using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{
    public class PlannedTransactionRepository 
        : Repository<PlannedTransaction>,
        IPlannedTransactionRepository
    {
        //.Include(x => x.Account)
        //        .Include(x => x.Category)
        //        .Include(x => x.FamilyMember)
        //        .Include(x => x.TransactionTags)
        //        .Include(x => x.Debt)
        //        .Include(x => x.User)
        public PlannedTransactionRepository(IDbContextFactory<FinansiesDbContext> factory)
            :base(factory)
        {
        }

        //public async Task AddAccount(Guid userId, Guid id, Account account)
        //{
        //    await using var db = await _factory.CreateDbContextAsync();

        //    var entity = await db.PlannedTransactions
        //        .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        //    if (entity != null)
        //    {
        //        entity.AccountId = account.Id;
        //        await db.SaveChangesAsync();
        //    }
        //}

        //public async Task AddFamilyMember(Guid userId, Guid id, FamilyMember familyMember)
        //{
        //    await using var db = await _factory.CreateDbContextAsync();

        //    var entity = await db.PlannedTransactions
        //        .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        //    if (entity != null)
        //    {
        //        entity.FamilyMemberId = familyMember.Id;
        //        await db.SaveChangesAsync();
        //    }
        //}

        //public async Task AddCategory(Guid userId, Guid id, Category category)
        //{
        //    await using var db = await _factory.CreateDbContextAsync();

        //    var entity = await db.PlannedTransactions
        //        .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        //    if (entity != null)
        //    {
        //        entity.CategoryId = category.Id;
        //        await db.SaveChangesAsync();
        //    }
        //}

        //public async Task AddTag(Guid userId, Guid id, TransactionTag tag)
        //{
        //    await using var db = await _factory.CreateDbContextAsync();

        //    var entity = await db.PlannedTransactions
        //        .Include(x => x.TransactionTags)
        //        .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        //    if (entity != null)
        //    {
        //        entity.TransactionTags.Add(tag);
        //        await db.SaveChangesAsync();
        //    }
        //}

    }

}
