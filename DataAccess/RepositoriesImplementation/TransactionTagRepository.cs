using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{
    public class TransactionTagRepository 
        : Repository<TransactionTag>,
        ITransactionTagRepository
    {

        public TransactionTagRepository(IDbContextFactory<FinansiesDbContext> factory)
            :base(factory)
        {
        }
    }

}
