using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{
    public class DebtRepository : IDebtRepository
    {
        readonly FinansiesDbContext _dbContext;

        public DebtRepository(FinansiesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Debt>> GetAll(Guid userId)
        {
            return await _dbContext.Debts
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task Add(Debt debt)
        {
            await _dbContext.Debts.AddAsync(debt);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Debt?> GetById(Guid userId, Guid id)
        {
            return await _dbContext.Debts
                .Select(a => a)
                .Where(a => a.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task AddCategory(Guid userId, Guid id, Category category)
        {
            var debt = await GetById(userId, id);

            if (debt is not null)
            {
                debt.CategoryId = category.Id;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task AddFamilyMember(Guid userId, Guid id, FamilyMember familyMember)
        {
            var debt = await GetById(userId, id);

            if (debt is not null)
            {
                debt.FamilyMemberId = familyMember.Id;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Debt debt)
        {
            var dbEntity = await GetById(debt.UserId, debt.Id);

            if (dbEntity is null)
            {
                await Add(debt);
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

            await _dbContext.SaveChangesAsync();
        }
    }
}
