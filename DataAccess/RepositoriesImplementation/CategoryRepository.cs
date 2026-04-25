using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{
    public class CategoryRepository : ICategoryRepository
    {
        readonly FinansiesDbContext _dbContext;

        public CategoryRepository(FinansiesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Category category)
        {
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Category?> GetById(Guid userId, Guid id)
        {
            return await _dbContext.Categories
                .Select(a => a)
                .Where(a => a.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task AddParent(Guid userId, Guid id, Category parent)
        {
            var category = await GetById(userId, id);

            if (category is not null)
            {
                category.ParentId = parent.Id;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Category category)
        {
            var dbEntity = await GetById(category.UserId, category.Id);

            if (dbEntity is null)
            {
                await Add(category);
            }
            else
            {
                dbEntity.Name = category.Name;
                dbEntity.Description = category.Description;
                dbEntity.ParentId = category.ParentId;
                dbEntity.UserId = category.UserId;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
