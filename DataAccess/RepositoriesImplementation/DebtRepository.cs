using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{

    public class DebtRepository 
        : Repository<Debt>,
        IDebtRepository
    {

        public DebtRepository(IDbContextFactory<FinansiesDbContext> factory)
            :base(factory)
        {
        }


        //public async Task AddCategory(Guid userId, Guid id, Category category)
        //{
        //    await using var db = await _factory.CreateDbContextAsync();

        //    var entity = await db.Debts
        //        .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        //    if (entity is not null)
        //    {
        //        entity.CategoryId = category.Id;
        //        await db.SaveChangesAsync();
        //    }
        //}

        //public async Task AddFamilyMember(Guid userId, Guid id, FamilyMember familyMember)
        //{
        //    await using var db = await _factory.CreateDbContextAsync();

        //    var entity = await db.Debts
        //        .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        //    if (entity is not null)
        //    {
        //        entity.FamilyMemberId = familyMember.Id;
        //        await db.SaveChangesAsync();
        //    }
        //}

        //public async Task PayOffDebt(Guid userId, Guid debtId, decimal amount, DateTime date)
        //{
        //    await using var db = await _factory.CreateDbContextAsync();

        //    var debt = db.Debts.FirstOrDefault(x => x.Id == debtId && x.UserId == userId);

        //    if (debt is null)
        //    {
        //        throw new ArgumentException($"No Debt with Id: {debtId}");
        //    }

        //    debt.MakeAPayment(amount, date);

        //    await db.SaveChangesAsync();
        //}
    }
}
