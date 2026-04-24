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

        public async Task Add(Debt debt)
        {
            await _dbContext.Accounts.AddAsync(debt);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Debt?> GetById(Guid userId, Guid id)
        {
            return await _dbContext.Debts
                .Select(a => a)
                .Where(a => a.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task Update(Debt debt)
        {
            var dbDebt = await GetById(debt.UserId, debt.Id);

            if (dbDebt is null)
            {
                await this.Add(debt);
            }
            else
            {
                dbDebt.CapitalisationsPerYear = debt.CapitalisationsPerYear;
                dbDebt.CategoryId = debt.CategoryId;
                dbDebt.EndDate = debt.EndDate;
                dbDebt.FamilyMemberId = debt.FamilyMemberId;
                dbDebt.FixedAddition = debt.FixedAddition;
                dbDebt.InterestRate = debt.InterestRate;
                dbDebt.InterestType = debt.InterestType;
                dbDebt.LastPaidDate = debt.LastPaidDate;
                dbDebt.Name = debt.Name;
                dbDebt.PaidAmount = debt.PaidAmount;
                dbDebt.StartAmount = debt.StartAmount;
                dbDebt.StartDate = debt.StartDate;
                dbDebt.TotalAmount = debt.TotalAmount;
                dbDebt.UserId = debt.UserId;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
