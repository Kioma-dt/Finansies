using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{
    public class TransferRepository 
        : Repository<Transfer>,
        ITransferRepository
    {
        public TransferRepository(IDbContextFactory<FinansiesDbContext> factory)
            :base(factory)
        {
        }
    }

}
