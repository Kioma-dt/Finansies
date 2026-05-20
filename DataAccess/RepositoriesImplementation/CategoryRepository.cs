using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.RepositoriesImplementation
{

    public class CategoryRepository 
        : Repository<Category>,
        ICategoryRepository
    {
        private readonly IDbContextFactory<FinansiesDbContext> _factory;

        public CategoryRepository(IDbContextFactory<FinansiesDbContext> factory)
            :base(factory)
        {
        }


        public async Task SetParent(Guid userId, Guid id, Category parent)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var entity = await db.Categories
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (entity is not null && id != parent.Id)
            {
                entity.ParentId = parent.Id;
                await db.SaveChangesAsync();
            }
        }

    }
}
