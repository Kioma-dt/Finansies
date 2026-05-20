using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{
    public class FamilyMemberRepository
        : Repository<FamilyMember>,
        IFamilyMemberRepository
    {

        public FamilyMemberRepository(IDbContextFactory<FinansiesDbContext> factory)
            :base(factory)
        {
        }

    }
}
