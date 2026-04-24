using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{
    public class FamilyMemberRepository : IFamilyMemberRepository
    {
        readonly FinansiesDbContext _dbContext;

        public FamilyMemberRepository(FinansiesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(FamilyMember familyMember)
        {
            await _dbContext.FamilyMembers.AddAsync(familyMember);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<FamilyMember?> GetById(Guid userId, Guid id)
        {
            return await _dbContext.FamilyMembers
                .Select(a => a)
                .Where(a => a.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task Update(FamilyMember familyMember)
        {
            var dbFamilyMember = await GetById(familyMember.UserId, familyMember.Id);

            if (dbFamilyMember is null)
            {
                await this.Add(familyMember);
            }
            else
            {
                dbFamilyMember.Name = familyMember.Name;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
