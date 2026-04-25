using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{
    public class UserRepository : IUserRepository
    {
        readonly FinansiesDbContext _dbContext;

        public UserRepository(FinansiesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User?> GetById(Guid id)
        {
            return await _dbContext.Users
                .Select(a => a)
                .FirstOrDefaultAsync();
        }

        public async Task Update(User user)
        {
            var dbEntity = await GetById(user.Id);

            if (dbEntity is null)
            {
                await Add(user);
            }
            else
            {
                dbEntity.Name = user.Name;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
