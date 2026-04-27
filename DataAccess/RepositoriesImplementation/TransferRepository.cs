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
        public async Task<List<Transfer>> GetAll(Guid userId)
        {
            return await _dbContext.Transfers
                .Where(x => x.UserId == userId)
                .ToListAsync();
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
            var dbEntity = await GetById(transfer.UserId, transfer.Id);

            if (dbEntity is null)
            {
                await Add(transfer);
            }
            else
            {
                dbEntity.Amount = transfer.Amount;
                dbEntity.Description = transfer.Description;
                dbEntity.Date = transfer.Date;

                dbEntity.FromAccountId = transfer.FromAccountId;
                dbEntity.ToAccountId = transfer.ToAccountId;
                dbEntity.UserId = transfer.UserId;
            }

            await _dbContext.SaveChangesAsync();
        }
    }

}
