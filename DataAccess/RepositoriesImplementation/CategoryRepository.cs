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

        public async Task Update(Category category)
        {
            var dbCategory = await GetById(category.UserId, category.Id);

            if (dbCategory is null)
            {
                await this.Add(category);
            }
            else
            {
                dbCategory.UserId = category.UserId;
                dbCategory.ParentId = category.ParentId;
                dbCategory.Description = category.Description;
                dbCategory.Name = category.Name;
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
