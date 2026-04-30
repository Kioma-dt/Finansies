using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{
    public class TransferRepository : ITransferRepository
    {
        private readonly IDbContextFactory<FinansiesDbContext> _factory;

        public TransferRepository(IDbContextFactory<FinansiesDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<Transfer>> GetAll(Guid userId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.Transfers
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task Add(Transfer transfer)
        {
            await using var db = await _factory.CreateDbContextAsync();

            await db.Transfers.AddAsync(transfer);
            await db.SaveChangesAsync();
        }

        public async Task<Transfer?> GetById(Guid userId, Guid id)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.Transfers
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        }

        public async Task Update(Transfer transfer)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var dbEntity = await db.Transfers
                .FirstOrDefaultAsync(x => x.Id == transfer.Id && x.UserId == transfer.UserId);

            if (dbEntity is null)
            {
                await db.Transfers.AddAsync(transfer);
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

            await db.SaveChangesAsync();
        }
    }

}
