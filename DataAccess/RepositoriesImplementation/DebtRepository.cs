using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{
    public class DebtRepository : IDebtRepository
    {
        private readonly IDbContextFactory<FinansiesDbContext> _factory;

        public DebtRepository(IDbContextFactory<FinansiesDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<Debt>> GetAll(Guid userId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.Debts
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task Add(Debt debt)
        {
            await using var db = await _factory.CreateDbContextAsync();

            await db.Debts.AddAsync(debt);
            await db.SaveChangesAsync();
        }

        public async Task<Debt?> GetById(Guid userId, Guid id)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.Debts
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        }

        public async Task AddCategory(Guid userId, Guid id, Category category)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var entity = await db.Debts
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (entity is not null)
            {
                entity.CategoryId = category.Id;
                await db.SaveChangesAsync();
            }
        }

        public async Task AddFamilyMember(Guid userId, Guid id, FamilyMember familyMember)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var entity = await db.Debts
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (entity is not null)
            {
                entity.FamilyMemberId = familyMember.Id;
                await db.SaveChangesAsync();
            }
        }

        public async Task Update(Debt debt)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var dbEntity = await db.Debts
                .FirstOrDefaultAsync(x => x.Id == debt.Id && x.UserId == debt.UserId);

            if (dbEntity is null)
            {
                await db.Debts.AddAsync(debt);
            }
            else
            {
                dbEntity.Name = debt.Name;
                dbEntity.StartAmount = debt.StartAmount;
                dbEntity.TotalAmount = debt.TotalAmount;
                dbEntity.PaidAmount = debt.PaidAmount;
                dbEntity.InterestRate = debt.InterestRate;
                dbEntity.CapitalisationsPerYear = debt.CapitalisationsPerYear;
                dbEntity.FixedAddition = debt.FixedAddition;
                dbEntity.Type = debt.Type;
                dbEntity.InterestType = debt.InterestType;
                dbEntity.StartDate = debt.StartDate;
                dbEntity.LastPaidDate = debt.LastPaidDate;
                dbEntity.EndDate = debt.EndDate;
                dbEntity.CategoryId = debt.CategoryId;
                dbEntity.FamilyMemberId = debt.FamilyMemberId;
                dbEntity.UserId = debt.UserId;
            }

            await db.SaveChangesAsync();
        }
    }
}
