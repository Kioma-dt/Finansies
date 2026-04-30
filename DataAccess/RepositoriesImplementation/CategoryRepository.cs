using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDbContextFactory<FinansiesDbContext> _factory;

        public CategoryRepository(IDbContextFactory<FinansiesDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<Category>?> GetAllScalar(Guid userId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.Categories
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Category>?> GetAll(Guid userId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.Categories
                .Where(x => x.UserId == userId)
                .Include(x => x.Parent)
                .Include(x => x.Children)
                .Include(x => x.Transactions)
                .Include(x => x.PlannedTransactions)
                .Include(x => x.Debts)
                .ToListAsync();
        }

        public async Task Add(Category category)
        {
            await using var db = await _factory.CreateDbContextAsync();

            await db.Categories.AddAsync(category);
            await db.SaveChangesAsync();
        }

        public async Task<Category?> GetById(Guid userId, Guid id)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.Categories
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        }

        public async Task AddParent(Guid userId, Guid id, Category parent)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var entity = await db.Categories
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (entity is not null)
            {
                entity.ParentId = parent.Id;
                await db.SaveChangesAsync();
            }
        }

        public async Task Update(Category category)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var dbEntity = await db.Categories
                .FirstOrDefaultAsync(x => x.Id == category.Id && x.UserId == category.UserId);

            if (dbEntity is null)
            {
                await db.Categories.AddAsync(category);
            }
            else
            {
                dbEntity.Name = category.Name;
                dbEntity.Description = category.Description;
                dbEntity.ParentId = category.ParentId;
                dbEntity.UserId = category.UserId;
            }

            await db.SaveChangesAsync();
        }
    }
}
