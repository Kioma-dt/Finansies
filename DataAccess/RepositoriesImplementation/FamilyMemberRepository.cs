using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{
    public class FamilyMemberRepository : IFamilyMemberRepository
    {
        private readonly IDbContextFactory<FinansiesDbContext> _factory;

        public FamilyMemberRepository(IDbContextFactory<FinansiesDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<FamilyMember>> GetAll(Guid userId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.FamilyMembers
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task Add(FamilyMember familyMember)
        {
            await using var db = await _factory.CreateDbContextAsync();

            await db.FamilyMembers.AddAsync(familyMember);
            await db.SaveChangesAsync();
        }

        public async Task<FamilyMember?> GetById(Guid userId, Guid id)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.FamilyMembers
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        }

        public async Task Update(FamilyMember member)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var dbEntity = await db.FamilyMembers
                .FirstOrDefaultAsync(x => x.Id == member.Id && x.UserId == member.UserId);

            if (dbEntity is null)
            {
                await db.FamilyMembers.AddAsync(member);
            }
            else
            {
                dbEntity.Name = member.Name;
                dbEntity.UserId = member.UserId;
            }

            await db.SaveChangesAsync();
        }
    }
}
