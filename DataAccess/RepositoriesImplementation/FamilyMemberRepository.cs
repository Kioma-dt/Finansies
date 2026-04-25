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

        public async Task Update(FamilyMember member)
        {
            var dbEntity = await GetById(member.UserId, member.Id);

            if (dbEntity is null)
            {
                await Add(member);
            }
            else
            {
                dbEntity.Name = member.Name;
                dbEntity.UserId = member.UserId;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
