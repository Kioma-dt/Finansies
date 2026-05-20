using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace DataAccess.RepositoriesImplementation
{
    public class AccountRepository
        : Repository<Account>,
        IAccountRepository
    {
        public AccountRepository(IDbContextFactory<FinansiesDbContext> dbContextFactory) 
            : base(dbContextFactory)
        {
        }
    }
}
