using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<FinansiesDbContext> _factory;

        public UserRepository(IDbContextFactory<FinansiesDbContext> factory)
        {
            _factory = factory;
        }

        public async Task Add(User user)
        {
            await using var db = await _factory.CreateDbContextAsync();

            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
        }

        public async Task<User?> GetById(Guid id)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.Users
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Update(User user)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var dbEntity = await db.Users
                .FirstOrDefaultAsync(x => x.Id == user.Id);

            if (dbEntity is null)
            {
                await db.Users.AddAsync(user);
            }
            else
            {
                dbEntity.Name = user.Name;
            }

            await db.SaveChangesAsync();
        }
    }
}
