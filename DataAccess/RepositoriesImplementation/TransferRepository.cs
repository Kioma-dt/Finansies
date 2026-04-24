using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{
    public class TransferRepository : ITransferRepository
    {
        readonly FinansiesDbContext _dbContext;

        public TransferRepository(FinansiesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Transfer transfer)
        {
            await _dbContext.Transfers.AddAsync(transfer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Transfer?> GetById(Guid userId, Guid id)
        {
            return await _dbContext.Transfers
                .Select(a => a)
                .Where(a => a.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task Update(Transfer transfer)
        {
            var dbTransfer = await GetById(transfer.UserId, transfer.Id);

            if (dbTransfer is null)
            {
                await this.Add(transfer);
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
